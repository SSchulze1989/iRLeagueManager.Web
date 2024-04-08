/* Migrate existing PenaltyPoints of ReviewPenaltys table to new JsonColum values
 * AddPenaltys has been dropped in the previous migration so no values need to be converted here
 */

USE TestDatabase;

START TRANSACTION;

-- Pre migration
-- Copy data to temp table before dropping
CREATE TEMPORARY TABLE TmpReviewPenaltys(
	LeagueId BIGINT NOT NULL,
	ResultRowId BIGINT NOT NULL,
	ReviewId BIGINT NOT NULL,
	ReviewVoteId BIGINT NOT NULL,
	`Value` JSON NULL,
	PRIMARY KEY (LeagueId, ResultRowId, ReviewId, ReviewVoteId)
);

INSERT INTO TmpReviewPenaltys
	SELECT * FROM ReviewPenaltys;

-- drop table and create new with corrct primary key
DROP TABLE ReviewPenaltys;
CREATE TABLE ReviewPenaltys(
	LeagueId BIGINT NOT NULL,
	ResultRowId BIGINT NOT NULL,
	ReviewId BIGINT NOT NULL,
	ReviewVoteId BIGINT NOT NULL,
	`Value` JSON NULL,
	PRIMARY KEY (LeagueId, ResultRowId, ReviewId, ReviewVoteId)
);
-- Handle creation of indexes and fk inside ef migration

-- Post migration
-- Copy data into newly created table after creation of indexes and fk
INSERT INTO ReviewPenaltys 
	SELECT * FROM TmpReviewPenaltys

DROP TABLE TmpReviewPenaltys;

ROLLBACK;