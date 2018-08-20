INSERT INTO Contexts VALUES("2ad57821-dad5-4e0a-abb4-47d99b314f21", "Home");
INSERT INTO Contexts VALUES("be17f3e2-764b-43b5-b943-63faf6223863", "Office");
INSERT INTO Contexts VALUES("f89d513b-c24d-468e-99f3-b841e5ceca6f", "Computer");
INSERT INTO Contexts VALUES("ae7491da-4a83-4cc6-ad26-cd090e81417b", "Errands");
INSERT INTO Contexts VALUES("c50d02de-d22c-475b-9fef-6e24c05f966b", "Phone");

--strftime('%Y-%m-%d %H:%M:%S', ...) 
--strftime('%Y-%m-%d %H:%M:%f', ...) 
-- INSERT INTO Projects VALUES(hex(randomblob(16)), "Test Project 1", DATE("2018-09-22"), strftime('%Y-%m-%d %H:%M:%S', "NOW"));

   --ID VARCHAR(36) PRIMARY KEY,
   --Name VARCHAR(100),
   --Status VARCHAR(30),
   --ProjectID VARCHAR(36),
   --DateStarted VARCHAR(30),
   --DateTimeCompleted VARCHAR(30),
   --DateDue VARCHAR(30),
   --ContextID VARCHAR(36),
   --DateTimeCreated VARCHAR(30),
