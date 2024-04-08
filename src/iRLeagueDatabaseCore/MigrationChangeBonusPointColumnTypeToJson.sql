/* Migrate the filterOptionEntity used on ResultConfigs to use json column for Conditions 
 * instead of previous FilterConditionEntity
 */

USE TestDatabase;

SET autocommit=0;
START TRANSACTION;

-- Pre migration
-- Create Temporary table holding the bonus points before changing column type
CREATE TEMPORARY TABLE TmpBonusPoints (
	LeagueId BIGINT NOT NULL,
	PointRuleId BIGINT NOT NULL PRIMARY KEY,
	BonusPoints TEXT
);

INSERT INTO	TmpBonusPoints (LeagueId, PointRuleId, BonusPoints)
	SELECT LeagueId, PointRuleId, BonusPoints
	FROM PointRules;

UPDATE PointRules
  SET BonusPoints='[]';

-- Post migration
-- Create function to convert bonus values from single string into json array 
DELIMITER //

DROP FUNCTION IF EXISTS splitStringToBonusPoints;

CREATE FUNCTION splitStringToBonusPoints(inputString TEXT,
  pointDelimiter TEXT)
  RETURNS TEXT
  DETERMINISTIC
BEGIN
  DECLARE bonusType TEXT;
  DECLARE bonusValue TEXT;
  DECLARE bonusPoints TEXT;
  SET bonusType = SUBSTRING_INDEX(inputString,pointDelimiter,1);
  SET bonusPoints = SUBSTRING(SUBSTRING_INDEX(inputString,pointDelimiter,2), LENGTH(bonusType)+2, 10);
  SET bonusValue = IF(LENGTH(bonusType)>1, SUBSTRING(bonusType, 2, 10), '0');
  SET bonusType = SUBSTRING(bonusType, 1, 1);
  SET bonusType = CASE bonusType
    WHEN 'p' THEN '0'
    WHEN 'q' THEN '1'
    WHEN 'f' THEN '2'
    WHEN 'a' THEN '3'
    WHEN 'c' THEN '4'
    WHEN 'n' THEN '5'
    WHEN 'g' THEN '6'
    WHEN 'd' THEN '7'
    WHEN 'l' THEN '8'
    WHEN 'm' THEN '9'
  END;
  RETURN CONCAT('{"Type":',bonusType,',"Value":',bonusValue,',"Points":',bonusPoints,',"Conditions":[]}'); 
END;

DROP FUNCTION IF EXISTS splitToArray;

CREATE FUNCTION splitToArray(
  inputString TEXT,
  arrayDelimiter TEXT,
  pointDelimiter TEXT)
  RETURNS TEXT
  DETERMINISTIC
BEGIN
  DECLARE tempString TEXT;
  SET tempString = '[';
  WHILE LOCATE(arrayDelimiter,inputString) > 1 DO
    SET tempString = CONCAT(tempString, splitStringToBonusPoints(SUBSTRING_INDEX(inputString,arrayDelimiter,1), pointDelimiter), ',');
    SET inputString = REGEXP_REPLACE(inputString, (
      SELECT LEFT(inputString, LOCATE(arrayDelimiter, inputString))
    ),'',1,1);
  END WHILE;
  SET tempString = CONCAT(tempString, splitStringToBonusPoints(SUBSTRING_INDEX(inputString,arrayDelimiter,1), pointDelimiter), ']');
  RETURN tempString;
END; //

DELIMITER ;

-- Populate `Conditions` column with stored values and perform necessary value conversions
UPDATE TmpBonusPoints
    SET BonusPoints = splitToArray(BonusPoints, ';', ':');
UPDATE PointRules AS pr
  JOIN TmpBonusPoints AS tmp
    ON tmp.PointRuleId=pr.PointRuleId
  SET pr.BonusPoints = tmp.BonusPoints
  WHERE tmp.BonusPoints IS NOT NULL;

DROP TABLE TmpBonusPoints;
DROP FUNCTION splitToArray;
DROP FUNCTION splitStringToBonusPoints;

ROLLBACK;