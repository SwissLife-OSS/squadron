CREATE TABLE features (
  id serial PRIMARY KEY,
  name VARCHAR(100) NOT NULL
);

INSERT INTO features ( name )
VALUES
  ('Redis');
