using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StormshrikeTODO.Data
{
    public class XmlFilePersistenceConfig
    {
        public XmlFilePersistenceConfig()
        {
        }

        public XmlFilePersistenceConfig(string prjFileLocation, string ctxFileLocation)
        {
            ProjectFileLocation = prjFileLocation;
            ContextFileLocation = ctxFileLocation;
        }

        public string ProjectFileLocation { get; private set; } = "d:\\extra\\Dropbox\\StormshrikeTODO-prj.xml";
        public string ContextFileLocation { get; private set; } = "d:\\extra\\Dropbox\\StormshrikeTODO-ctx.xml";
    }

}
