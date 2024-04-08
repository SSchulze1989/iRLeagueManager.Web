/* Migrate existing PenaltyPoints of ReviewPenaltys table to new JsonColum values
 * AddPenaltys has been dropped in the previous migration so no values need to be converted here
 */

USE TestDatabase;

START TRANSACTION;

-- Pre migration
-- Create Temporary table holding the pointValues
CREATE TEMPORARY TABLE TmpPenaltyPoints (
	LeagueId BIGINT NOT NULL,
	ResultRowId BIGINT NOT NULL,
	ReviewId BIGINT NOT NULL,
	PenaltyPoints INT NOT NULL,
	PRIMARY KEY (LeagueId, ResultRowId, ReviewId)
);

INSERT INTO	TmpPenaltyPoints (LeagueId, ResultRowId, ReviewId, PenaltyPoints)
	SELECT LeagueId, ResultRowId, ReviewId, PenaltyPoints
	FROM ReviewPenaltys;

-- Post migration
UPDATE ReviewPenaltys AS rp
	JOIN TmpPenaltyPoints AS tmp
		ON rp.ResultRowId=tmp.ResultRowId AND rp.ReviewId=tmp.ReviewId
	SET rp.`Value` = CONCAT('{\"Type\": 0, \"Points\": ', tmp.PenaltyPoints, '}');

DROP TABLE TmpPenaltyPoints;

ROLLBACK;