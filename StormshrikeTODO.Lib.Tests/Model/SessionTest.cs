using Microsoft.VisualStudio.TestTools.UnitTesting;
using StormshrikeTODO.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using StormshrikeTODO.Persistence;
using System.Collections.ObjectModel;
using StormshrikeTODO.Model.Tests;

namespace StormshrikeTODO.Tests
{
    [TestClass]
    public class SessionTest
    {
        [TestMethod]
        public void TestLoadProjects()
        {
            Collection<Project> prjList = GetTestProjectList();
            DefinedContexts dc = GetTestContexts();
            Session session = LoadTestSession(prjList, dc);

            Assert.IsTrue(session.Initialized);
            Assert.AreEqual(prjList[0].UniqueID, session.ProjectEnumerable().ToList()[0].UniqueID);
            Assert.AreEqual(prjList[1].UniqueID, session.ProjectEnumerable().ToList()[1].UniqueID);
            Assert.AreEqual(2, session.Contexts.Count);
            Assert.AreEqual(dc.FindIdByDescr("Home1").ID, session.Contexts.FindIdByDescr("Home1").ID);
            Assert.AreEqual(dc.FindIdByDescr("Home2").ID, session.Contexts.FindIdByDescr("Home2").ID);
        }

        [TestMethod]
        public void TestLoadProjectsWithTaskOrder()
        {
            var prj1Mock = new Mock<Project>();
            var prj2Mock = new Mock<Project>();
            var prjList = new Collection<Project>();
            prjList.Add(prj1Mock.Object);
            prjList.Add(prj2Mock.Object);

            //Collection<Project> prjList = GetTestProjectList();
            DefinedContexts dc = GetTestContexts();
            Session session = LoadTestSession(prjList, dc);

            Assert.IsTrue(session.Initialized);
            Assert.AreEqual(prjList[0].UniqueID, session.ProjectEnumerable().ToList()[0].UniqueID);
            Assert.AreEqual(prjList[1].UniqueID, session.ProjectEnumerable().ToList()[1].UniqueID);
            Assert.AreEqual(2, session.Contexts.Count);

            prj1Mock.Verify(m => m.OrderTasks());
            prj2Mock.Verify(m => m.OrderTasks());

            Assert.AreEqual(dc.FindIdByDescr("Home1").ID, session.Contexts.FindIdByDescr("Home1").ID);
            Assert.AreEqual(dc.FindIdByDescr("Home2").ID, session.Contexts.FindIdByDescr("Home2").ID);
        }

        [TestMethod]
        public void TestSave()
        {
            Collection<Project> prjList = GetTestProjectList();
            DefinedContexts dc = GetTestContexts();
            var persistenceMock = new Mock<IPersistence>();
            Session session = LoadTestSession(prjList, dc, persistenceMock);

            session.Save();
            persistenceMock.Verify(m => m.SaveProjects(prjList));
            persistenceMock.Verify(m => m.SaveContexts(dc));
        }

        [TestMethod]
        public void TestSaveWithTaskOrder()
        {
            var prj1Mock = new Mock<Project>();
            var prj2Mock = new Mock<Project>();
            var prjList = new Collection<Project>();
            prjList.Add(prj1Mock.Object);
            prjList.Add(prj2Mock.Object);

            DefinedContexts dc = GetTestContexts();
            var persistenceMock = new Mock<IPersistence>();
            Session session = LoadTestSession(prjList, dc, persistenceMock);

            session.Save();
            persistenceMock.Verify(m => m.SaveProjects(prjList));
            persistenceMock.Verify(m => m.SaveContexts(dc));
            prj1Mock.Verify(m => m.OrderTasks());
            prj2Mock.Verify(m => m.OrderTasks());
        }

