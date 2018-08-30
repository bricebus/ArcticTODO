using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StormshrikeTODO.Model.Tests
{
    public class TestTaskCreator
    {
        public static Task[] SetUpThreeTasks()
        {
            var taskArray = new Task[3];

            taskArray[0] = new Task("A New Task")
            {
                UniqueID = Guid.Parse("4541b261-9a28-4872-9e8a-878aa1bf5eb5"),
                Order = 1000
            };

            taskArray[1] = new Task("Another New Task")
            {
                UniqueID = Guid.Parse("223a74ea-04b1-4d23-8938-63062c184b03"),
                Order = 2000
            };

            taskArray[2] = new Task("And ANOTHER New Task")
            {
                UniqueID = Guid.Parse("8fbb8fc5-2b61-4dd7-ad39-c100b8473f0f"),
                Order = 3000
            };

            return taskArray;
        }

        public static List<Task> SetUpThreeTasksList()
        {
            return TestTaskCreator.SetUpThreeTasks().ToList();

        }
    }
}
