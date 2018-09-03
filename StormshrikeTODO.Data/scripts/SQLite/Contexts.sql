DROP TABLE IF EXISTS Contexts;

CREATE TABLE Contexts (
   ID VARCHAR(36) PRIMARY KEY,
   Description VARCHAR(100)
   );

CREATE UNIQUE INDEX idx_Contexts ON Contexts (ID);