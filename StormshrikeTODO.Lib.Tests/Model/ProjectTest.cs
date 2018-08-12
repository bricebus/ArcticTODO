using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using StormshrikeTODO.Model;

namespace StormshrikeTODO.Tests
{
    [TestClass]
    public class ProjectTest
    {
        [TestMethod]
        public void TestProjectDueDate()
        {
            Project prj = new Project("Test Project", DateTime.Parse("1/1/2016"));

            Assert.AreEqual("Test Project", prj.ProjectName);
            Assert.AreEqual(DateTime.Parse("1/1/2016"), prj.DueDate);
            Assert.IsFalse(prj.HasTasks);
            Assert.AreEqual(0, prj.TaskCount);
            Assert.AreNotEqual("00000000-0000-0000-0000-000000000000", prj.UniqueID.ToString());

        }

        [TestMethod]
        public void TestProjectWithNoDueDate()
        {
            Project prj = new Project("Test Project");

            Assert.IsFalse(prj.DueDate.HasValue);
            Assert.AreNotEqual("00000000-0000-0000-0000-000000000000", prj.UniqueID.ToString());
        }

        [TestMethod]
        public void TestProjectAddTask()
        {
            Project prj = NewProject();

            Assert.IsTrue(prj.HasTasks);
            Assert.AreEqual(1, prj.TaskCount);
        }

        private Project NewProject()
        {
            Task task = new Task("A New Task");
            Project prj = new Project("Test Project");
            prj.AddTask(task);
            return prj;
        }

        [TestMethod]
        public void TestTaskOrdering()
        {
            Task task1 = new Task("A New Task");
            Task task2 = new Task("Another New Task");
            Project prj = new Project("Test Project");
            prj.AddTask(task2);
            prj.AddTask(task1);

            Assert.IsTrue(prj.HasTasks);
            Assert.AreEqual(2, prj.TaskCount);

            Assert.AreEqual(task2.Name, prj.GetNextTask().Name);
        }

        [TestMethod]
        public void TestToStringNoDueDate()
        {
            Project prj = new Project("Test Project");

            String id = prj.UniqueID.ToString();
            Assert.AreEqual("ID:{" + id + "},Name:{Test Project},DueDate:{},TaskCount:{0}", prj.ToString());
        }

        [TestMethod]
        public void TestToStringWithDueDate()
        {
            Project prj = new Project("Test Project", DateTime.Parse("04/06/2018"));

            String id = prj.UniqueID.ToString();
            Assert.AreEqual("ID:{" + id + "},Name:{Test Project},DueDate:{4/6/2018},TaskCount:{0}", prj.ToString());
            
        }

        [TestMethod]
        public void TestEmptyTaskList()
        {
            Project prj = new Project("Test Project");
            Assert.AreEqual(0, prj.GetTaskList().Count);
        }

        [TestMethod]
        public void TestPopulatedTaskList()
        {
            Project prj = new Project("Test Project");
            Task task1 = new Task("A New Task");
            Task task2 = new Task("Another New Task");
            prj.AddTask(task1);
            prj.AddTask(task2);

            var taskListFromProject = prj.GetTaskList();

            Assert.AreEqual(2, taskListFromProject.Count);

            Task task1fromList = taskListFromProject.ElementAt(0);
            Task task2fromList = taskListFromProject.ElementAt(1);
            Assert.AreEqual("A New Task", task1fromList.Name);
            Assert.AreEqual("Another New Task", task2fromList.Name);
            Assert.AreNotSame(task1, task1fromList);
            Assert.AreNotSame(task2, task2fromList);
        }

        [TestMethod]
        public void TestFindByGUID()
        {

            Project prj = new Project("Test Project");
            Task task1 = new Task("A New Task");
            Guid task1Id = task1.UniqueID;
            Task task2 = new Task("Another New Task");
            prj.AddTask(task1);
            prj.AddTask(task2);

            var taskListFromProject = prj.GetTaskList();
            var foundTask = taskListFromProject.Where(t => t.UniqueID == task1Id).First();

            Assert.AreEqual("A New Task", foundTask.Name);
        }

        [TestMethod]
        public void TestInsertAfterByPos()
        {
            Project prj = new Project("Test Project");
            Task task1 = new Task("A New Task");
            Task task2 = new Task("Another New Task");
            Task task3 = new Task("Yet Another New Task");
            Guid task1Id = task1.UniqueID;
            Guid task2Id = task2.UniqueID;
            Guid task3Id = task3.UniqueID;
            prj.AddTask(task1);
            prj.AddTask(task2);

            prj.InsertAfter(1, task3);

            Assert.AreEqual(3, prj.GetTaskList().Count);
            Assert.AreEqual(task1Id, prj.GetTaskList().ElementAt(0).UniqueID);
            Assert.AreEqual(task3Id, prj.GetTaskList().ElementAt(1).UniqueID);
            Assert.AreEqual(task2Id, prj.GetTaskList().ElementAt(2).UniqueID);
        }

