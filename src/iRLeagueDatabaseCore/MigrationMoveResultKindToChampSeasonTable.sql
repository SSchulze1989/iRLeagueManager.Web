/* Migrate the filterOptionEntity used on ResultConfigs to use json column for Conditions 
 * instead of previous FilterConditionEntity
 */

USE TestDatabase;

START TRANSACTION;

DROP TABLE IF EXISTS TmpResultConfigKinds;

CREATE TEMPORARY TABLE TmpResultConfigKinds(
	LeagueId BIGINT NOT NULL,
	ResultConfigId BIGINT NOT NULL,
	ChampSeasonId BIGINT NOT NULL,
	ResultKind VARCHAR(50) NOT NULL,
	PRIMARY KEY(LeagueId, ResultConfigId)
);

INSERT INTO TmpResultConfigKinds(LeagueId, ResultConfigId, ChampSeasonId, ResultKind)
	SELECT LeagueId, ResultConfigId, ChampSeasonId, ResultKind FROM ResultConfigurations;

SELECT * FROM TmpResultConfigKinds;

-- Populate `Conditions` column with stored values and perform necessary value conversions
UPDATE ChampSeasons AS cs
	JOIN TmpResultConfigKinds as rc
		ON cs.ChampSeasonId=rc.ChampSeasonId
	SET cs.ResultKind=rc.ResultKind;

UPDATE ChampSeasons
	SET ResultKind='Member'
	WHERE ResultKind='';

SELECT LeagueId, ChampSeasonId, ResultKind FROM ChampSeasons;

DROP TABLE TmpResultConfigKinds;

ROLLBACK;