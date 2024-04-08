USE LeagueDatabase;

START TRANSACTION;

-- pre migration
-- safe the CarNumber values as ints

CREATE TEMPORARY TABLE TmpResultRowsNumbers (
	ResultRowId BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
	CarNumber INT NOT NULL
);

INSERT INTO TmpResultRowsNumbers (ResultRowId, CarNumber)
	SELECT ResultRowId, CarNumber
	FROM ResultRows;

CREATE TEMPORARY TABLE TmpScoredResultRowsNumbers (
	ScoredResultRowId BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
	CarNumber INT NOT NULL
);

INSERT INTO	TmpScoredResultRowsNumbers (ScoredResultRowId, CarNumber)
	SELECT ScoredResultRowId, CarNumber
	FROM ScoredResultRows;

-- post migration
-- restore CarNumber values as string

UPDATE ResultRows `rows` 
	JOIN TmpResultRowsNumbers `numbers` 
		ON `numbers`.ResultRowId=`rows`.ResultRowId
	SET `rows`.CarNumber = `numbers`.CarNumber;

UPDATE ScoredResultRows `rows` 
	JOIN TmpScoredResultRowsNumbers `numbers` 
		ON `numbers`.ScoredResultRowId=`rows`.ScoredResultRowId
	SET `rows`.CarNumber = `numbers`.CarNumber;

DROP TABLE TmpResultRowsNumbers;
DROP TABLE TmpScoredResultRowsNumbers;

ROLLBACK;