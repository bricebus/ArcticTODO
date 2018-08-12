using System;
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

            Assert.AreEqual("TestTask1", task.Name);
            Assert.IsFalse(task.DateDue.HasValue);
            Assert.IsFalse(task.DateStarted.HasValue);
            Assert.IsNotNull(task.UniqueID);
            Assert.AreNotEqual("00000000-0000-0000-0000-000000000000", task.UniqueID.ToString());
            Assert.AreEqual(Task.StatusEnum.NotStarted, task.Status);
            Assert.AreEqual("", task.ContextID);
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
        public void TestClone()
        {
            Task task = new Task("TestTask3", DateTime.Parse("1/15/2016"));
            task.Details = "More details here!";
            Task taskClone = Task.DeepClone(task);

            Assert.AreNotEqual(task, taskClone);
            Assert.AreEqual(task.Name, taskClone.Name);
            Assert.AreEqual(task.UniqueID, taskClone.UniqueID);
            Assert.AreEqual(task.DateDue, taskClone.DateDue);
            Assert.AreEqual(task.Details, taskClone.Details);

        }

        [TestMethod]
        public void TestToString()
        {
            Task task = new Task("TestTask4", DateTime.Parse("6/15/2016"));
            task.ContextID = "1234";
            Assert.AreEqual("ID:{"
                   + task.UniqueID
                   + "},Name:{TestTask4},DueDate:{6/15/2016},Details:{},Status:{NotStarted}," +
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
