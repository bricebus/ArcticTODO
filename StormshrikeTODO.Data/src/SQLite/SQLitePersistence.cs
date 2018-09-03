using System.Data.SQLite;
using System;
using StormshrikeTODO.Model;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using log4net;

namespace StormshrikeTODO.Data
{
    public class SQLitePersistence : IPersistence, IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        SQLiteConnectionStringBuilder _connStrBuilder;
        public SQLitePersistence(SQLitePersistenceConfig config)
        {
            log.Info("DB File: " + config.DbFileLocation);
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
            log.Info("Loading Contexts...");
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

            log.Info("Done loading Contexts");

            return dc;
        }

        public Collection<Project> LoadProjects()
        {
            log.Info("Loading Projects...");

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
                    var prj = new Project(reader["Name"].ToString())
                    {
                        UniqueID = Guid.Parse(reader["ID"].ToString())
                    };

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

            log.Info("Done loading Projects");
            return prjList;
        }

        public Collection<Task> LoadTasks(string prjID)
        {
            log.Debug("Loading Tasks for Project: " + prjID);

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
            log.Debug("Done loading Tasks for Project: " + prjID);
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
            log.Info("Saving Contexts");
            DefinedContexts dcDB = LoadContexts();
            if (DefinedContexts.IdentifyDifferences(dcDB, dcNew, out List<Context> newList,
                out List<Context> chgList, out List<Context> delList))
            {
                using (SQLiteConnection db = new SQLiteConnection(_connStrBuilder.ConnectionString))
                {
                    db.Open();
                    foreach (var changedItem in chgList)
                    {
                        SQLiteCommand cmd = BuildChangedContextSQL(changedItem, db);
                        cmd.ExecuteNonQuery();
                    }

                    foreach (var deletedItem in delList)
                    {
                        SQLiteCommand cmd = BuildDeletedContextSQL(deletedItem, db);
                        cmd.ExecuteNonQuery();
                    }

                    foreach (var newItem in newList)
                    {
                        SQLiteCommand cmd = BuildNewContextSQL(newItem, db);
                        cmd.ExecuteNonQuery();
                    }
                    db.Close();
                }
            }
            log.Info("Done saving Contexts");
        }

