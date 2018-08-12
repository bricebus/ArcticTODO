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

        internal Project()
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
            _taskList.Add(Task.DeepClone(task));
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
            return Task.DeepClone(_taskList.First());
        }

        public Collection<Task> GetTaskList()
        {
            Collection<Task> taskListToReturn = new Collection<Task>();
            _taskList.ToList().ForEach(t => taskListToReturn.Add(Task.DeepClone(t)));

            return taskListToReturn;
        }

        public void InsertAfter(int pos, Task task)
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

        public void InsertAfter(Guid taskID, Task task)
        {
            if (task == null)
            {
                throw new ArgumentException("Task is null!", "Task");
            }

            var newTask = Task.DeepClone(task);

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

            _taskList.Insert(idx + 1, newTask);
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
    }
}
