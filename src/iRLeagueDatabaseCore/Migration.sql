USE TestDatabase;

START TRANSACTION;

CREATE TEMPORARY TABLE TmpChampionships (
	LeagueId BIGINT NOT NULL,
	ChampionshipId BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
	ResultConfigId BIGINT NOT NULL,
	Name VARCHAR(80),
	DisplayName VARCHAR(255),
	Version INT DEFAULT 1
);

INSERT INTO	TmpChampionships (LeagueId, ResultConfigId, Name, DisplayName)
	SELECT LeagueId, ResultConfigId, Name, DisplayName
	FROM ResultConfigurations;

DELETE FROM Championships;

INSERT INTO Championships (LeagueId, ChampionshipId, Name, DisplayName, Version)
	SELECT LeagueId, ChampionshipId, Name, DisplayName, Version
	FROM TmpChampionships;

SELECT * FROM Championships;

CREATE TEMPORARY TABLE TmpChampSeasons (
	LeagueId BIGINT NOT NULL,
	ChampSeasonId BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
	ChampionshipId BIGINT NOT NULL,
	SeasonId BIGINT NOT NULL,
	StandingConfigId BIGINT
);

INSERT INTO TmpChampSeasons (LeagueId, ChampionshipId, SeasonId, StandingConfigId)
	SELECT c.LeagueId, c.ChampionshipId, sch.SeasonId, s.StandingConfigId
	FROM ScoredEventResults AS er
	JOIN Events AS ev
	ON er.EventId=ev.EventId
	JOIN Schedules AS sch
	ON ev.ScheduleId=sch.ScheduleId
	JOIN ResultConfigurations AS r
	ON er.ResultConfigId=r.ResultConfigId
	JOIN TmpChampionships AS c
	ON r.ResultConfigId=c.ResultConfigId
	JOIN StandingConfigs_ResultConfigs AS s_r
	ON r.ResultConfigId=s_r.ResultConfigId
	JOIN StandingConfigurations AS s
	ON s_r.StandingConfigId=s.StandingConfigId
	GROUP BY c.ChampionshipId, sch.SeasonId, s.StandingConfigId;

INSERT INTO ChampSeasons
	SELECT * FROM TmpChampSeasons;

SELECT * FROM ChampSeasons;

INSERT INTO ChampSeasons_ResultConfigs
	SELECT cs.LeagueId, cs.ChampSeasonId, c.ResultConfigId
	FROM TmpChampSeasons AS cs
	JOIN TmpChampionships AS c
	ON cs.ChampionshipId=c.ChampionshipId;

SELECT * FROM ChampSeasons_ResultConfigs;

UPDATE ScoredEventResults AS er
	JOIN Events AS ev
		ON er.EventId=ev.EventId
	JOIN Schedules AS sch
		ON ev.ScheduleId=sch.ScheduleId
	JOIN ChampSeasons_ResultConfigs as cs_rc
		ON er.ResultConfigId=cs_rc.ResultConfigId
	JOIN ChampSeasons cs
		ON cs_rc.ChampSeasonId=cs.ChampSeasonId 
		AND sch.SeasonId=cs.SeasonId
	SET er.ChampSeasonId=cs.ChampSeasonId;

SELECT LeagueId, EventId, ResultConfigId, ChampSeasonId
	FROM ScoredEventResults;

UPDATE Standings AS s
	JOIN ChampSeasons AS cs
		ON s.StandingConfigId=cs.StandingConfigId
	SET s.ChampSeasonId=cs.ChampSeasonId;

SELECT LeagueId, StandingConfigId, ChampSeasonId FROM Standings;

DROP TABLE TmpChampionships;
DROP TABLE TmpChampSeasons;

ROLLBACK;