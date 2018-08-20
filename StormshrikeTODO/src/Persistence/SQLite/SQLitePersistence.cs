using System.Data.SQLite;
using System;
using StormshrikeTODO.Model;
using System.Collections.ObjectModel;

namespace StormshrikeTODO.Persistence
{
    public class SQLitePersistence : IPersistence
    {
        SQLiteConnectionStringBuilder _connStrBuilder;
        public SQLitePersistence(SQLitePersistenceConfig config)
        {
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
                SQLiteCommand xcmd = new SQLiteCommand();

                string sql = "SELECT * FROM Contexts";
                SQLiteCommand command = new SQLiteCommand(sql, db);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Context c = new Context(reader["ID"].ToString(), reader["Description"].ToString());
                    dc.Add(c);
                    //Console.WriteLine("ID: " + reader["ID"] + "\tDescription: " + reader["Description"]);
                }
            }

            return dc;
        }

        public Collection<Project> LoadProjects()
        {
            return new Collection<Project>();
        }

        public void SaveContexts(DefinedContexts definedContexts)
        {
            throw new NotImplementedException();
        }

        public void SaveProjects(Collection<Project> projectList)
        {
            throw new NotImplementedException();
        }
    }
    public partial class Contexts
    {
        public String ID { get; set; }
        public String Description { get; set; }
        
    }
    
    public partial class Projects
    {
        public String ID { get; set; }
        public String Name { get; set; }
        public String DateDue { get; set; }
        public String DateTimeCreated { get; set; }
        
    }
    
    public partial class Tasks
    {
        public String ID { get; set; }
        public String Name { get; set; }
        public String Status { get; set; }
        public String ProjectID { get; set; }
        public String DateStarted { get; set; }
        public String DateTimeCompleted { get; set; }
        public String DateDue { get; set; }
        public String ContextID { get; set; }
        public String DateTimeCreated { get; set; }
        
    }
    
}
