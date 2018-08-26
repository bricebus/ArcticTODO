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

                    var taskCollection = LoadTasks(prj.UniqueID.ToString());
                    foreach (var task in taskCollection)
                    {
                        prj.AddTask(task);
                    }

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
                string sql = "SELECT * FROM Tasks WHERE Tasks.ProjectID = @prjID";
                SQLiteCommand command = new SQLiteCommand(sql, db);
                command.Parameters.AddWithValue("@prjID", prjID);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    //Console.WriteLine("ID: " + reader["ID"] + "\tName: " + reader["Name"] + "\tDateTimeCreated: " + reader["DateTimeCreated"].ToString());
                    Task t = new Task(reader["Name"].ToString(), ConvertDateTimeNullable(reader["DateDue"].ToString()))
                    {
                        Order = Convert.ToInt32(reader["TaskOrder"].ToString()),
                        UniqueID = Guid.Parse(reader["ID"].ToString()),
                        ContextID = reader["ContextID"].ToString(),
                        DateCompleted = ConvertDateTimeNullable(reader["DateTimeCompleted"].ToString()),
                        Details = reader["Details"].ToString(),
                        Status = GetStatusEnum(reader["Status"].ToString())
                    };

                    if (!String.IsNullOrWhiteSpace(reader["DateTimeCreated"].ToString()))
                    {
                        t.DateTimeCreated = ConvertDateTime(reader["DateTimeCreated"].ToString());
                    }

                    if (!String.IsNullOrWhiteSpace(reader["DateStarted"].ToString()))
                    {
                        t.DateStarted = ConvertDateTimeNullable(reader["DateStarted"].ToString());
                    }
                    else
                    {
                        t.DateStarted = null;
                    }

                    taskCollection.Add(t);
                }
                db.Close();
            }
            return taskCollection;
        }

        private Task.StatusEnum GetStatusEnum(String statusStr)
        {
            Enum.TryParse(statusStr, out Task.StatusEnum stat);
            return stat;
        }

        private DateTime ConvertDateTime(String dateTimeStr)
        {
            return DateTime.TryParse(dateTimeStr, out DateTime dtParsed) ? dtParsed : DateTime.MinValue;
        }

        private DateTime? ConvertDateTimeNullable(String dateTimeStr)
        {
            DateTime? dtNull = null;
            return (DateTime.TryParse(dateTimeStr, out DateTime dt) ? dt : dtNull);
        }

        public void SaveContexts(DefinedContexts dcNew)
        {
            int changedCount = 0;
            DefinedContexts dcDB = LoadContexts();
            if (DefinedContexts.IdentifyDifferences(dcDB, dcNew, out List<Context> newList,
                out List<Context> chgList, out List<Context> delList))
            {
                using (SQLiteConnection db = new SQLiteConnection(_connStrBuilder.ConnectionString))
                {
                    db.Open();
                    foreach (var changedItem in chgList)
                    {
                        SQLiteCommand cmd = buildChangedContextSQL(changedItem, db);
                        changedCount += cmd.ExecuteNonQuery();
                    }

                    foreach (var deletedItem in delList)
                    {
                        SQLiteCommand cmd = buildDeletedContextSQL(deletedItem, db);
                        changedCount += cmd.ExecuteNonQuery();
                    }

                    foreach (var newItem in newList)
                    {
                        SQLiteCommand cmd = buildNewContextSQL(newItem, db);
                        changedCount += cmd.ExecuteNonQuery();
                    }
                }

            }
        }

        private SQLiteCommand buildChangedContextSQL (Context ctx, SQLiteConnection db)
        {
            string sql = "UPDATE Contexts SET Description = @Description WHERE Contexts.ID = @ID";
            SQLiteCommand command = new SQLiteCommand(sql, db);
            command.Parameters.AddWithValue("@ID", ctx.ID);
            command.Parameters.AddWithValue("@Description", ctx.Description);

            return command;
        }

        private SQLiteCommand buildNewContextSQL(Context ctx, SQLiteConnection db)
        {
            string sql = "INSERT INTO Contexts VALUES(@ID, @Description)";
            SQLiteCommand command = new SQLiteCommand(sql, db);
            command.Parameters.AddWithValue("@ID", ctx.ID);
            command.Parameters.AddWithValue("@Description", ctx.Description);

            return command;
        }

        private SQLiteCommand buildDeletedContextSQL (Context ctx, SQLiteConnection db)
        {
            string sql = "DELETE FROM Contexts WHERE Contexts.ID = @ID";
            SQLiteCommand command = new SQLiteCommand(sql, db);
            command.Parameters.AddWithValue("@ID", ctx.ID);

            return command;
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
