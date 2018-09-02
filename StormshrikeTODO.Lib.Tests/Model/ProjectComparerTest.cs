using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StormshrikeTODO.Model.Tests
{
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class ProjectComparerTest
    {
        [TestMethod]
        public void TestSame_NoTasks()
        {
            var prj1 = new Project("Test Project 1");
            var prj2 = new Project("Test Project 2");
            var prj3 = new Project("Test Project 1");
            var prj4 = new Project("Test Project 2");
            prj3.UniqueID = prj1.UniqueID;
            prj3.DateTimeCreated = prj1.DateTimeCreated;
            prj4.UniqueID = prj2.UniqueID;
            prj4.DateTimeCreated = prj2.DateTimeCreated;

            Collection<Project> prjList1 = new Collection<Project>
            {
                prj1,
                prj2
            };
            Collection<Project> prjList2 = new Collection<Project>
            {
                prj3,
                prj4
            };

            Assert.IsTrue(ProjectComparer.AreListsEquivalent(prjList1, prjList2,
                out List<Project> chgList, out List<Project> addList, out List<Project> delList,
                out List<Project> chgTaskList));
        }

        [TestMethod]
        public void TestSame_WithTasks()
        {
            CreateTwoIdenticalProjectLists(out Collection<Project> prjList1, out Collection<Project> prjList2);

            Assert.IsTrue(ProjectComparer.AreListsEquivalent(prjList1, prjList2,
                out List<Project> chgList, out List<Project> addList, out List<Project> delList,
                out List<Project> chgTaskList));
        }

        [TestMethod]
        public void TestSameProjectAttrs_DifferentTasks()
        {
            CreateTwoIdenticalProjectLists(out Collection<Project> prjList1, out Collection<Project> prjList2);
            var changedProjectID = prjList2[0].UniqueID;
            prjList2[0].TaskList[0].Name = "Changed Name 1";

            Assert.IsFalse(ProjectComparer.AreListsEquivalent(prjList1, prjList2,
                out List<Project> chgList, out List<Project> addList, out List<Project> delList,
                out List<Project> chgTaskList));
            Assert.AreEqual(0, addList.Count);
            Assert.AreEqual(0, chgList.Count);
            Assert.AreEqual(0, delList.Count);
            Assert.AreEqual(1, chgTaskList.Count);
            Assert.AreEqual(changedProjectID, chgTaskList[0].UniqueID);
        }

        [TestMethod]
        public void TestDifferentProjectAttrs_DifferentTasks()
        {
            CreateTwoIdenticalProjectLists(out Collection<Project> prjList1, out Collection<Project> prjList2);
            var changedTaskProjectID = prjList2[0].UniqueID;
            var changedProjectID = prjList2[1].UniqueID;
            prjList2[0].TaskList[0].Name = "Changed Task Name 1";
            prjList2[1].ProjectName = "Changed Project Name 1";

            Assert.IsFalse(ProjectComparer.AreListsEquivalent(prjList1, prjList2,
                out List<Project> chgList, out List<Project> addList, out List<Project> delList,
                out List<Project> chgTaskList));
            Assert.AreEqual(0, addList.Count);
            Assert.AreEqual(1, chgList.Count);
            Assert.AreEqual(0, delList.Count);
            Assert.AreEqual(1, chgTaskList.Count);
            Assert.AreEqual(changedProjectID, chgList[0].UniqueID);
            Assert.AreEqual(changedTaskProjectID, chgTaskList[0].UniqueID);
        }

        [TestMethod]
        public void TestDifferent_NoTasks()
        {
            var prj1 = new Project("Test Project 1");
            var prj2 = new Project("Test Project 2");
            var prj3 = new Project("Test Project 1");
            var prj4 = new Project("Test Project 2");

            Collection<Project> prjList1 = new Collection<Project>
            {
                prj1,
                prj2
            };
            Collection<Project> prjList2 = new Collection<Project>
            {
                prj3,
                prj4
            };

            // Different UniqueIDs and DateTimeCreated
            Assert.IsFalse(ProjectComparer.AreListsEquivalent(prjList1, prjList2,
                out List<Project> chgList, out List<Project> addList, out List<Project> delList,
                out List<Project> chgTaskList));
            Assert.AreEqual(2, addList.Count);
            Assert.AreEqual(0, chgList.Count);
            Assert.AreEqual(2, delList.Count);
            Assert.AreEqual(0, chgTaskList.Count);
        }

        [TestMethod]
        public void TestFirstNull()
        {
            Collection<Project> prjList1 = null;
            var prj1 = new Project("Test Project 1");
            var prj2 = new Project("Test Project 2");
            Collection<Project> prjList2 = new Collection<Project>
            {
                prj1,
                prj2
            };

            Assert.IsFalse(ProjectComparer.AreListsEquivalent(prjList1, prjList2,
                out List<Project> chgList, out List<Project> addList, out List<Project> delList,
                out List<Project> chgTaskList));
            Assert.AreEqual(2, addList.Count);
            Assert.AreEqual(0, chgList.Count);
            Assert.AreEqual(0, delList.Count);
            Assert.AreEqual(0, chgTaskList.Count);
        }

        [TestMethod]
        public void TestSecondNull()
        {
            var prj1 = new Project("Test Project 1");
            var prj2 = new Project("Test Project 2");
            Collection<Project> prjList1 = new Collection<Project>
            {
                prj1,
                prj2
            };
            Collection<Project> prjList2 = null;

            Assert.IsFalse(ProjectComparer.AreListsEquivalent(prjList1, prjList2,
                out List<Project> chgList, out List<Project> addList, out List<Project> delList,
                out List<Project> chgTaskList));
            Assert.AreEqual(0, addList.Count);
            Assert.AreEqual(0, chgList.Count);
            Assert.AreEqual(2, delList.Count);
            Assert.AreEqual(0, chgTaskList.Count);
        }

        private static Project CreateTestProject()
        {
            return new Project("Test Project");
        }

        private static void CreateTwoIdenticalProjectLists(out Collection<Project> prjList1, out Collection<Project> prjList2)
        {
            var prj1 = new Project("Test Project 1");
            var prj2 = new Project("Test Project 2");
            var prj3 = new Project("Test Project 1");
            var prj4 = new Project("Test Project 2");
            prj3.UniqueID = prj1.UniqueID;
            prj3.DateTimeCreated = prj1.DateTimeCreated;
            prj4.UniqueID = prj2.UniqueID;
            prj4.DateTimeCreated = prj2.DateTimeCreated;

            var taskList1 = TestTaskCreator.SetUpThreeTasksList();
            var taskList2 = TestTaskCreator.SetUpThreeTasksList();
            var taskList3 = TestTaskCreator.SetUpThreeTasksList();
            var taskList4 = TestTaskCreator.SetUpThreeTasksList();
            taskList3[0].UniqueID = taskList1[0].UniqueID;
            taskList3[0].DateTimeCreated = taskList1[0].DateTimeCreated;
            taskList3[1].UniqueID = taskList1[1].UniqueID;
            taskList3[1].DateTimeCreated = taskList1[1].DateTimeCreated;
            taskList3[2].UniqueID = taskList1[2].UniqueID;
            taskList3[2].DateTimeCreated = taskList1[2].DateTimeCreated;
            taskList4[0].UniqueID = taskList2[0].UniqueID;
            taskList4[0].DateTimeCreated = taskList2[0].DateTimeCreated;
            taskList4[1].UniqueID = taskList2[1].UniqueID;
            taskList4[1].DateTimeCreated = taskList2[1].DateTimeCreated;
            taskList4[2].UniqueID = taskList2[2].UniqueID;
            taskList4[2].DateTimeCreated = taskList2[2].DateTimeCreated;

            taskList1.ForEach(t => prj1.AddTask(t));
            taskList2.ForEach(t => prj2.AddTask(t));
            taskList3.ForEach(t => prj3.AddTask(t));
            taskList4.ForEach(t => prj4.AddTask(t));

            prjList1 = new Collection<Project>
            {
                prj1,
                prj2
            };
            prjList2 = new Collection<Project>
            {
                prj3,
                prj4
            };
        }

    }
}
