using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StormshrikeTODO.Model;

namespace StormshrikeTODO.Tests.Model
{
    [TestClass]
    public class TaskTest
    {
        [TestMethod]
        public void TestCreateTask()
        {
            Task task = new Task("TestTask1");

            DateTime dt1 = DateTime.Now;

            Assert.AreEqual("TestTask1", task.Name);
            Assert.IsFalse(task.DateDue.HasValue);
            Assert.IsFalse(task.DateStarted.HasValue);
            Assert.IsNotNull(task.UniqueID);
            Assert.AreNotEqual("00000000-0000-0000-0000-000000000000", task.UniqueID.ToString());
            Assert.AreEqual(Task.StatusEnum.NotStarted, task.Status);
            Assert.AreEqual("", task.ContextID);
            DateTime dt2 = DateTime.Now;
            Assert.IsTrue(task.DateTimeCreated >= dt1);
            Assert.IsTrue(task.DateTimeCreated <= dt2);
        }

        [TestMethod]
        public void TestTaskMembers()
        {
            // Compare members (public and private) of the Task class to a "known" list.  If any
            // of them change, the test should fail as a reminder that any changes to the Task
            // class should be evaluated to see if the IsEquivalent() method should change.
            //typeof(Task).GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
            //.Where(x => x.GetCustomAttributes(typeof(SomeAttribute), false).Length > 0); 
            String[] actualMembers = new string[10];
            int idx = 0;
            typeof(Task).GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public).ToList()
                .ForEach(x => actualMembers[idx++] = x.ToString());

            String[] expected = new string[]
            {
                "System.String <Name>k__BackingField",
                "System.String <ContextID>k__BackingField",
                "System.Nullable`1[System.DateTime] <DateDue>k__BackingField",
                "System.Nullable`1[System.DateTime] <DateStarted>k__BackingField",
                "System.Nullable`1[System.DateTime] <DateCompleted>k__BackingField",
                "StatusEnum _status",
                "System.String <Details>k__BackingField",
                "System.Guid <UniqueID>k__BackingField",
                "Int32 <Order>k__BackingField",
                "System.DateTime <DateTimeCreated>k__BackingField"
            };

            Assert.AreEqual(10, idx);

            for (int i = 0; i < actualMembers.Length; i++)
            {
                Assert.AreEqual(expected[i], actualMembers[i], "idx == " + i.ToString());
            }
        }

        [TestMethod]
        public void TestEquivalant()
        {
            Task taskA = GetTestTaskA();
            Task taskB = GetTestTaskB(taskA, "");

            Assert.IsTrue(taskA.IsEquivalentTo(taskB));
        }

        [TestMethod]
        public void TestNotEquivalant_Name()
        {
            Task taskA = GetTestTaskA();
            Task taskB = GetTestTaskB(taskA, "Name");

            Assert.IsFalse(taskA.IsEquivalentTo(taskB));
        }

        [TestMethod]
        public void TestNotEquivalant_DateTimeCreated()
        {
            Task taskA = GetTestTaskA();
            Task taskB = GetTestTaskB(taskA, "DateTimeCreated");

            Assert.IsFalse(taskA.IsEquivalentTo(taskB));
        }

        [TestMethod]
        public void TestNotEquivalant_Status()
        {
            Task taskA = GetTestTaskA();
            Task taskB = GetTestTaskB(taskA, "Status");

            Assert.IsFalse(taskA.IsEquivalentTo(taskB));
        }

        [TestMethod]
        public void TestNotEquivalant_ContextID()
        {
            Task taskA = GetTestTaskA();
            Task taskB = GetTestTaskB(taskA, "ContextID");

            Assert.IsFalse(taskA.IsEquivalentTo(taskB));
        }

        [TestMethod]
        public void TestNotEquivalant_DateDue()
        {
            Task taskA = GetTestTaskA();
            Task taskB = GetTestTaskB(taskA, "DateDue");

            Assert.IsFalse(taskA.IsEquivalentTo(taskB));
        }

        [TestMethod]
        public void TestNotEquivalant_DateStarted()
        {
            Task taskA = GetTestTaskA();
            Task taskB = GetTestTaskB(taskA, "DateStarted");

            Assert.IsFalse(taskA.IsEquivalentTo(taskB));
        }

        [TestMethod]
        public void TestNotEquivalant_DateCompleted()
        {
            Task taskA = GetTestTaskA();
            Task taskB = GetTestTaskB(taskA, "DateCompleted");

            Assert.IsFalse(taskA.IsEquivalentTo(taskB));
        }

        [TestMethod]
        public void TestNotEquivalant_Details()
        {
            Task taskA = GetTestTaskA();
            Task taskB = GetTestTaskB(taskA, "Details");

            Assert.IsFalse(taskA.IsEquivalentTo(taskB));
        }

        [TestMethod]
        public void TestNotEquivalant_UniqueID()
        {
            Task taskA = GetTestTaskA();
            Task taskB = GetTestTaskB(taskA, "UniqueID");

            Assert.IsFalse(taskA.IsEquivalentTo(taskB));
        }

        [TestMethod]
        public void TestNotEquivalant_Order()
        {
            Task taskA = GetTestTaskA();
            Task taskB = GetTestTaskB(taskA, "Order");

            Assert.IsFalse(taskA.IsEquivalentTo(taskB));
        }

