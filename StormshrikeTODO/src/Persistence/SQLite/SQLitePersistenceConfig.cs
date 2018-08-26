using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StormshrikeTODO.Persistence
{
    public class SQLitePersistenceConfig
    {
        public SQLitePersistenceConfig()
        {
        }

        public SQLitePersistenceConfig(string dbLocation)
        {
            DbFileLocation = dbLocation;
        }

        public string DbFileLocation { get; private set; } = System.Environment.GetEnvironmentVariable("TEMP") + "\\Stormshrike.db";
    }

}
