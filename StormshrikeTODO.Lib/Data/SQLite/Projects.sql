DROP TABLE IF EXISTS Projects;

CREATE TABLE Projects (
   ID VARCHAR(36),
   Name VARCHAR(100),
   DateDue VARCHAR(30),
   DateTimeCreated VARCHAR(30)
   );

CREATE UNIQUE INDEX idx_Projects ON Projects (ID);
