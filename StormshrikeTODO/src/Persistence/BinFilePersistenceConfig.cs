using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StormshrikeTODO.Persistence
{
    public class BinFilePersistenceConfig
    {
        public BinFilePersistenceConfig()
        {
        }

        public BinFilePersistenceConfig(string prjFileLocation, string ctxFileLocation)
        {
            ProjectFileLocation = prjFileLocation;
            ContextFileLocation = ctxFileLocation;
        }

        public string ProjectFileLocation { get; private set; } = "d:\\extra\\Dropbox\\StormshrikeTODO-prj.bin";
        public string ContextFileLocation { get; private set; } = "d:\\extra\\Dropbox\\StormshrikeTODO-ctx.bin";
    }

}
