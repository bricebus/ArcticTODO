using Microsoft.VisualStudio.TestTools.UnitTesting;
using StormshrikeTODO.Model;
using StormshrikeTODO.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace StormshrikeTODO.Tests.Persistence
{
    [TestClass]
    public class SQLItePersistenceTest
    {
        private static int RunPowershellScript(string ps, string args)
        {
            int errorLevel;
            ProcessStartInfo processInfo;
            Process process;


            if (!System.IO.File.Exists(ps))
            {
                throw new ArgumentException(ps + " file not found!");
            }

            string ps1arg = "-File " + ps + " " + args;

            processInfo = new ProcessStartInfo("powershell.exe", ps1arg)
            {
                CreateNoWindow = false,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            
            using (process = Process.Start(processInfo))
            {
                //
                // Read in all the text from the process with the StreamReader.
                //
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.Write(result);
                }

                string stdErr = "";
                using (StreamReader reader = process.StandardError)
                {
                    stdErr = reader.ReadToEnd();
                }
                if (!String.IsNullOrEmpty(stdErr))
                {
                    throw new Exception("Error running " + ps + ":" + stdErr);
                }

                process.WaitForExit();
                errorLevel = process.ExitCode;
                process.Close();
            }

            return errorLevel;
        }

        //[ClassInitialize]
        //public static void Initialize(TestContext testContext)
        [TestInitialize]
        public void Initialize()
        {
            CreateTestDB(GetTestDBLocation());
            LoadTestDataIntoDB(GetTestDBLocation());
        }

        private int GetThreadID()
        {
            return Thread.CurrentThread.ManagedThreadId;
        }

        private string GetTestDBLocation()
        {
            return System.Environment.GetEnvironmentVariable("TEMP")
                + "\\StormshrikeTest-"
                + System.Diagnostics.Process.GetCurrentProcess().Id
                + "-"
                + Thread.CurrentThread.ManagedThreadId
                + ".db";

        }

        private static void CreateTestDB(string testDbLocation)
        {
            string origCwd = System.IO.Directory.GetCurrentDirectory();

            System.IO.Directory.SetCurrentDirectory("..\\..\\..\\StormshrikeTODO.Data\\bin\\Debug\\scripts\\SQLite");
            RunPowershellScript("CreateDB.ps1", "-dblocation \"" + testDbLocation + "\"");
            System.IO.Directory.SetCurrentDirectory(origCwd);
            Assert.IsTrue(System.IO.File.Exists(testDbLocation), "Cannot find DB file!");
        }

        private static void LoadTestDataIntoDB(string testDbLocation)
        {
            string origCwd = System.IO.Directory.GetCurrentDirectory();

            System.IO.Directory.SetCurrentDirectory("Data\\SQLite");
            RunPowershellScript("LoadTestDB.ps1", "-dblocation \"" + testDbLocation + "\"");
            System.IO.Directory.SetCurrentDirectory(origCwd);
        }

        //[ClassCleanup]
        [TestCleanup]
        public void Cleanup()
        {
            System.IO.File.Delete(GetTestDBLocation());
        }

        [TestMethod]
        public void TestLoadContexts()
        {
            using (SQLitePersistence sqlite = new SQLitePersistence(new SQLitePersistenceConfig(GetTestDBLocation())))
            {
                DefinedContexts dc = sqlite.LoadContexts();
                Assert.AreEqual(6, dc.Count);
                Assert.AreEqual("2ad57821-dad5-4e0a-abb4-47d99b314f21", dc.FindIdByDescr("Home").ID);
                Assert.AreEqual("be17f3e2-764b-43b5-b943-63faf6223863", dc.FindIdByDescr("Office").ID);
                Assert.AreEqual("f89d513b-c24d-468e-99f3-b841e5ceca6f", dc.FindIdByDescr("Computer").ID);
                Assert.AreEqual("ae7491da-4a83-4cc6-ad26-cd090e81417b", dc.FindIdByDescr("Errands").ID);
                Assert.AreEqual("c50d02de-d22c-475b-9fef-6e24c05f966b", dc.FindIdByDescr("Phone").ID);
                Assert.AreEqual("1b9c460b-0287-4011-87fc-0ae0c5f7b81c", dc.FindIdByDescr("Manager").ID);
            }

        }

        [TestMethod]
        public void TestSaveContexts_NoChanges()
        {
            DefinedContexts dc = null;
            DefinedContexts dcNew = null;
            using (SQLitePersistence sqlite = new SQLitePersistence(new SQLitePersistenceConfig(GetTestDBLocation())))
            {
                dc = sqlite.LoadContexts();
            }

            using (SQLitePersistence sqlite = new SQLitePersistence(new SQLitePersistenceConfig(GetTestDBLocation())))
            {
                sqlite.SaveContexts(dc);
            }

            using (SQLitePersistence sqlite = new SQLitePersistence(new SQLitePersistenceConfig(GetTestDBLocation())))
            {
                dcNew = sqlite.LoadContexts();
            }

            Assert.IsFalse(DefinedContexts.AreDifferences(dc, dcNew));
        }

        [TestMethod]
        public void TestSaveContexts_Change()
        {
            DefinedContexts dc = null;
            DefinedContexts dcNew = null;
            using (SQLitePersistence sqlite = new SQLitePersistence(new SQLitePersistenceConfig(GetTestDBLocation())))
            {
                dc = sqlite.LoadContexts();
            }

            Context ctx = dc.FindIdByID("2ad57821-dad5-4e0a-abb4-47d99b314f21");
            ctx.Description = "Home Office";

            using (SQLitePersistence sqlite = new SQLitePersistence(new SQLitePersistenceConfig(GetTestDBLocation())))
            {
                sqlite.SaveContexts(dc);
            }

            using (SQLitePersistence sqlite = new SQLitePersistence(new SQLitePersistenceConfig(GetTestDBLocation())))
            {
                dcNew = sqlite.LoadContexts();
                Assert.AreEqual(6, dcNew.Count);
                Assert.AreEqual("2ad57821-dad5-4e0a-abb4-47d99b314f21", dcNew.FindIdByDescr("Home Office").ID);
                Assert.AreEqual("be17f3e2-764b-43b5-b943-63faf6223863", dcNew.FindIdByDescr("Office").ID);
                Assert.AreEqual("f89d513b-c24d-468e-99f3-b841e5ceca6f", dcNew.FindIdByDescr("Computer").ID);
                Assert.AreEqual("ae7491da-4a83-4cc6-ad26-cd090e81417b", dcNew.FindIdByDescr("Errands").ID);
                Assert.AreEqual("c50d02de-d22c-475b-9fef-6e24c05f966b", dcNew.FindIdByDescr("Phone").ID);
                Assert.AreEqual("1b9c460b-0287-4011-87fc-0ae0c5f7b81c", dcNew.FindIdByDescr("Manager").ID);
            }
            Assert.IsFalse(DefinedContexts.AreDifferences(dc, dcNew));
        }

        [TestMethod]
        public void TestSaveContexts_Deleted()
        {
            DefinedContexts dc = null;
            DefinedContexts dcNew = null;
            using (SQLitePersistence sqlite = new SQLitePersistence(new SQLitePersistenceConfig(GetTestDBLocation())))
            {
                dc = sqlite.LoadContexts();
            }

            dc.Remove("1b9c460b-0287-4011-87fc-0ae0c5f7b81c");

            using (SQLitePersistence sqlite = new SQLitePersistence(new SQLitePersistenceConfig(GetTestDBLocation())))
            {
                sqlite.SaveContexts(dc);
            }

            using (SQLitePersistence sqlite = new SQLitePersistence(new SQLitePersistenceConfig(GetTestDBLocation())))
            {
                dcNew = sqlite.LoadContexts();
                Assert.AreEqual(5, dcNew.Count);
                Assert.AreEqual("2ad57821-dad5-4e0a-abb4-47d99b314f21", dc.FindIdByDescr("Home").ID);
                Assert.AreEqual("be17f3e2-764b-43b5-b943-63faf6223863", dcNew.FindIdByDescr("Office").ID);
                Assert.AreEqual("f89d513b-c24d-468e-99f3-b841e5ceca6f", dcNew.FindIdByDescr("Computer").ID);
                Assert.AreEqual("ae7491da-4a83-4cc6-ad26-cd090e81417b", dcNew.FindIdByDescr("Errands").ID);
                Assert.AreEqual("c50d02de-d22c-475b-9fef-6e24c05f966b", dcNew.FindIdByDescr("Phone").ID);
            }
            Assert.IsFalse(DefinedContexts.AreDifferences(dc, dcNew));
        }

        [TestMethod]
        public void TestSaveContexts_New()
        {
            DefinedContexts dc = null;
            DefinedContexts dcNew = null;
            using (SQLitePersistence sqlite = new SQLitePersistence(new SQLitePersistenceConfig(GetTestDBLocation())))
            {
                dc = sqlite.LoadContexts();
            }

            dc.Add(new Context("b19bce30-96fe-46dc-b408-891a12a3a933", "Weekly Team Meeting"));

            using (SQLitePersistence sqlite = new SQLitePersistence(new SQLitePersistenceConfig(GetTestDBLocation())))
            {
                sqlite.SaveContexts(dc);
            }

            using (SQLitePersistence sqlite = new SQLitePersistence(new SQLitePersistenceConfig(GetTestDBLocation())))
            {
                dcNew = sqlite.LoadContexts();
                Assert.AreEqual(7, dcNew.Count);
                Assert.AreEqual("2ad57821-dad5-4e0a-abb4-47d99b314f21", dcNew.FindIdByDescr("Home").ID);
                Assert.AreEqual("be17f3e2-764b-43b5-b943-63faf6223863", dcNew.FindIdByDescr("Office").ID);
                Assert.AreEqual("f89d513b-c24d-468e-99f3-b841e5ceca6f", dcNew.FindIdByDescr("Computer").ID);
                Assert.AreEqual("ae7491da-4a83-4cc6-ad26-cd090e81417b", dcNew.FindIdByDescr("Errands").ID);
                Assert.AreEqual("c50d02de-d22c-475b-9fef-6e24c05f966b", dcNew.FindIdByDescr("Phone").ID);
                Assert.AreEqual("1b9c460b-0287-4011-87fc-0ae0c5f7b81c", dcNew.FindIdByDescr("Manager").ID);
                Assert.AreEqual("b19bce30-96fe-46dc-b408-891a12a3a933", dcNew.FindIdByDescr("Weekly Team Meeting").ID);
            }
            Assert.IsFalse(DefinedContexts.AreDifferences(dc, dcNew));
        }

        [TestMethod]
        public void TestSaveProject_ChangedProject()
        {
            using (SQLitePersistence sqlite = new SQLitePersistence(new SQLitePersistenceConfig(GetTestDBLocation())))
            {
                Collection<Project> prjList = sqlite.LoadProjects();

                var changedID = prjList[0].UniqueID;
                prjList[0].ProjectName = "Changed Name";

                sqlite.SaveProjects(prjList);

                Collection<Project> prjListNew = sqlite.LoadProjects();

                Assert.IsTrue(ProjectComparer.AreListsEquivalent(prjList, prjListNew,
                    out List<Project> chgList, out List<Project> addList, out List<Project> delList,
                    out List<Project> chgTaskList));
            }
        }

        [TestMethod]
        public void TestSaveProject_DeleteProject()
        {
            using (SQLitePersistence sqlite = new SQLitePersistence(new SQLitePersistenceConfig(GetTestDBLocation())))
            {
                Collection<Project> prjList = sqlite.LoadProjects();

                var removedProjectID = prjList[0].UniqueID;
                prjList.RemoveAt(0);

                sqlite.SaveProjects(prjList);

                Collection<Project> prjListNew = sqlite.LoadProjects();

                Assert.IsTrue(ProjectComparer.AreListsEquivalent(prjList, prjListNew,
                    out List<Project> chgList, out List<Project> addList, out List<Project> delList,
                    out List<Project> chgTaskList));

                // test that tasks tied to the removed project have been deleted from the DB
                Assert.AreEqual(0, sqlite.LoadTasks(removedProjectID.ToString()).Count);
            }
        }

        [TestMethod]
        public void TestSaveProject_AddTaskToExistingProject()
        {
            using (SQLitePersistence sqlite = new SQLitePersistence(new SQLitePersistenceConfig(GetTestDBLocation())))
            {
                Collection<Project> prjList = sqlite.LoadProjects();
                DefinedContexts dc = sqlite.LoadContexts();

                var prjIDWithNewTask = prjList[0].UniqueID;
                var newTask = new Task("New Task")
                {
                    ContextID = dc.FindIdByDescr("Home").ID
                };
                prjList[0].AddTask(newTask);

                sqlite.SaveProjects(prjList);

                Collection<Project> prjListNew = sqlite.LoadProjects();

                Assert.IsTrue(ProjectComparer.AreListsEquivalent(prjList, prjListNew,
                    out List<Project> chgList, out List<Project> addList, out List<Project> delList,
                    out List<Project> chgTaskList));
                Assert.IsNotNull(prjListNew[0].TaskList.Where(t => t.UniqueID == newTask.UniqueID).First());
            }
        }

        [TestMethod]
        public void TestSaveProject_ChangeExistingTask()
        {
            using (SQLitePersistence sqlite = new SQLitePersistence(new SQLitePersistenceConfig(GetTestDBLocation())))
            {
                Collection<Project> prjList = sqlite.LoadProjects();
                DefinedContexts dc = sqlite.LoadContexts();

                var changedTask = prjList[2].TaskList[4].UniqueID;
                prjList[2].TaskList[4].Details = "New details";

                sqlite.SaveProjects(prjList);

                Collection<Project> prjListNew = sqlite.LoadProjects();

                Assert.IsTrue(ProjectComparer.AreListsEquivalent(prjList, prjListNew,
                    out List<Project> chgList, out List<Project> addList, out List<Project> delList,
                    out List<Project> chgTaskList));
                Assert.AreEqual("New details", prjListNew[2].TaskList.Where(t => t.UniqueID == changedTask).First().Details);
            }
        }

        [TestMethod]
        public void TestSaveProject_DeleteExistingTask()
        {
            using (SQLitePersistence sqlite = new SQLitePersistence(new SQLitePersistenceConfig(GetTestDBLocation())))
            {
                Collection<Project> prjList = sqlite.LoadProjects();
                DefinedContexts dc = sqlite.LoadContexts();

                var deletedTask = prjList[3].TaskList[1].UniqueID;
                prjList[3].TaskList.RemoveAt(1);

                sqlite.SaveProjects(prjList);

                Collection<Project> prjListNew = sqlite.LoadProjects();

                Assert.IsTrue(ProjectComparer.AreListsEquivalent(prjList, prjListNew,
                    out List<Project> chgList, out List<Project> addList, out List<Project> delList,
                    out List<Project> chgTaskList));
                Assert.AreEqual(0, prjListNew[3].TaskList.Where(t => t.UniqueID == deletedTask).Count());
            }
        }

        [TestMethod]
        public void TestSaveProject_AddProject_NoTasks()
        {
            using (SQLitePersistence sqlite = new SQLitePersistence(new SQLitePersistenceConfig(GetTestDBLocation())))
            {
                Collection<Project> prjList = sqlite.LoadProjects();

                var removedProjectID = prjList[0].UniqueID;
                var newProject = new Project("New Test Project");
                prjList.Add(newProject);

                sqlite.SaveProjects(prjList);

                Collection<Project> prjListNew = sqlite.LoadProjects();

                Assert.IsTrue(ProjectComparer.AreListsEquivalent(prjList, prjListNew,
                    out List<Project> chgList, out List<Project> addList, out List<Project> delList,
                    out List<Project> chgTaskList));
            }
        }

        [TestMethod]
        public void TestSaveProject_AddProject_WithTasks()
        {
            using (SQLitePersistence sqlite = new SQLitePersistence(new SQLitePersistenceConfig(GetTestDBLocation())))
            {
                Collection<Project> prjList = sqlite.LoadProjects();
                DefinedContexts dc = sqlite.LoadContexts();

                var removedProjectID = prjList[0].UniqueID;
                var newProject = new Project("New Test Project");
                var newTask = new Task("New Task")
                {
                    ContextID = dc.FindIdByDescr("Home").ID
                };
                newProject.AddTask(newTask);
                prjList.Add(newProject);

                sqlite.SaveProjects(prjList);

                Collection<Project> prjListNew = sqlite.LoadProjects();

                Assert.IsTrue(ProjectComparer.AreListsEquivalent(prjList, prjListNew,
                    out List<Project> chgList, out List<Project> addList, out List<Project> delList,
                    out List<Project> chgTaskList));
                Assert.IsTrue(prjListNew[10].TaskList.Select(t => t.UniqueID == newTask.UniqueID).First());
            }
        }

        [TestMethod]
        public void TestLoadProjects()
        {
            using (SQLitePersistence sqlite = new SQLitePersistence(new SQLitePersistenceConfig(GetTestDBLocation())))
            {
                Collection<Project> prjList = sqlite.LoadProjects();
                Assert.AreEqual(10, prjList.Count);
                foreach (var prj in prjList)
                {
                    Assert.AreEqual(5, prj.TaskCount);
                }

                Project prj1 = prjList.First(p => p.ProjectName == "Test Project  1");
                Task t1_1 = prj1.GetTask("49f184a0-d6bb-44b2-a6e4-cb49559d494e");
                Task t1_2 = prj1.GetTask("d34c00e4-bdcb-42a5-88e9-4b8961a543f0");
                Task t1_3 = prj1.GetTask("871ba660-c08c-4bf0-9cd5-650c51a968f0");
                Task t1_4 = prj1.GetTask("fcbfc3e8-88df-48f5-ae89-80a9ca159113");
                Task t1_5 = prj1.GetTask("6d1ac5a8-1e68-4de9-901c-ee426d8ec681");

                Assert.AreEqual("Test Task  1.1", t1_1.Name);
                Assert.AreEqual("Detail 1.1", t1_1.Details);
                Assert.AreEqual(1000, t1_1.Order);
                Assert.AreEqual(Model.Task.StatusEnum.New, t1_1.Status);
                Assert.AreEqual(DateTime.Parse("2018-01-01 01:30:54"), t1_1.DateTimeCreated);
                Assert.AreEqual(null, t1_1.DateDue);
                Assert.AreEqual(null, t1_1.DateCompleted);
                Assert.AreEqual(null, t1_1.DateStarted);

                Assert.AreEqual("Test Task  1.2", t1_2.Name);
                Assert.AreEqual("", t1_2.Details);
                Assert.AreEqual(2000, t1_2.Order);
                Assert.AreEqual(Model.Task.StatusEnum.Done, t1_2.Status);
                Assert.AreEqual(DateTime.Parse("2018-01-02 02:30:54"), t1_2.DateTimeCreated);
                Assert.IsTrue(DateTime.Now >= t1_2.DateTimeCreated);
                Assert.AreEqual(null, t1_2.DateDue);
                Assert.AreEqual(DateTime.Parse("2018-01-02"), t1_2.DateCompleted);
                Assert.AreEqual(null, t1_2.DateStarted);

                Assert.AreEqual("Test Task  1.3", t1_3.Name);
                Assert.AreEqual("", t1_3.Details);
                Assert.AreEqual(3000, t1_3.Order);
                Assert.AreEqual(Model.Task.StatusEnum.InProgress, t1_3.Status);
                Assert.AreEqual(DateTime.Parse("2018-01-03 03:30:54"), t1_3.DateTimeCreated);
                Assert.AreEqual(null, t1_3.DateStarted);
                Assert.AreEqual(null, t1_3.DateCompleted);
                Assert.AreEqual(DateTime.Parse("2018-01-03"), t1_3.DateDue);

                Assert.AreEqual("Test Task  1.4", t1_4.Name);
                Assert.AreEqual("", t1_4.Details);
                Assert.AreEqual(4000, t1_4.Order);
                Assert.AreEqual(Model.Task.StatusEnum.Waiting, t1_4.Status);
                Assert.AreEqual(DateTime.Parse("2018-01-04 04:30:54"), t1_4.DateTimeCreated);
                Assert.AreEqual(null, t1_4.DateDue);
                Assert.AreEqual(null, t1_4.DateCompleted);
                Assert.AreEqual(DateTime.Parse("2018-01-04"), t1_4.DateStarted);

                Assert.AreEqual("Test Task  1.5", t1_5.Name);
                Assert.AreEqual("", t1_5.Details);
                Assert.AreEqual(5000, t1_5.Order);
                Assert.AreEqual(Model.Task.StatusEnum.NotStarted, t1_5.Status);
                Assert.AreEqual(DateTime.Parse("2018-01-05 05:30:54"), t1_5.DateTimeCreated);
                Assert.AreEqual(null, t1_5.DateDue);
                Assert.AreEqual(null, t1_5.DateCompleted);
                Assert.AreEqual(null, t1_5.DateStarted);

                Project prj2 = prjList.First(p => p.ProjectName == "Test Project  2");
                Task t2_1 = prj2.GetTask("19f6e9eb-1cf6-4416-81b1-e4b42e9d6dfe");
                Task t2_2 = prj2.GetTask("56138652-f14a-4a26-bda6-c43976adb414");
                Task t2_3 = prj2.GetTask("79962192-ad3d-44fc-bf15-cf9de5926dde");
                Task t2_4 = prj2.GetTask("bd69d3bf-7dbe-4b88-88ca-d44d96b025f8");
                Task t2_5 = prj2.GetTask("b76551c7-45f5-4712-a697-9dc0f93829ca");

                Assert.AreEqual("Test Task  2.1", t2_1.Name);
                Assert.AreEqual("Test Task  2.2", t2_2.Name);
                Assert.AreEqual("Test Task  2.3", t2_3.Name);
                Assert.AreEqual("Test Task  2.4", t2_4.Name);
                Assert.AreEqual("Test Task  2.5", t2_5.Name);

                Project prj3 = prjList.First(p => p.ProjectName == "Test Project  3");
                Task t3_1 = prj3.GetTask("c61e4925-294c-4a68-81a3-aedd4c7754ac");
                Task t3_2 = prj3.GetTask("9d3f2cee-84aa-4deb-b8e1-c5b88792f7e8");
                Task t3_3 = prj3.GetTask("3c69e3d5-8883-4fe6-82e3-a19160db20a1");
                Task t3_4 = prj3.GetTask("c98839f0-9ea5-4103-a72d-40f0afa6a114");
                Task t3_5 = prj3.GetTask("6cb81a5a-439e-4b08-83a9-e4df143573f1");


                Assert.AreEqual("Test Task  3.1", t3_1.Name);
                Assert.AreEqual("Test Task  3.2", t3_2.Name);
                Assert.AreEqual("Test Task  3.3", t3_3.Name);
                Assert.AreEqual("Test Task  3.4", t3_4.Name);
                Assert.AreEqual("Test Task  3.5", t3_5.Name);

                Project prj4 = prjList.First(p => p.ProjectName == "Test Project  4");
                Task t4_1 = prj4.GetTask("e95f56cb-5f37-4d87-af3d-a3267924c9aa");
                Task t4_2 = prj4.GetTask("a73b0a06-a0b3-4f7f-827c-0a8c158565d4");
                Task t4_3 = prj4.GetTask("d30a9ff9-fd68-41c8-9126-51c56c1f7e85");
                Task t4_4 = prj4.GetTask("535a323a-3cc4-45f2-a0ad-a89debace4a2");
                Task t4_5 = prj4.GetTask("6b1e3072-47de-4fc5-9579-4f07a2fa144c");

                Assert.AreEqual("Test Task  4.1", t4_1.Name);
                Assert.AreEqual("Test Task  4.2", t4_2.Name);
                Assert.AreEqual("Test Task  4.3", t4_3.Name);
                Assert.AreEqual("Test Task  4.4", t4_4.Name);
                Assert.AreEqual("Test Task  4.5", t4_5.Name);

                Project prj5 = prjList.First(p => p.ProjectName == "Test Project  5");
                Task t5_1 = prj5.GetTask("e51921e6-1a20-407b-aca3-f0f50af176c8");
                Task t5_2 = prj5.GetTask("e3edd351-f76d-465f-82e5-b44a772d72be");
                Task t5_3 = prj5.GetTask("3a1dc098-c85c-49c9-bf76-48caf8bd7f3a");
                Task t5_4 = prj5.GetTask("a2be4560-65e2-42e7-a930-04af97866c4f");
                Task t5_5 = prj5.GetTask("a08c62ca-8036-4723-9b4d-2e2b615f9fef");

                Assert.AreEqual("Test Task  5.1", t5_1.Name);
                Assert.AreEqual("Test Task  5.2", t5_2.Name);
                Assert.AreEqual("Test Task  5.3", t5_3.Name);
                Assert.AreEqual("Test Task  5.4", t5_4.Name);
                Assert.AreEqual("Test Task  5.5", t5_5.Name);

                Project prj6 = prjList.First(p => p.ProjectName == "Test Project  6");
                Task t6_1 = prj6.GetTask("a6dc6a7d-5744-4767-8f27-4dce529975f8");
                Task t6_2 = prj6.GetTask("7c13673d-422e-411a-ac1d-2c3a166c1683");
                Task t6_3 = prj6.GetTask("0a8935f4-2a80-40ff-ab1b-90a3bfa33d49");
                Task t6_4 = prj6.GetTask("fb9cd0b5-4b50-4b52-9b81-ee2a577629e3");
                Task t6_5 = prj6.GetTask("4ed89d52-47ca-4fd0-a7f2-df7a2c8c75cf");

                Assert.AreEqual("Test Task  6.1", t6_1.Name);
                Assert.AreEqual("Test Task  6.2", t6_2.Name);
                Assert.AreEqual("Test Task  6.3", t6_3.Name);
                Assert.AreEqual("Test Task  6.4", t6_4.Name);
                Assert.AreEqual("Test Task  6.5", t6_5.Name);

                Project prj7 = prjList.First(p => p.ProjectName == "Test Project  7");
                Task t7_1 = prj7.GetTask("0fcbae3c-ac37-4ee8-95ef-10d190b17e2c");
                Task t7_2 = prj7.GetTask("a4efcf3c-a920-4b6b-869f-1dc2340f4f1b");
                Task t7_3 = prj7.GetTask("095c6c3b-4e4d-407e-9580-e7dd1c972a54");
                Task t7_4 = prj7.GetTask("002d22b2-e42b-4b39-89c7-3d002783cc2e");
                Task t7_5 = prj7.GetTask("4f5c8442-21a6-4235-b3a0-6bef72642e69");

                Assert.AreEqual("Test Task  7.1", t7_1.Name);
                Assert.AreEqual("Test Task  7.2", t7_2.Name);
                Assert.AreEqual("Test Task  7.3", t7_3.Name);
                Assert.AreEqual("Test Task  7.4", t7_4.Name);
                Assert.AreEqual("Test Task  7.5", t7_5.Name);

                Project prj8 = prjList.First(p => p.ProjectName == "Test Project  8");
                Task t8_1 = prj8.GetTask("02fadb5d-e9d5-4726-b458-c4e15ca98633");
                Task t8_2 = prj8.GetTask("b771aaa9-81b3-4e68-a7c9-a00c6135331f");
                Task t8_3 = prj8.GetTask("366e78a9-c8fa-41f8-8660-4019c98614b1");
                Task t8_4 = prj8.GetTask("3292b766-7206-4ff2-86dc-5957587975ec");
                Task t8_5 = prj8.GetTask("1e676e6c-8ff7-462f-9500-e2cd2d613e94");

                Assert.AreEqual("Test Task  8.1", t8_1.Name);
                Assert.AreEqual("Test Task  8.2", t8_2.Name);
                Assert.AreEqual("Test Task  8.3", t8_3.Name);
                Assert.AreEqual("Test Task  8.4", t8_4.Name);
                Assert.AreEqual("Test Task  8.5", t8_5.Name);

                Project prj9 = prjList.First(p => p.ProjectName == "Test Project  9");
                Task t9_1 = prj9.GetTask("f76bec0c-ac6d-4c5d-ba7a-64102867a307");
                Task t9_2 = prj9.GetTask("d84f4b07-3ad6-44dd-8696-a770a044d660");
                Task t9_3 = prj9.GetTask("04effe3e-d355-4094-b604-37ab8641307e");
                Task t9_4 = prj9.GetTask("45c96282-7d5d-43dc-99aa-0dda9d550c38");
                Task t9_5 = prj9.GetTask("d968c831-5def-433e-95f7-38965f50e1dd");

                Assert.AreEqual("Test Task  9.1", t9_1.Name);
                Assert.AreEqual("Test Task  9.2", t9_2.Name);
                Assert.AreEqual("Test Task  9.3", t9_3.Name);
                Assert.AreEqual("Test Task  9.4", t9_4.Name);
                Assert.AreEqual("Test Task  9.5", t9_5.Name);

                Project prj10 = prjList.First(p => p.ProjectName == "Test Project 10");
                Task t10_1 = prj10.GetTask("0c6be24d-6516-41d6-9022-f50ddd1d7293");
                Task t10_2 = prj10.GetTask("a5f571ff-3641-444d-8c8c-465e3b2081fe");
                Task t10_3 = prj10.GetTask("68a7e284-cfb6-4435-8c0f-e8c68e437225");
                Task t10_4 = prj10.GetTask("06b9a45c-e628-4bd5-ad30-2bca2a8b9ed9");
                Task t10_5 = prj10.GetTask("2585ee54-2202-441f-9112-3ba9a8c7a3f9");

                Assert.AreEqual("Test Task 10.1", t10_1.Name);
                Assert.AreEqual("Test Task 10.2", t10_2.Name);
                Assert.AreEqual("Test Task 10.3", t10_3.Name);
                Assert.AreEqual("Test Task 10.4", t10_4.Name);
                Assert.AreEqual("Test Task 10.5", t10_5.Name);
            }
        }
    }
}
