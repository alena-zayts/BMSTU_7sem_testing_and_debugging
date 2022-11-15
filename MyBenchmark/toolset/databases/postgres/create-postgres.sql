BEGIN;

CREATE EXTENSION IF NOT EXISTS pg_stat_statements;

CREATE TABLE  World (
  id integer NOT NULL,
  randomNumber integer NOT NULL default 0,
  PRIMARY KEY  (id)
);
GRANT ALL PRIVILEGES ON World to benchmarkdbuser;

INSERT INTO World (id, randomnumber)
SELECT x.id, least(floor(random() * 10000 + 1), 10000) FROM generate_series(1,10000) as x(id);


CREATE TABLE  "World" (
  id integer NOT NULL,
  randomNumber integer NOT NULL default 0,
  PRIMARY KEY  (id)
);
GRANT ALL PRIVILEGES ON "World" to benchmarkdbuser;

INSERT INTO "World" (id, randomnumber)
SELECT x.id, least(floor(random() * 10000 + 1), 10000) FROM generate_series(1,10000) as x(id);

COMMIT;

