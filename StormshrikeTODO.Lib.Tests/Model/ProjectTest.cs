using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using StormshrikeTODO.Model;
using System.Collections.ObjectModel;
using System.Reflection;

namespace StormshrikeTODO.Model.Tests
{
    [TestClass]
    public class ProjectTest
    {

        [TestMethod]
        public void TestTaskMembers()
        {
            // Compare members (public and private) of the Project class to a "known" list.  If any
            // of them change, the test should fail as a reminder that any changes to the Project
            // class should be evaluated to see if the IsEquivalent() method should change.
            String[] actualMembers = new string[5];
            int idx = 0;
            typeof(Project).GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public).ToList()
                .ForEach(x => actualMembers[idx++] = x.ToString());

            String[] expected = new string[]
            {
                "System.Collections.ObjectModel.Collection`1[StormshrikeTODO.Model.Task] _taskList",
                "System.Guid <UniqueID>k__BackingField",
                "System.String <ProjectName>k__BackingField",
                "System.Nullable`1[System.DateTime] <DueDate>k__BackingField",
                "System.DateTime <DateTimeCreated>k__BackingField"
            };

            Assert.AreEqual(5, idx);

            for (int i = 0; i < actualMembers.Length; i++)
            {
                Assert.AreEqual(expected[i], actualMembers[i], "idx == " + i.ToString());
            }
        }
        [TestMethod]
        public void TestDateTimeCreated()
        {

            DateTime dt1 = DateTime.Now;

            Project prj = new Project("TestProject1");

            DateTime dt2 = DateTime.Now;
            Assert.IsTrue(prj.DateTimeCreated >= dt1);
            Assert.IsTrue(prj.DateTimeCreated <= dt2);
        }

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
            Task task1 = new Task("A New Task")
            {
                Order = 1000
            };
            Task task2 = new Task("Another New Task")
            {
                Order = 2000
            };
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
            CreateThreeTasks(out Project prj, out Task foundTask1, out Task foundTask2, out Task foundTask3);

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
            CreateThreeTasks(out Project prj, out Task foundTask1, out Task foundTask2, out Task foundTask3);

            prj.MoveTaskLast(foundTask2.UniqueID.ToString());

