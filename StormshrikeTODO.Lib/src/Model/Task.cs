using StormshrikeTODO.Model.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace StormshrikeTODO.Model
{
    [Serializable]
    public class Task
    {
        public enum StatusEnum
        {
            New,
            NotStarted,
            InProgress,
            Waiting,
            Done
        }

        public String ContextID { get; set; }

        public DateTime? DateDue { get; set; }
        public DateTime? DateStarted { get; set; }
        public DateTime? DateCompleted { get; set; }
        public String Name { get; set; }

        private StatusEnum _status;
        public StatusEnum Status
        {
            get
            {
                return _status;
            }
            set
            {
                if (value == StatusEnum.InProgress && this.DateStarted == null)
                {
                    this.DateStarted = DateTime.Now;
                }
                else if (value == StatusEnum.Done && this.DateCompleted == null)
                {
                    this.DateCompleted = DateTime.Now;
                }
                _status = value;
            }
        }

        /// <summary>
        /// Optional longer description of the Task
        /// </summary>
        public String Details { get; set; }

        public Guid UniqueID { get; set; }

        /// <summary>
        /// Used to sort the Tasks
        /// </summary>
        public int Order { get; set; }

        internal Task()
        {

        }

        public Task(String name)
        {
            this.Name = name;
            this.Details = "";
            this.UniqueID = Guid.NewGuid();
            this.Status = StatusEnum.NotStarted;
            this.ContextID = "";
        }

        public Task(String name, DateTime? dueDate) : this(name)
        {
            this.DateDue = dueDate;
        }

        public static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }

        public override String ToString()
        {
            return String.Format(
                "ID:{{{0}}},Order:{{{8}}},Name:{{{1}}},DueDate:{{{2}}},Details:{{{3}}},Status:{{{4}}},ContextID:{{{5}}}" +
                ",DateStarted:{{{6}}},DateCompleted:{{{7}}}",
                this.UniqueID, this.Name, Utility.GetDateTimeString(this.DateDue), this.Details,
                this.Status, this.ContextID, Utility.GetDateTimeString(this.DateStarted),
                Utility.GetDateTimeString(this.DateCompleted), this.Order);
        }


        public void StartTask()
        {
            this.Status = StatusEnum.InProgress;
            this.DateStarted = DateTime.Now;
        }
    }
}