        [TestMethod]
        public void TestListProjectsWithNoTasks()
        {
            Collection<Project> prjList = GetTestProjectList();
            Session session = LoadTestSession(prjList);
            var expected = session.ProjectEnumerable().ToList().Where(p => !p.HasTasks);
            var actual = session.ListProjectsWithNoTasks();
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [TestMethod]
        public void TestFindProject()
        {
            var prj1 = new Project("Test Project1", DateTime.Parse("1/1/2017"));
            var prj2 = new Project("Test Project2", DateTime.Parse("1/1/2018"));
            var task = new Task("Test Task 1.1", DateTime.Parse("7/1/2016"));
            prj1.AddTask(task);
            var prjList = new Collection<Project>();
            prjList.Add(prj1);
            prjList.Add(prj2);

            Session session = LoadTestSession(prjList);

            var foundPrj = session.FindProjectByID(prj1.UniqueID.ToString());
            Assert.AreEqual(prj1.UniqueID, foundPrj.UniqueID);
        }

        [TestMethod]
        public void TestFindProjectInvalidID()
        {
            var prj1 = new Project("Test Project1", DateTime.Parse("1/1/2017"));
            var prj2 = new Project("Test Project2", DateTime.Parse("1/1/2018"));
            var task = new Task("Test Task 1.1", DateTime.Parse("7/1/2016"));
//            prj1.AddTask(task);
            var prjList = new Collection<Project>();
            prjList.Add(prj1);
            prjList.Add(prj2);

            Session session = LoadTestSession(prjList);

            var foundPrj = session.FindProjectByID("blah");
            Assert.IsNull(foundPrj);
        }

        [TestMethod]
        public void TestFindProjectUninitialized()
        {
            var mock = new Mock<IPersistence>();
            Session session = new Session(mock.Object);

            var foundPrj = session.FindProjectByID("blah");
            Assert.IsNull(foundPrj);
        }

        [TestMethod]
        public void TestIsInitialized()
        {
            var mock = new Mock<IPersistence>();
            Session session = new Session(mock.Object);

            Assert.IsFalse(session.Initialized);
        }

        [TestMethod]
        public void TestAddProject()
        {
            Collection<Project> prjList = GetTestProjectList();
            Session session = LoadTestSession(prjList);

            int prjCount = session.ProjectCount;

            var prj = new Project("Test Project New", DateTime.Parse("1/1/2019"));

            session.AddProject(prj);

            Assert.AreEqual(prjCount + 1, session.ProjectCount);
            Assert.IsTrue(session.FindProjectByID(prj.UniqueID.ToString()) != null);
        }

        [TestMethod]
        public void TestRemoveProject()
        {
            Collection<Project> prjList = GetTestProjectList();
            Session session = LoadTestSession(prjList);
            int prjCount = session.ProjectCount;

            var prj = new Project("Test Project New", DateTime.Parse("1/1/2019"));
            var prjID = prj.UniqueID.ToString();

            session.AddProject(prj);
            session.RemoveProject(prj.UniqueID.ToString());

            Assert.AreEqual(prjCount, session.ProjectCount);
            Assert.IsFalse(session.FindProjectByID(prjID) != null);
        }

        [TestMethod]
        public void TestRemoveProjectInvalid()
        {
            Collection<Project> prjList = GetTestProjectList();
            Session session = LoadTestSession(prjList);
            int prjCount = session.ProjectCount;

            session.RemoveProject("blah");
            Assert.AreEqual(prjCount, session.ProjectCount);
        }

        [TestMethod]
        public void TestProjectCount()
        {
            Collection<Project> prjList = GetTestProjectList();
            Session session = LoadTestSession(prjList);

            Assert.AreEqual(2, session.ProjectCount);
        }

        [TestMethod]
        public void TestProjectCount0()
        {
            Collection<Project> prjList = new Collection<Project>();
            Session session = LoadTestSession(prjList);

            Assert.AreEqual(0, session.ProjectCount);
        }

        [TestMethod]
        public void TestProjectCountUnitialized()
        {
            var mock = new Mock<IPersistence>();
            Session session = new Session(mock.Object);

            Assert.AreEqual(0, session.ProjectCount);
        }

        [TestMethod]
        public void TestGetNextProject()
        {
            var prj1 = new Project("Test Project1", DateTime.Parse("1/1/2017"));
            var prj2 = new Project("Test Project2", DateTime.Parse("1/1/2018"));
            var task = new Task("Test Task 1.1", DateTime.Parse("7/1/2016"));
            prj1.AddTask(task);
            var prjList = new Collection<Project>();
            prjList.Add(prj1);
            prjList.Add(prj2);
            var session = LoadTestSession(prjList);

            var firstPrj = session.ProjectEnumerable();
            var secondPrj = session.ProjectEnumerable();
            Project[] projects = new Project[2];

            int i = 0;
            foreach (var prj in session.ProjectEnumerable())
            {
                projects[i] = prj;
                i++;
            }

            Assert.AreEqual(prj1.UniqueID, projects[0].UniqueID);
            Assert.AreEqual(prj2.UniqueID, projects[1].UniqueID);
        }

        [TestMethod]
        public void TestUnitializedGetNextProject()
        {
            var mock = new Mock<IPersistence>();
            Session session = new Session(mock.Object);

            int i = 0;
            foreach (var prj in session.ProjectEnumerable())
            {
                i++;
            }

            Assert.AreEqual(0, i);
        }

        [TestMethod]
        public void TestLoadDefaultContexts()
        {
            Collection<Project> prjList = GetTestProjectList();
            DefinedContexts dc = GetTestContexts();
            Session session = LoadTestSession(prjList, null);

            Assert.IsNull(session.Contexts);
            Assert.IsTrue(session.LoadDefaultContexts());

            DefinedContexts defaultContexts = new DefaultContextGenerator().GenerateDefaultContexts();
            Assert.AreEqual(defaultContexts.Count, session.Contexts.Count);
            foreach (var c in defaultContexts.GetList())
            {
                Assert.AreEqual(c.Description, session.Contexts.FindIdByDescr(c.Description).Description);
            }

        }

        [TestMethod]
        public void TestLoadDefaultContextsFail()
        {
            Collection<Project> prjList = GetTestProjectList();
            DefinedContexts dc = GetTestContexts();
            Session session = LoadTestSession(prjList, null);

            session.Contexts = dc;
            Assert.IsNotNull(session.Contexts);
               
            // LoadDefaultContexts should not replace existing Contexts.
            Assert.IsFalse(session.LoadDefaultContexts());

            Assert.AreEqual(dc.Count, session.Contexts.Count);
            foreach (var c in dc.GetList())
            {
                Assert.AreEqual(c.Description, session.Contexts.FindIdByDescr(c.Description).Description);
            }

        }

        [TestMethod]
        public void TestGetTaskList()
        {
            Collection<Project> prjList = GetTestProjectList();
            Session session = LoadTestSession(prjList);
            Project prj1 = prjList[0];
            Task task = prj1.GetNextTask();
            prj1.MoveTaskLast(task.UniqueID.ToString());
            List<Task> taskList = session.GetTaskList(prjList[0].UniqueID);

            Assert.AreEqual(3, taskList.Count);
            Assert.AreEqual(1000, taskList[0].Order);
            Assert.AreEqual(2000, taskList[1].Order);
            Assert.AreEqual(3000, taskList[2].Order);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestGetTaskListInvalid()
        {
            Collection<Project> prjList = GetTestProjectList();
            Session session = LoadTestSession(prjList);
            List<Task> taskList = session.GetTaskList(new Guid());
        }

        private DefinedContexts GetTestContexts()
        {
            String id1 = Guid.NewGuid().ToString();
            String id2 = Guid.NewGuid().ToString();
            Context context1 = new Context(id1, "Home1");
            Context context2 = new Context(id2, "Home2");
            DefinedContexts dc = new DefinedContexts();
            dc.Add(context1);
            dc.Add(context2);
            return dc;
        }


        private static Collection<Project> GetTestProjectList()
        {
            var prj1 = new Project("Test Project1", DateTime.Parse("1/1/2017"));
            var prj2 = new Project("Test Project2", DateTime.Parse("1/1/2018"));
            var task = new Task("Test Task 1.1", DateTime.Parse("7/1/2016"));
            prj1.AddTask(task);
            prj1.AddTask(new Task("Test Task 1.2", DateTime.Parse("7/1/2018")));
            prj1.AddTask(new Task("Test Task 1.3", DateTime.Parse("7/1/2017")));
            var prjList = new Collection<Project>();
            prjList.Add(prj1);
            prjList.Add(prj2);
            return prjList;
        }

        private static Session LoadTestSession(Collection<Project> prjList, DefinedContexts dc,
            Mock<IPersistence> mock)
        {
            mock.Setup(m => m.LoadProjects()).Returns(prjList);
            mock.Setup(m => m.LoadContexts()).Returns(dc);

            var persistenceMock = mock.Object;
            var session = new Session(persistenceMock);
            session.LoadProjects();
            session.LoadContexts();
            return session;
        }

        private static Session LoadTestSession(Collection<Project> prjList)
        {
            var persistenceMock = Mock.Of<IPersistence>(m => m.LoadProjects() == prjList);
            var session = new Session(persistenceMock);
            session.LoadProjects();
            return session;
        }

        private static Session LoadTestSession(Collection<Project> prjList, DefinedContexts dc)
        {
            var mock = new Mock<IPersistence>();
            mock.Setup(m => m.LoadProjects()).Returns(prjList);
            mock.Setup(m => m.LoadContexts()).Returns(dc);
            var persistenceMock = mock.Object;
            return LoadTestSession(prjList, dc, mock);
        }
    }
}