        [TestMethod]
        public void TestInsertAfterByUniqueID()
        {
            Project prj = new Project("Test Project");
            Task task1 = new Task("A New Task");
            Task task2 = new Task("Another New Task");
            Task task3 = new Task("Yet Another New Task");
            Guid task1Id = task1.UniqueID;
            Guid task2Id = task2.UniqueID;
            Guid task3Id = task3.UniqueID;
            prj.AddTask(task1);
            prj.AddTask(task2);

            prj.InsertAfter(task2Id, task3);

            Assert.AreEqual(3, prj.GetTaskList().Count);
            Assert.AreEqual(task1Id, prj.GetTaskList().ElementAt(0).UniqueID);
            Assert.AreEqual(task2Id, prj.GetTaskList().ElementAt(1).UniqueID);
            Assert.AreEqual(task3Id, prj.GetTaskList().ElementAt(2).UniqueID);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInsertAfterPosTooSmall()
        {
            Project prj = new Project("Test Project");
            Task task1 = new Task("A New Task");
            Task task2 = new Task("Another New Task");
            Task task3 = new Task("Yet Another New Task");
            Guid task3Id = task3.UniqueID;
            prj.AddTask(task1);
            prj.AddTask(task2);

            prj.InsertAfter(0, task3);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInsertAfterPosTooLarge()
        {
            Project prj = new Project("Test Project");
            Task task1 = new Task("A New Task");
            Task task2 = new Task("Another New Task");
            Task task3 = new Task("Yet Another New Task");
            Guid task3Id = task3.UniqueID;
            prj.AddTask(task1);
            prj.AddTask(task2);

            prj.InsertAfter(44, task3);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInsertAfterPosNullTaskID()
        {
            Project prj = new Project("Test Project");
            Task task1 = new Task("A New Task");
            Task task2 = new Task("Another New Task");
            prj.AddTask(task1);
            prj.AddTask(task2);

            prj.InsertAfter(1, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInsertAfterInvalidTaskID()
        {
            Project prj = new Project("Test Project");
            Task task1 = new Task("A New Task");
            Task task2 = new Task("Another New Task");
            Task task3 = new Task("Yet Another New Task");
            Guid task3Id = task3.UniqueID;
            prj.AddTask(task1);
            prj.AddTask(task2);

            prj.InsertAfter(Guid.NewGuid(), task3);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInsertAfterNullTaskID()
        {
            Project prj = new Project("Test Project");
            Task task1 = new Task("A New Task");
            Task task2 = new Task("Another New Task");
            prj.AddTask(task1);
            prj.AddTask(task2);

            prj.InsertAfter(Guid.NewGuid(), null);
        }

        [TestMethod]
        public void TestGetTaskByGuidID()
        {
            Project prj = new Project("Test Project");
            Task task1 = new Task("A New Task");
            Task task2 = new Task("Another New Task");
            prj.AddTask(task1);
            prj.AddTask(task2);

            Task foundTask = prj.GetTask(task1.UniqueID);

            Assert.AreEqual(task1.UniqueID, foundTask.UniqueID);
        }

        [TestMethod]
        public void TestInvalidGetTaskByGuidID()
        {
            Project prj = new Project("Test Project");
            Task task1 = new Task("A New Task");
            Task task2 = new Task("Another New Task");
            prj.AddTask(task1);
            prj.AddTask(task2);

            Task foundTask = prj.GetTask(Guid.NewGuid());
            Assert.IsNull(foundTask);
        }

        [TestMethod]
        public void TestGetTaskByStringID()
        {
            Project prj = new Project("Test Project");
            Task task1 = new Task("A New Task");
            Task task2 = new Task("Another New Task");
            prj.AddTask(task1);
            prj.AddTask(task2);

            Task foundTask = prj.GetTask(task1.UniqueID.ToString());

            Assert.AreEqual(task1.UniqueID, foundTask.UniqueID);
        }

        [TestMethod]
        public void TestInvalidGetTaskByStringID()
        {
            Project prj = new Project("Test Project");
            Task task1 = new Task("A New Task");
            Task task2 = new Task("Another New Task");
            prj.AddTask(task1);
            prj.AddTask(task2);

            Task foundTask = prj.GetTask("blah");

            Assert.IsNull(foundTask);
        }
    }
}