            Assert.AreEqual(foundTask1.Name, prj.GetNextTask().Name);
            Assert.AreEqual(1000, foundTask1.Order);
            Assert.AreEqual(3000, foundTask2.Order);
            Assert.AreEqual(2000, foundTask3.Order);
        }

        [TestMethod]
        public void TestMoveTaskUp()
        {
            CreateThreeTasks(out Project prj, out Task foundTask1, out Task foundTask2, out Task foundTask3);

            prj.MoveTaskUp(foundTask2.UniqueID.ToString());

            Assert.AreEqual(foundTask2.Name, prj.GetNextTask().Name);
            Assert.AreEqual(2000, foundTask1.Order);
            Assert.AreEqual(1000, foundTask2.Order);
            Assert.AreEqual(3000, foundTask3.Order);
        }

        [TestMethod]
        public void TestMoveTaskUpAlreadyFirst()
        {
            CreateThreeTasks(out Project prj, out Task foundTask1, out Task foundTask2, out Task foundTask3);

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
            CreateThreeTasks(out Project prj, out Task foundTask1, out Task foundTask2, out Task foundTask3);

            prj.MoveTaskUp("blah");
        }

        [TestMethod]
        public void TestMoveTaskDown()
        {
            CreateThreeTasks(out Project prj, out Task foundTask1, out Task foundTask2, out Task foundTask3);

            prj.MoveTaskDown(foundTask2.UniqueID.ToString());

            Assert.AreEqual(foundTask1.Name, prj.GetNextTask().Name);
            Assert.AreEqual(1000, foundTask1.Order);
            Assert.AreEqual(3000, foundTask2.Order);
            Assert.AreEqual(2000, foundTask3.Order);
        }

        [TestMethod]
        public void TestMoveTaskDownAlreadyLast()
        {
            CreateThreeTasks(out Project prj, out Task foundTask1, out Task foundTask2, out Task foundTask3);

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
            CreateThreeTasks(out Project prj, out Task foundTask1, out Task foundTask2, out Task foundTask3);

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

        [TestMethod]
        public void TestTaskListCompare_Same()
        {
            CreateThreeTasks(out Project prj1, out Task foundTask1_1, out Task foundTask1_2, out Task foundTask1_3);

            var taskArray = SetUpThreeTasks();
            var taskCollection = new Collection<Task>();
            taskArray[0].DateTimeCreated = foundTask1_1.DateTimeCreated;
            taskArray[1].DateTimeCreated = foundTask1_2.DateTimeCreated;
            taskArray[2].DateTimeCreated = foundTask1_3.DateTimeCreated;
            taskArray.ToList().ForEach(t => taskCollection.Add(t));

            Assert.IsTrue(prj1.IsTaskListEquivalentTo(taskCollection));
        }

        [TestMethod]
        public void TestTaskListCompare_Changed()
        {
            CreateThreeTasks(out Project prj1, out Task foundTask1_1, out Task foundTask1_2, out Task foundTask1_3);

            var taskArray = SetUpThreeTasks();
            var taskCollection = new Collection<Task>();
            taskArray[0].DateTimeCreated = DateTime.Parse("2015-01-01");
            taskArray[1].DateTimeCreated = foundTask1_2.DateTimeCreated;
            taskArray[2].DateTimeCreated = foundTask1_3.DateTimeCreated;
            taskArray.ToList().ForEach(t => taskCollection.Add(t));

            Assert.IsFalse(prj1.IsTaskListEquivalentTo(taskCollection));
        }

        [TestMethod]
        public void TestTaskListCompare_Deleted()
        {
            CreateThreeTasks(out Project prj1, out Task foundTask1_1, out Task foundTask1_2, out Task foundTask1_3);

            var taskArray = SetUpThreeTasks();
            var taskCollection = new Collection<Task>();
            taskArray[0].DateTimeCreated = foundTask1_1.DateTimeCreated;
            taskArray[1].DateTimeCreated = foundTask1_2.DateTimeCreated;

            taskCollection.Add(taskArray[0]);
            taskCollection.Add(taskArray[1]);

            Assert.IsFalse(prj1.IsTaskListEquivalentTo(taskCollection));
        }

        [TestMethod]
        public void TestTaskListCompare_New()
        {
            CreateThreeTasks(out Project prj1, out Task foundTask1_1, out Task foundTask1_2, out Task foundTask1_3);

            var taskArray = SetUpThreeTasks();
            var taskCollection = new Collection<Task>();
            taskArray[0].DateTimeCreated = foundTask1_1.DateTimeCreated;
            taskArray[1].DateTimeCreated = foundTask1_2.DateTimeCreated;
            taskArray[2].DateTimeCreated = foundTask1_3.DateTimeCreated;

            taskArray.ToList().ForEach(t => taskCollection.Add(t));
            taskCollection.Add(new Task("New Task"));

            Assert.IsFalse(prj1.IsTaskListEquivalentTo(taskCollection));
        }

        [TestMethod]
        public void TestProjectEquivalence_NoTasks_Same()
        {
            var prj1 = CreateTestProject();
            var prj2 = CreateTestProject();
            prj2.UniqueID = prj1.UniqueID;
            prj2.DateTimeCreated = prj1.DateTimeCreated;

            Assert.IsTrue(prj1.IsEquivalentTo(prj2));
            Assert.IsTrue(prj1.AreProjectAttributesEquivalentTo(prj2));
        }

        [TestMethod]
        public void TestProjectEquivalence_ID_Different()
        {
            var prj1 = CreateTestProject();
            var prj2 = CreateTestProject();
            prj2.DateTimeCreated = prj1.DateTimeCreated;
            prj2.UniqueID = prj1.UniqueID;
            Assert.IsTrue(prj1.IsEquivalentTo(prj2));

            prj2.UniqueID = Guid.NewGuid();
            Assert.IsFalse(prj1.IsEquivalentTo(prj2));
            Assert.IsFalse(prj1.AreProjectAttributesEquivalentTo(prj2));
        }

        [TestMethod]
        public void TestProjectEquivalence_DateTimeCreated_Different()
        {
            var prj1 = CreateTestProject();
            var prj2 = CreateTestProject();
            prj2.UniqueID = prj1.UniqueID;
            prj2.DateTimeCreated = prj1.DateTimeCreated;
            Assert.IsTrue(prj1.IsEquivalentTo(prj2));

            prj2.DateTimeCreated = DateTime.Parse("2018-01-01");

            Assert.IsFalse(prj1.IsEquivalentTo(prj2));
            Assert.IsFalse(prj1.AreProjectAttributesEquivalentTo(prj2));
        }

        [TestMethod]
        public void TestProjectEquivalence_DateDue_Different()
        {
            var prj1 = CreateTestProject();
            var prj2 = CreateTestProject();
            prj2.UniqueID = prj1.UniqueID;
            prj2.DateTimeCreated = prj1.DateTimeCreated;
            Assert.IsTrue(prj1.IsEquivalentTo(prj2));

            prj1.DueDate = DateTime.Parse("2019-01-01");
            prj2.DueDate = DateTime.Parse("2020-01-01");

            Assert.IsFalse(prj1.IsEquivalentTo(prj2));
            Assert.IsFalse(prj1.AreProjectAttributesEquivalentTo(prj2));
        }

        [TestMethod]
        public void TestProjectEquivalence_Name_Different()
        {
            var prj1 = CreateTestProject();
            var prj2 = CreateTestProject();
            prj2.UniqueID = prj1.UniqueID;
            prj2.DateTimeCreated = prj1.DateTimeCreated;
            Assert.IsTrue(prj1.IsEquivalentTo(prj2));

            prj2.ProjectName = "Blah";
            Assert.IsFalse(prj1.IsEquivalentTo(prj2));
            Assert.IsFalse(prj1.AreProjectAttributesEquivalentTo(prj2));
        }

        [TestMethod]
        public void TestProjectEquivalence_WithTasks_Same()
        {
            CreateThreeTasks(out Project prj1, out Task foundTask1_1, out Task foundTask1_2, out Task foundTask1_3);
            CreateThreeTasks(out Project prj2, out Task foundTask2_1, out Task foundTask2_2, out Task foundTask2_3);
            foundTask2_1.DateTimeCreated = foundTask1_1.DateTimeCreated;
            foundTask2_2.DateTimeCreated = foundTask1_2.DateTimeCreated;
            foundTask2_3.DateTimeCreated = foundTask1_3.DateTimeCreated;

            prj2.UniqueID = prj1.UniqueID;
            prj2.DateTimeCreated = prj1.DateTimeCreated;
            Assert.IsTrue(prj1.IsEquivalentTo(prj2));
            Assert.IsTrue(prj1.AreProjectAttributesEquivalentTo(prj2));
        }


        [TestMethod]
        public void TestProjectEquivalence_WithOneProjectHavingTasks_Different()
        {
            CreateThreeTasks(out Project prj1, out Task foundTask1_1, out Task foundTask1_2, out Task foundTask1_3);
            var prj2 = CreateTestProject();

            prj2.UniqueID = prj1.UniqueID;
            prj2.DateTimeCreated = prj1.DateTimeCreated;
            Assert.IsFalse(prj1.IsEquivalentTo(prj2));
            Assert.IsTrue(prj1.AreProjectAttributesEquivalentTo(prj2));
        }

        [TestMethod]
        public void TestProjectEquivalence_WithBothProjecstHavingTasks_Different()
        {
            CreateThreeTasks(out Project prj1, out Task foundTask1_1, out Task foundTask1_2, out Task foundTask1_3);
            CreateThreeTasks(out Project prj2, out Task foundTask2_1, out Task foundTask2_2, out Task foundTask2_3);
            foundTask2_1.DateTimeCreated = foundTask1_1.DateTimeCreated;
            foundTask2_2.DateTimeCreated = foundTask1_2.DateTimeCreated;
            foundTask2_3.DateTimeCreated = foundTask1_3.DateTimeCreated;

            prj2.UniqueID = prj1.UniqueID;
            prj2.DateTimeCreated = prj1.DateTimeCreated;
            Assert.IsTrue(prj1.IsEquivalentTo(prj2));

            foundTask2_1.DateTimeCreated = DateTime.Parse("2018-01-01");

            Assert.IsFalse(prj1.IsEquivalentTo(prj2));
            Assert.IsTrue(prj1.AreProjectAttributesEquivalentTo(prj2));
        }

        private void CreateThreeTasks(out Project prj, out Task foundTask1, out Task foundTask2, out Task foundTask3)
        {
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

            taskArray[0] = new Task("A New Task")
            {
                Order = 2
            };

            taskArray[1] = new Task("Another New Task")
            {
                Order = 1
            };

            return taskArray;
        }

        private Task[] SetUpThreeTasks()
        {
            return TestTaskCreator.SetUpThreeTasks();
        }

        private static Project CreateTestProject()
        {
            return new Project("Test Project");
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
