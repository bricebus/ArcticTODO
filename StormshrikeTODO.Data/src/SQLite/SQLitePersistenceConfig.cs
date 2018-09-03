using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StormshrikeTODO.Data
{
    public class SQLitePersistenceConfig
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public SQLitePersistenceConfig()
        {
            var dbDir = System.Environment.GetEnvironmentVariable("STORMSHRIKETODO_DB_DIR");
            var dbFile = System.Environment.GetEnvironmentVariable("STORMSHRIKETODO_DB_FILE");

            if (String.IsNullOrEmpty(dbDir))
            {
                log.Debug("STORMSHRIKETODO_DB_DIR not set. Using default.");
                dbDir = System.Environment.GetEnvironmentVariable("TEMP");
            }
            else
            {
                log.Debug("STORMSHRIKETODO_DB_DIR=" + dbDir);
            }

            if (String.IsNullOrEmpty(dbFile))
            {
                log.Debug("STORMSHRIKETODO_DB_FILE not set. Using default.");
                dbFile = "Stormshrike.db";
            }
            else
            {
                log.Debug("STORMSHRIKETODO_DB_FILE=" + dbFile);
            }

            DbFileLocation = dbDir + "\\" + dbFile;
        }

        public SQLitePersistenceConfig(string dbLocation)
        {
            DbFileLocation = dbLocation;
        }

        //public string DbFileLocation { get; private set; } = System.Environment.GetEnvironmentVariable("TEMP") + "\\Stormshrike.db";
        public string DbFileLocation { get; private set; } = "";
    }
}
