/* Migrate the filterOptionEntity used on ResultConfigs to use json column for Conditions 
 * instead of previous FilterConditionEntity
 */

USE TestDatabase;

START TRANSACTION;

-- Populate `Conditions` column with stored values and perform necessary value conversions
UPDATE FilterOptions AS f
	JOIN ResultConfigurations AS rc
		ON rc.ResultConfigId=f.ResultFilterResultConfigId
	JOIN ChampSeasons AS cs
		ON cs.ChampSeasonId=rc.ChampSeasonId
	JOIN Seasons AS s
		ON cs.SeasonId=s.SeasonId
	WHERE s.Finished=0 AND cs.IsActive
	SET f.ChampSeasonId = rc.ChampSeasonId, f.ResultFilterResultConfigId=NULL;

ROLLBACK;