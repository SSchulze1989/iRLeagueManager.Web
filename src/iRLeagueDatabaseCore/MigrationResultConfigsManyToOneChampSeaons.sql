USE TestDatabase;

START TRANSACTION;

DELETE r FROM ResultConfigurations r
	LEFT JOIN ChampSeasons_ResultConfigs cr
		ON cr.ResultConfigId=r.ResultConfigId
	WHERE cr.ChampSeasonId is NULL;

UPDATE ResultConfigurations r
	JOIN ChampSeasons_ResultConfigs cr
		ON cr.ResultConfigId=r.ResultConfigId
	SET r.ChampSeasonId=cr.ChampSeasonId;

ROLLBACK;