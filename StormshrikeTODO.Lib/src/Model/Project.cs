using System;
using System.Collections.Generic;
using System.Linq;
using StormshrikeTODO.Model.Util;
using System.Collections.ObjectModel;

namespace StormshrikeTODO.Model
{
    [Serializable]
    public class Project
    {
        private Collection<Task> _taskList = new Collection<Task>();
        public Collection<Task> TaskList
        {
            get
            {
                return _taskList;
            }
        }

        public Guid UniqueID { get; set; }

        public string ProjectName { get; set; }

        public DateTime? DueDate { get; set; }

        // Needed to mock
        public Project()
        {

        }

        public Project(string prjName, DateTime? dueDate)
            : this(prjName)
        {
            DueDate = dueDate;
        }

        public Project(string prjName)
        {
            ProjectName = prjName;
            this.UniqueID = Guid.NewGuid();
        }

        public void AddTask(Task task)
        {
            _taskList.Add(task);
        }

        public bool HasTasks
        {
            get
            {
                return _taskList.Count > 0;
            }
        }

        public int TaskCount
        {
            get
            {
                return _taskList.Count;
            }
        }

        public override string ToString()
        {
            return String.Format("ID:{{{0}}},Name:{{{1}}},DueDate:{{{2}}},TaskCount:{{{3}}}",
                this.UniqueID, this.ProjectName, Utility.GetDateTimeString(this.DueDate), _taskList.Count);
        }

        public Task GetNextTask()
        {
            if (_taskList.Count == 0)
            {
                return null;
            }
            return _taskList.OrderBy(t => t.Order).First();
        }

        /// <summary>
        /// Return a list of all Tasks for the project; default order
        /// </summary>
        /// <returns></returns>
        public Collection<Task> GetTaskList()
        {
            Collection<Task> taskListToReturn = new Collection<Task>();
            foreach (var t in _taskList)
            {
               taskListToReturn.Add(t);
            }

            return taskListToReturn;
        }

        public void InsertTaskAfter(int pos, Task task)
        {
            if (pos <= 0)
            {
                throw new ArgumentException("Position too small", "Position");
            }
            else if (pos > _taskList.Count)
            {
                throw new ArgumentException("Position too large", "Position");
            }
            else if (task == null)
            {
                throw new ArgumentException("Task is null", "Task");
            }

            _taskList.Insert(pos, task);
        }

        public void InsertTaskAfter(Guid taskID, Task task)
        {
            if (task == null)
            {
                throw new ArgumentException("Task is null!", "Task");
            }

            Task current = null;
            int idx = 0;
            for (int i = 0; i < _taskList.Count; i++)
            {
                if (_taskList[i].UniqueID == taskID)
                {
                    current = _taskList[i];
                    idx = i;
                    break;
                }
            }

            if (current == null)
            {
                throw new ArgumentException("Cannot find TaskID!", "TaskID");
            }

            _taskList.Insert(idx + 1, task);
        }

        public Task GetTask(Guid taskID)
        {
            return _taskList.FirstOrDefault(t => t.UniqueID == taskID);
        }

        public Task GetTask(String taskIdStr)
        {
            Guid taskId;
            var parseSuccessful = Guid.TryParse (taskIdStr, out taskId);
            if (!parseSuccessful)
            {
                return null;

            }
            return GetTask(taskId);
        }

        // Make OrderTasks() virtual so that Moq can verify that it is called in SessionTest.
        public virtual void OrderTasks()
        {
            IEnumerable<Task> query = _taskList.OrderBy(task => task.Order);

            var order = 1000;

            foreach (Task t in query)
            {
                t.Order = order;
                order += 1000;
            }
        }

        public void MoveTaskFirst(string taskIdStr)
        {
            var task = GetTask(taskIdStr);
            if (task == null)
            {
                throw new ArgumentException("Cannot find TaskID!", "TaskID");
            }
            task.Order = 0;
            OrderTasks();
        }

        public void MoveTaskUp(string taskIdStr)
        {
            var task = GetTask(taskIdStr);
            if (task == null)
            {
                throw new ArgumentException("Cannot find TaskID!", "TaskID");
            }
            task.Order -= 1500;
            OrderTasks();
        }

        public void MoveTaskDown(string taskIdStr)
        {
            var task = GetTask(taskIdStr);
            if (task == null)
            {
                throw new ArgumentException("Cannot find TaskID!", "TaskID");
            }
            task.Order += 1500;
            OrderTasks();
        }


        public void MoveTaskLast(string taskIdStr)
        {
            var task = GetTask(taskIdStr);
            if (task == null)
            {
                throw new ArgumentException("Cannot find TaskID!", "TaskID");
            }
            task.Order = Int32.MaxValue;
            OrderTasks();
        }
    }
}
