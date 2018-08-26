using System.Data.SQLite;
using System;
using StormshrikeTODO.Model;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace StormshrikeTODO.Persistence
{
    public class SQLitePersistence : IPersistence, IDisposable
    {
        SQLiteConnectionStringBuilder _connStrBuilder;
        public SQLitePersistence(SQLitePersistenceConfig config)
        {
            if (!System.IO.File.Exists(config.DbFileLocation))
            {
                throw new ArgumentException("DB File does not exist: " + config.DbFileLocation);
            }

            _connStrBuilder = new SQLiteConnectionStringBuilder
            {
                DataSource = config.DbFileLocation,
                Version = 3,
                ForeignKeys = true
            };
        }

        public DefinedContexts LoadContexts()
        {
            DefinedContexts dc = new DefinedContexts();
            using (SQLiteConnection db = new SQLiteConnection(_connStrBuilder.ConnectionString))
            {
                db.Open();
                string sql = "SELECT * FROM Contexts";
                SQLiteCommand command = new SQLiteCommand(sql, db);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Context c = new Context(reader["ID"].ToString(), reader["Description"].ToString());
                    dc.Add(c);
                }
                db.Close();
            }

            return dc;
        }

        public Collection<Project> LoadProjects()
        {
            var prjList = new Collection<Project>();

            using (SQLiteConnection db = new SQLiteConnection(_connStrBuilder.ConnectionString))
            {
                db.Open();
                string sql = "SELECT * FROM Projects";
                SQLiteCommand command = new SQLiteCommand(sql, db);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    //Console.WriteLine("ID: " + reader["ID"] + "\tName: " + reader["Name"] + "\tDateTimeCreated: " + reader["DateTimeCreated"].ToString());
                    var prj = new Project(reader["Name"].ToString());
                    prj.UniqueID = Guid.Parse(reader["ID"].ToString());

                    if (!String.IsNullOrWhiteSpace(reader["DateDue"].ToString()))
                    {
                        prj.DueDate = ConvertDateTime(reader["DateDue"].ToString());
                    }

                    if (!String.IsNullOrWhiteSpace(reader["DateTimeCreated"].ToString()))
                    {
                        prj.DateTimeCreated = ConvertDateTime(reader["DateTimeCreated"].ToString());
                    }

                    //taskCollection.Where(t => t. == prj.UniqueID.ToString()).ToList().ForEach(t => prj.AddTask(t.Value));
                    var taskCollection = LoadTasks(prj.UniqueID.ToString());
                    foreach (var task in taskCollection)
                    {
                        prj.AddTask(task);

                    }

                    //Collection<Task> taskCollection = new Collection<Task>();


                    prjList.Add(prj);
                }
                db.Close();
            }

            return prjList;
        }

        private Collection<Task> LoadTasks(string prjID)
        {
            Collection<Task> taskCollection = new Collection<Task>();
            using (SQLiteConnection db = new SQLiteConnection(_connStrBuilder.ConnectionString))
            {
                db.Open();
                string sql = "SELECT * FROM Tasks WHERE Tasks.ProjectID = '" + prjID + "'" ;
                SQLiteCommand command = new SQLiteCommand(sql, db);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    //Console.WriteLine("ID: " + reader["ID"] + "\tName: " + reader["Name"] + "\tDateTimeCreated: " + reader["DateTimeCreated"].ToString());
                    Task t = new Task(reader["Name"].ToString(), ConvertDateTimeNullable(reader["DateDue"].ToString()));
                    t.Order = Convert.ToInt32(reader["TaskOrder"].ToString());
                    Enum.TryParse(reader["Status"].ToString(), out Task.StatusEnum stat);
                    t.Status = stat;
                    t.UniqueID = Guid.Parse(reader["ID"].ToString());
                    t.ContextID = reader["ContextID"].ToString();
                    t.DateCompleted = ConvertDateTimeNullable(reader["DateTimeCompleted"].ToString());
                    t.DateStarted = ConvertDateTimeNullable(reader["DateStarted"].ToString());
                    t.Details = reader["Details"].ToString();

                    if (!String.IsNullOrWhiteSpace(reader["DateTimeCreated"].ToString()))
                    {
                        t.DateTimeCreated = ConvertDateTime(reader["DateTimeCreated"].ToString());
                    }
                    //string prjID = reader["ProjectID"].ToString();
                    taskCollection.Add(t);
                }
                db.Close();
            }
            return taskCollection;
        }

        private DateTime ConvertDateTime(String dateTimeStr)
        {
            DateTime dtParsed;
            DateTime dt = DateTime.MinValue;
            bool parsed = DateTime.TryParse(dateTimeStr, out dtParsed);
            if (parsed)
            {
                dt = dtParsed;
            }
            return dt;
        }

        private DateTime? ConvertDateTimeNullable(String dateTimeStr)
        {
            DateTime dt;
            bool parsed = DateTime.TryParse(dateTimeStr, out dt);
            if (parsed)
            {
                return dt;

            }
            return null;
        }

        public void SaveContexts(DefinedContexts definedContexts)
        {
            throw new NotImplementedException();
        }

        public void SaveProjects(Collection<Project> projectList)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
