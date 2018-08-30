using Microsoft.VisualStudio.TestTools.UnitTesting;
using StormshrikeTODO.Model;
using StormshrikeTODO.Model.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StormshrikeTODO.Tests
{
    [TestClass]
    public class TaskComparerTest
    {
        [TestMethod]
        public void TestTaskList_Same()
        {
            var taskList1 = TestTaskCreator.SetUpThreeTasksList();
            var taskList2 = TestTaskCreator.SetUpThreeTasksList();
            for (int i = 0; i < taskList1.Count; i++)
            {
                taskList2[i].DateTimeCreated = taskList1[i].DateTimeCreated;
            }

            Assert.IsTrue(new TaskComparer().AreListsEquivalent(taskList1, taskList2,
                out List<Task> chgList, out List<Task> addList, out List<Task> delList));
            Assert.AreEqual(0, chgList.Count);
            Assert.AreEqual(0, delList.Count);
            Assert.AreEqual(0, addList.Count);
        }

        [TestMethod]
        public void TestTaskList_First_List_Null()
        {
            var taskList1 = TestTaskCreator.SetUpThreeTasksList();
            List<Task> taskList2 = null;

            Assert.IsFalse(new TaskComparer().AreListsEquivalent(taskList1, taskList2,
                out List<Task> chgList, out List<Task> addList, out List<Task> delList));
            Assert.AreEqual(0, chgList.Count);
            Assert.AreEqual(3, delList.Count);
            Assert.AreEqual(0, addList.Count);
        }

        [TestMethod]
        public void TestTaskList_Second_List_Null()
        {
            List<Task> taskList1 = null;
            var taskList2 = TestTaskCreator.SetUpThreeTasksList();

            Assert.IsFalse(new TaskComparer().AreListsEquivalent(taskList1, taskList2,
                out List<Task> chgList, out List<Task> addList, out List<Task> delList));
            Assert.AreEqual(0, chgList.Count);
            Assert.AreEqual(0, delList.Count);
            Assert.AreEqual(3, addList.Count);
        }

        [TestMethod]
        public void TestTaskList_Different_Added()
        {
            var taskList1 = TestTaskCreator.SetUpThreeTasksList();
            var taskList2 = TestTaskCreator.SetUpThreeTasksList();
            for (int i = 0; i < taskList1.Count; i++)
            {
                taskList2[i].DateTimeCreated = taskList1[i].DateTimeCreated;
            }
            Assert.IsTrue(new TaskComparer().AreListsEquivalent(taskList1, taskList2,
                out List<Task> chgList, out List<Task> addList, out List<Task> delList));

            var newTask = new Task("new test task");
            var newID = newTask.UniqueID;
            taskList2.Add(newTask);

            Assert.IsFalse(new TaskComparer().AreListsEquivalent(taskList1, taskList2,
                out chgList, out addList, out delList));
            Assert.AreEqual(0, chgList.Count);
            Assert.AreEqual(0, delList.Count);
            Assert.AreEqual(1, addList.Count);
            Assert.IsTrue(addList.First().UniqueID == newID);
        }

        [TestMethod]
        public void TestTaskList_Different_Deleted()
        {
            var taskList1 = TestTaskCreator.SetUpThreeTasksList();
            var taskList2 = TestTaskCreator.SetUpThreeTasksList();
            for (int i = 0; i < taskList1.Count; i++)
            {
                taskList2[i].DateTimeCreated = taskList1[i].DateTimeCreated;
            }
            Assert.IsTrue(new TaskComparer().AreListsEquivalent(taskList1, taskList2,
                out List<Task> chgList, out List<Task> addList, out List<Task> delList));

            var removedID = taskList2[0].UniqueID;
            taskList2.RemoveAt(0);

            Assert.IsFalse(new TaskComparer().AreListsEquivalent(taskList1, taskList2,
                out chgList, out addList, out delList));
            Assert.AreEqual(0, chgList.Count);
            Assert.AreEqual(1, delList.Count);
            Assert.AreEqual(0, addList.Count);
            Assert.IsTrue(delList.First().UniqueID == removedID);
        }

        [TestMethod]
        public void TestTaskList_Different_Changed()
        {
            var taskList1 = TestTaskCreator.SetUpThreeTasksList();
            var taskList2 = TestTaskCreator.SetUpThreeTasksList();
            for (int i = 0; i < taskList1.Count; i++)
            {
                taskList2[i].DateTimeCreated = taskList1[i].DateTimeCreated;
            }
            Assert.IsTrue(new TaskComparer().AreListsEquivalent(taskList1, taskList2,
                out List<Task> chgList, out List<Task> addList, out List<Task> delList));

            var changedID = taskList2[0].UniqueID;
            taskList2[0].Name = "New Name";

            Assert.IsFalse(new TaskComparer().AreListsEquivalent(taskList1, taskList2,
                out chgList, out addList, out delList));
            Assert.AreEqual(1, chgList.Count);
            Assert.AreEqual(0, delList.Count);
            Assert.AreEqual(0, addList.Count);
            Assert.IsTrue(chgList.First().UniqueID == changedID);
        }
    }
}