        public void SaveProjects(Collection<Project> prjListNew)
        {
            log.Info("Saving Projects");
            var prjListFromDB = new Collection<Project>();
            using (SQLiteConnection db = new SQLiteConnection(_connStrBuilder.ConnectionString))
            {
                prjListFromDB = LoadProjects();
            }

            if (!ProjectComparer.AreListsEquivalent(prjListFromDB, prjListNew,
                out List<Project> chgList, out List<Project> addList, out List<Project> delList,
                out List<Project> chgTaskList))
            {
                using (SQLiteConnection db = new SQLiteConnection(_connStrBuilder.ConnectionString))
                {
                    db.Open();
                    foreach (var changedPrj in chgList)
                    {
                        SQLiteCommand cmd = BuildChangedProjectSQL(changedPrj, db);
                        cmd.ExecuteNonQuery();
                    }

                    foreach (var deletedPrj in delList)
                    {
                        DeleteAttachedTasks(deletedPrj, db);
                        SQLiteCommand cmd = BuildDeleteProjectSQL(deletedPrj, db);
                        cmd.ExecuteNonQuery();
                    }

                    foreach (var addedPrj in addList)
                    {
                        SQLiteCommand cmd = BuildAddProjectSQL(addedPrj, db);
                        cmd.ExecuteNonQuery();
                        AddAttachedTasks(addedPrj.TaskList, addedPrj.UniqueID, db);
                    }

                    //changed tasks
                    foreach (var prj in chgTaskList)
                    {
                        var prjFromDB = prjListFromDB.First(p => p.UniqueID == prj.UniqueID);
                        if (!new TaskComparer().AreListsEquivalent(prjFromDB.TaskList.ToList(),
                            prj.TaskList.ToList(), out List<Task> chgTaskListPerPrj,
                            out List<Task> addTaskList, out List<Task> delTaskList))
                        {
                            foreach (var changedTask in chgTaskListPerPrj)
                            {
                                SQLiteCommand cmd = BuildChangedTaskSQL(changedTask,
                                    prj.UniqueID.ToString(), db);
                                cmd.ExecuteNonQuery();
                            }

                            foreach (var addedTask in addTaskList)
                            {
                                SQLiteCommand cmd = BuildAddTaskSQL(addedTask,
                                    prj.UniqueID.ToString(), db);
                                cmd.ExecuteNonQuery();
                            }
                            foreach (var deletedTask in delTaskList)
                            {
                                SQLiteCommand cmd = BuildDeleteTaskSQL(deletedTask, db);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    db.Close();
                }
            }
            log.Info("Done saving Projects");
        }

        private SQLiteCommand BuildChangedTaskSQL(Task changedTask, string projectID, SQLiteConnection db)
        {
            string sql = "UPDATE Tasks SET " +
                "Name = @Name," +
                "Status = @Status," +
                "ProjectID = @ProjectID," +
                "TaskOrder = @TaskOrder," +
                "Details = @Details," +
                "DateStarted = @DateStarted," +
                "DateTimeCompleted = @DateTimeCompleted," +
                "DateDue = @DateDue," +
                "ContextID = @ContextID," +
                "DateTimeCreated = @DateTimeCreated" +
                " WHERE ID = @ID";
            SQLiteCommand command = new SQLiteCommand(sql, db);
            command.Parameters.AddWithValue("@Name", changedTask.Name);
            command.Parameters.AddWithValue("@Status", changedTask.Status);
            command.Parameters.AddWithValue("@ProjectID", projectID);
            command.Parameters.AddWithValue("@TaskOrder", changedTask.Order);
            command.Parameters.AddWithValue("@Details", changedTask.Details);
            command.Parameters.AddWithValue("@DateStarted", changedTask.DateStarted);
            command.Parameters.AddWithValue("@DateTimeCompleted", changedTask.DateCompleted);
            command.Parameters.AddWithValue("@DateDue", changedTask.DateDue);
            command.Parameters.AddWithValue("@ContextID", changedTask.ContextID);
            command.Parameters.AddWithValue("@DateTimeCreated", changedTask.DateTimeCreated);
            command.Parameters.AddWithValue("@ID", changedTask.UniqueID.ToString());

            return command;
        }

        private void AddAttachedTasks(Collection<Task> taskList, Guid prjID, SQLiteConnection db)
        {
            foreach (var newTask in taskList)
            {
                SQLiteCommand cmd = BuildAddTaskSQL(newTask, prjID.ToString(), db);
                cmd.ExecuteNonQuery();
            }
        }

        private SQLiteCommand BuildAddTaskSQL(Task addedTask, String projectID, SQLiteConnection db)
        {
            string sql = "INSERT INTO Tasks VALUES(@ID, @Name, @Status, @ProjectID, " +
                "@TaskOrder, @Details, @DateStarted, @DateTimeCompleted, @DateDue, " +
                "@ContextID, @DateTimeCreated)";
            SQLiteCommand command = new SQLiteCommand(sql, db);
            command.Parameters.AddWithValue("@ID", addedTask.UniqueID.ToString());
            command.Parameters.AddWithValue("@Name", addedTask.Name);
            command.Parameters.AddWithValue("@Status", addedTask.Status);
            command.Parameters.AddWithValue("@ProjectID", projectID);
            command.Parameters.AddWithValue("@TaskOrder", addedTask.Order);
            command.Parameters.AddWithValue("@Details", addedTask.Details);
            command.Parameters.AddWithValue("@DateStarted", addedTask.DateStarted);
            command.Parameters.AddWithValue("@DateTimeCompleted", addedTask.DateCompleted);
            command.Parameters.AddWithValue("@DateDue", addedTask.DateDue);
            command.Parameters.AddWithValue("@ContextID", addedTask.ContextID);
            command.Parameters.AddWithValue("@DateTimeCreated", addedTask.DateTimeCreated);

            return command;
        }

        private SQLiteCommand BuildAddProjectSQL(Project addedPrj, SQLiteConnection db)
        {
            string sql = "INSERT INTO Projects VALUES(@ID, @Name, @DateDue, @DateTimeCreated)";
            SQLiteCommand command = new SQLiteCommand(sql, db);
            command.Parameters.AddWithValue("@ID", addedPrj.UniqueID.ToString());
            command.Parameters.AddWithValue("@Name", addedPrj.ProjectName);
            command.Parameters.AddWithValue("@DateDue", addedPrj.DueDate);
            command.Parameters.AddWithValue("@DateTimeCreated", addedPrj.DateTimeCreated);

            return command;
        }

        private void DeleteAttachedTasks(Project deletedPrj, SQLiteConnection db)
        {
            SQLiteCommand cmd = BuildDeleteTaskSQLByProject(deletedPrj, db);
            cmd.ExecuteNonQuery();
        }

        private SQLiteCommand BuildDeleteTaskSQLByProject(Project deletedPrj, SQLiteConnection db)
        {
            string sql = "DELETE FROM Tasks " +
                " WHERE ProjectID = @ProjectID";
            SQLiteCommand command = new SQLiteCommand(sql, db);
            command.Parameters.AddWithValue("@ProjectID", deletedPrj.UniqueID.ToString());

            return command;
        }

        private SQLiteCommand BuildDeleteTaskSQL(Task deletedTask, SQLiteConnection db)
        {
            string sql = "DELETE FROM Tasks " +
                " WHERE ID = @ID";
            SQLiteCommand command = new SQLiteCommand(sql, db);
            command.Parameters.AddWithValue("@ID", deletedTask.UniqueID.ToString());

            return command;
        }

        private SQLiteCommand BuildDeleteProjectSQL(Project prj, SQLiteConnection db)
        {
            string sql = "DELETE FROM Projects " +
                " WHERE ID = @ID";
            SQLiteCommand command = new SQLiteCommand(sql, db);
            command.Parameters.AddWithValue("@ID", prj.UniqueID.ToString());

            return command;
        }

        public void Dispose()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private SQLiteCommand BuildChangedProjectSQL (Project prj, SQLiteConnection db)
        {
            string sql = "UPDATE Projects SET " +
                "Name = @Name," +
                "DateDue = @DateDue," +
                "DateTimeCreated = @DateTimeCreated" +
                " WHERE ID = @ID";
            SQLiteCommand command = new SQLiteCommand(sql, db);
            command.Parameters.AddWithValue("@ID", prj.UniqueID.ToString());
            command.Parameters.AddWithValue("@Name", prj.ProjectName);
            command.Parameters.AddWithValue("@DateDue", prj.DueDate);
            command.Parameters.AddWithValue("@DateTimeCreated", prj.DateTimeCreated);

            return command;
        }

        private SQLiteCommand BuildChangedContextSQL (Context ctx, SQLiteConnection db)
        {
            string sql = "UPDATE Contexts SET Description = @Description WHERE Contexts.ID = @ID";
            SQLiteCommand command = new SQLiteCommand(sql, db);
            command.Parameters.AddWithValue("@ID", ctx.ID);
            command.Parameters.AddWithValue("@Description", ctx.Description);

            return command;
        }

        private SQLiteCommand BuildNewContextSQL(Context ctx, SQLiteConnection db)
        {
            string sql = "INSERT INTO Contexts VALUES(@ID, @Description)";
            SQLiteCommand command = new SQLiteCommand(sql, db);
            command.Parameters.AddWithValue("@ID", ctx.ID);
            command.Parameters.AddWithValue("@Description", ctx.Description);

            return command;
        }

        private SQLiteCommand BuildDeletedContextSQL (Context ctx, SQLiteConnection db)
        {
            string sql = "DELETE FROM Contexts WHERE Contexts.ID = @ID";
            SQLiteCommand command = new SQLiteCommand(sql, db);
            command.Parameters.AddWithValue("@ID", ctx.ID);

            return command;
        }
    }
}
