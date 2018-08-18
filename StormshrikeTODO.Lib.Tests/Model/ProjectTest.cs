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
            task1.Order = 1000;
            Task task2 = new Task("Another New Task");
            task2.Order = 2000;
            Project prj = new Project("Test Project");
            prj.AddTask(task2);
            prj.AddTask(task1);

            Assert.IsTrue(prj.HasTasks);
            Assert.AreEqual(2, prj.TaskCount);

            Assert.AreEqual(task1.Name, prj.GetNextTask().Name);
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
            Assert.AreSame(task1, task1fromList);
            Assert.AreSame(task2, task2fromList);
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

            prj.InsertTaskAfter(1, task3);

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

            prj.InsertTaskAfter(task2Id, task3);

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

            prj.InsertTaskAfter(0, task3);
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

            prj.InsertTaskAfter(44, task3);
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

            prj.InsertTaskAfter(1, null);
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

            prj.InsertTaskAfter(Guid.NewGuid(), task3);
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

            prj.InsertTaskAfter(Guid.NewGuid(), null);
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

        [TestMethod]
        public void TestTaskOrderer()
        {
            var taskArray = SetUpTwoTasks();
            Project prj = CreateTestProject(taskArray);

            Task foundTask1 = prj.GetTask(taskArray[0].UniqueID.ToString());
            Task foundTask2 = prj.GetTask(taskArray[1].UniqueID.ToString());

            Assert.AreEqual(2000, foundTask1.Order);
            Assert.AreEqual(1000, foundTask2.Order);
        }

        [TestMethod]
        public void TestMoveTaskFirst()
        {
            Project prj;
            Task foundTask1, foundTask2, foundTask3;
            CreateThreeTasks(out prj, out foundTask1, out foundTask2, out foundTask3);

            prj.MoveTaskFirst(foundTask2.UniqueID.ToString());

            Assert.AreEqual(foundTask2.Name, prj.GetNextTask().Name);
            Assert.AreEqual(2000, foundTask1.Order);
            Assert.AreEqual(1000, foundTask2.Order);
            Assert.AreEqual(3000, foundTask3.Order);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestMoveTaskFirstInvalidID()
        {
            var taskArray = SetUpThreeTasks();
            Project prj = CreateTestProject(taskArray);

            prj.MoveTaskFirst("blah");
        }

        [TestMethod]
        public void TestMoveTaskLast()
        {
            Project prj;
            Task foundTask1, foundTask2, foundTask3;
            CreateThreeTasks(out prj, out foundTask1, out foundTask2, out foundTask3);

            prj.MoveTaskLast(foundTask2.UniqueID.ToString());

            Assert.AreEqual(foundTask1.Name, prj.GetNextTask().Name);
            Assert.AreEqual(1000, foundTask1.Order);
            Assert.AreEqual(3000, foundTask2.Order);
            Assert.AreEqual(2000, foundTask3.Order);
        }

        [TestMethod]
        public void TestMoveTaskUp()
        {
            Project prj;
            Task foundTask1, foundTask2, foundTask3;
            CreateThreeTasks(out prj, out foundTask1, out foundTask2, out foundTask3);

            prj.MoveTaskUp(foundTask2.UniqueID.ToString());

            Assert.AreEqual(foundTask2.Name, prj.GetNextTask().Name);
            Assert.AreEqual(2000, foundTask1.Order);
            Assert.AreEqual(1000, foundTask2.Order);
            Assert.AreEqual(3000, foundTask3.Order);
        }

        [TestMethod]
        public void TestMoveTaskUpAlreadyFirst()
        {
            Project prj;
            Task foundTask1, foundTask2, foundTask3;
            CreateThreeTasks(out prj, out foundTask1, out foundTask2, out foundTask3);

            prj.MoveTaskUp(foundTask1.UniqueID.ToString());

            Assert.AreEqual(foundTask1.Name, prj.GetNextTask().Name);
            Assert.AreEqual(1000, foundTask1.Order);
            Assert.AreEqual(2000, foundTask2.Order);
            Assert.AreEqual(3000, foundTask3.Order);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestMoveTaskUpInvalidTaskID()
        {
            Project prj;
            Task foundTask1, foundTask2, foundTask3;
            CreateThreeTasks(out prj, out foundTask1, out foundTask2, out foundTask3);

            prj.MoveTaskUp("blah");
        }

        [TestMethod]
        public void TestMoveTaskDown()
        {
            Project prj;
            Task foundTask1, foundTask2, foundTask3;
            CreateThreeTasks(out prj, out foundTask1, out foundTask2, out foundTask3);

            prj.MoveTaskDown(foundTask2.UniqueID.ToString());

            Assert.AreEqual(foundTask1.Name, prj.GetNextTask().Name);
            Assert.AreEqual(1000, foundTask1.Order);
            Assert.AreEqual(3000, foundTask2.Order);
            Assert.AreEqual(2000, foundTask3.Order);
        }

        [TestMethod]
        public void TestMoveTaskDownAlreadyLast()
        {
            Project prj;
            Task foundTask1, foundTask2, foundTask3;
            CreateThreeTasks(out prj, out foundTask1, out foundTask2, out foundTask3);

            prj.MoveTaskDown(foundTask3.UniqueID.ToString());

            Assert.AreEqual(foundTask1.Name, prj.GetNextTask().Name);
            Assert.AreEqual(1000, foundTask1.Order);
            Assert.AreEqual(2000, foundTask2.Order);
            Assert.AreEqual(3000, foundTask3.Order);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestMoveTaskDownInvalidTaskID()
        {
            Project prj;
            Task foundTask1, foundTask2, foundTask3;
            CreateThreeTasks(out prj, out foundTask1, out foundTask2, out foundTask3);

            prj.MoveTaskDown("blah");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestMoveTaskLastInvalidID()
        {
            var taskArray = SetUpThreeTasks();
            Project prj = CreateTestProject(taskArray);

            prj.MoveTaskLast("blah");
        }

        private void CreateThreeTasks(out Project prj, out Task foundTask1, out Task foundTask2, out Task foundTask3)
        {
            // t1.Order = 1000
            // t2.Order = 2000
            // t3.Order = 3000
            var taskArray = SetUpThreeTasks();
            prj = CreateTestProject(taskArray);

            foundTask1 = prj.GetTask(taskArray[0].UniqueID.ToString());
            foundTask2 = prj.GetTask(taskArray[1].UniqueID.ToString());
            foundTask3 = prj.GetTask(taskArray[2].UniqueID.ToString());
            Assert.AreEqual(1000, foundTask1.Order);
            Assert.AreEqual(2000, foundTask2.Order);
            Assert.AreEqual(3000, foundTask3.Order);
        }

        private Task[] SetUpTwoTasks()
        {
            var taskArray = new Task[2];

            taskArray[0] = new Task("A New Task");
            taskArray[0].Order = 2;

            taskArray[1] = new Task("Another New Task");
            taskArray[1].Order = 1;

            return taskArray;
        }

        private Task[] SetUpThreeTasks()
        {
            var taskArray = new Task[3];

            taskArray[0] = new Task("A New Task");
            taskArray[0].Order = 1;

            taskArray[1] = new Task("Another New Task");
            taskArray[1].Order = 2;

            taskArray[2] = new Task("And ANOTHER New Task");
            taskArray[2].Order = 3;

            return taskArray;
        }

        private static Project CreateTestProject(Task[] taskArray)
        {
            Project prj = new Project("Test Project");
            foreach (var task in taskArray)
            {
                prj.AddTask(task);
            }

            prj.OrderTasks();
            return prj;
        }
    }
}
