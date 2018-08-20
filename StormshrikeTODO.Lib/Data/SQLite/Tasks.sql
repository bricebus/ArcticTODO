DROP TABLE IF EXISTS Tasks;

CREATE TABLE Tasks (
   ID VARCHAR(36) PRIMARY KEY,
   Name VARCHAR(100),
   Status VARCHAR(30),
   ProjectID VARCHAR(36),
   DateStarted VARCHAR(30),
   DateTimeCompleted VARCHAR(30),
   DateDue VARCHAR(30),
   ContextID VARCHAR(36),
   DateTimeCreated VARCHAR(30),

   FOREIGN KEY(ProjectID) REFERENCES Projects(ID),
   FOREIGN KEY(ContextID) REFERENCES Contexts(ID)
   );

CREATE UNIQUE INDEX idx_Tasks ON Tasks (ID);