        private static Task GetTestTaskA()
        {
            Task taskA = new Task("TestTask1");
            taskA.DateTimeCreated = DateTime.Parse("1/14/2016");
            taskA.Status = StormshrikeTODO.Model.Task.StatusEnum.Waiting;
            taskA.ContextID = "contextID1";
            taskA.DateDue = DateTime.Parse("1/15/2017");
            taskA.DateStarted = DateTime.Parse("1/15/2016");
            taskA.DateCompleted = DateTime.Parse("1/15/2018");
            taskA.Details = "Details 1";
            taskA.UniqueID = Guid.NewGuid();
            taskA.Order = 154;
            return taskA;
        }

        private static Task GetTestTaskB(Task taskA, string except)
        {
            Task taskB = new Task("TaskB");

            if (except != "Name")
            {
                taskB.Name = taskA.Name;
            }

            if (except != "DateTimeCreated")
            {
                taskB.DateTimeCreated = taskA.DateTimeCreated;
            }

            if (except != "Status")
            {
                taskB.Status = taskA.Status;
            }

            if (except != "ContextID")
            {
                taskB.ContextID = taskA.ContextID;
            }

            if (except != "DateDue")
            {
                taskB.DateDue = taskA.DateDue;
            }

            if (except != "DateStarted")
            {
                taskB.DateStarted = taskA.DateStarted;
            }

            if (except != "DateCompleted")
            {
                taskB.DateCompleted = taskA.DateCompleted;
            }

            if (except != "Details")
            {
                taskB.Details = taskA.Details;
            }

            if (except != "UniqueID")
            {
                taskB.UniqueID = taskA.UniqueID;
            }

            if (except != "Order")
            {
                taskB.Order = taskA.Order;
            }
            return taskB;
        }

        [TestMethod]
        public void TestDueDate()
        {
            Task task = new Task("TestTask2", DateTime.Parse("1/15/2016"));

            Assert.AreEqual(DateTime.Parse("1/15/2016"), task.DateDue);
            Assert.AreNotEqual("00000000-0000-0000-0000-000000000000", task.UniqueID.ToString());
        }

        /// <summary>
        /// Test the <code>Details</code> property
        /// </summary>
        [TestMethod]
        public void TestDetails()
        {
            String testDetails = "More detailed description";

            Task task = new Task("TestTask3", DateTime.Parse("1/15/2016"));

            Assert.AreEqual("", task.Details);

            task.Details = testDetails;

            Assert.AreEqual(testDetails, task.Details);
        }

        [TestMethod]
        public void TestToString()
        {
            Task task = new Task("TestTask4", DateTime.Parse("6/15/2016"));
            task.ContextID = "1234";
            task.Order = 5678;
            Assert.AreEqual("ID:{"
                   + task.UniqueID
                   + "},Order:{5678},Name:{TestTask4},DueDate:{6/15/2016},Details:{},Status:{NotStarted}," +
                   "ContextID:{1234},DateStarted:{},DateCompleted:{}",
                task.ToString());
        }

        [TestMethod]
        public void TestStartTask()
        {
            Task task = new Task("TestTask5", DateTime.Parse("7/15/2016"));
            task.StartTask();
            Assert.AreEqual(Task.StatusEnum.InProgress, task.Status);
            Assert.AreEqual(DateTime.Today, task.DateStarted.GetValueOrDefault().Date);
        }

        [TestMethod]
        public void TestSetContext()
        {
            Task task = new Task("GA", DateTime.Parse("6/20/2016"));
            task.ContextID = "1234";
            Assert.AreEqual("1234", task.ContextID);
        }

        [TestMethod]
        public void TestStatusInProgress()
        {
            Task task = new Task("TestTask6", DateTime.Parse("9/15/2016"));
            task.Status = Task.StatusEnum.InProgress;
            Assert.AreEqual(Task.StatusEnum.InProgress, task.Status);
            Assert.AreEqual(DateTime.Today.ToShortDateString(), task.DateStarted.Value.ToShortDateString());
        }

        [TestMethod]
        public void TestStatusInProgressDateStartedAlreadySet()
        {
            Task task = new Task("TestTask6", DateTime.Parse("9/15/2016"));
            task.DateStarted = DateTime.Parse("3/1/2017");
            task.Status = Task.StatusEnum.InProgress;
            Assert.AreEqual(Task.StatusEnum.InProgress, task.Status);
            Assert.AreEqual(DateTime.Parse("3/1/2017").ToShortDateString(),
                task.DateStarted.Value.ToShortDateString());
        }

        [TestMethod]
        public void TestStatusDone()
        {
            Task task = new Task("TestTask7", DateTime.Parse("9/15/2017"));
            task.Status = Task.StatusEnum.Done;
            Assert.AreEqual(Task.StatusEnum.Done, task.Status);
            Assert.AreEqual(DateTime.Today.ToShortDateString(), task.DateCompleted.Value.ToShortDateString());
        }

        [TestMethod]
        public void TestStatusDoneDateCompletedAlreadySet()
        {
            Task task = new Task("TestTask8", DateTime.Parse("10/15/2016"));
            task.DateCompleted = DateTime.Parse("3/2/2017");
            task.Status = Task.StatusEnum.Done;
            Assert.AreEqual(Task.StatusEnum.Done, task.Status);
            Assert.AreEqual(DateTime.Parse("3/2/2017").ToShortDateString(),
                task.DateCompleted.Value.ToShortDateString());
        }
    }
}
