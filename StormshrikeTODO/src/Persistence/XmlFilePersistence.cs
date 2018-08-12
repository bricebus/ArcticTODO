using StormshrikeTODO.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using StormshrikeTODO.Model;
using System.IO;
using System.Collections.ObjectModel;

namespace StormshrikeTODO.Persistence
{
    public class XmlFilePersistence : IPersistence
    {
        /// <summary>
        /// The File that will be written to/from
        /// </summary>
        public String ProjectPersistenceFile { get; private set; }
        public String ContextPersistenceFile { get; private set; }

        public XmlFilePersistence(XmlFilePersistenceConfig xmlFilePersistenceConfig)
        {
            this.ProjectPersistenceFile = xmlFilePersistenceConfig.ProjectFileLocation;
            this.ContextPersistenceFile = xmlFilePersistenceConfig.ContextFileLocation;
        }

        public DefinedContexts LoadContexts()
        {
            if (!File.Exists(this.ContextPersistenceFile))
            {
                CreateContextFile();
            }

            using (Stream stream = File.Open(this.ContextPersistenceFile, FileMode.Open))
            {
                XmlSerializer s = new XmlSerializer(typeof(Collection<Context>));
                Collection<Context> contextColleciton = (Collection<Context>) s.Deserialize(stream);
                DefinedContexts dc = new DefinedContexts();
                foreach (var context in contextColleciton)
                {
                    dc.Add(context);
                }
                return dc;
            }
        }

        public Collection<Project> LoadProjects()
        {
            if (!File.Exists(this.ProjectPersistenceFile))
            {
                CreateProjectFile();
            }

            using (Stream stream = File.Open(this.ProjectPersistenceFile, FileMode.Open))
            {
                XmlSerializer s = new XmlSerializer(typeof(Collection<Project>));
                return (Collection<Project>) s.Deserialize(stream);
            }
        }

        public void SaveContexts(DefinedContexts dc)
        {
            using (Stream stream = File.Open(this.ContextPersistenceFile, FileMode.Create))
            {
                XmlSerializer s = new XmlSerializer(typeof(Collection<Context>));
                Collection<Context> contextCollection = new Collection<Context>(dc.GetList());
                s.Serialize(stream, contextCollection);
            }
        }

        public void SaveProjects(Collection<Project> projectList)
        {
            using (Stream stream = File.Open(this.ProjectPersistenceFile, FileMode.Create))
            {
                XmlSerializer s = new XmlSerializer(typeof(Collection<Project>));
                s.Serialize(stream, projectList);
            }
        }

        public void CreateContextFile()
        {
            using (Stream stream = File.Open(this.ContextPersistenceFile, FileMode.Create))
            {
                XmlSerializer s = new XmlSerializer(typeof(Collection<Context>));
                s.Serialize(stream, new Collection<Context>());
            }
        }

        public void CreateProjectFile()
        {
            Collection<Project> emptyPrjList = new Collection<Project>();
            using (Stream stream = File.Open(this.ProjectPersistenceFile, FileMode.Create))
            {
                XmlSerializer s = new XmlSerializer(typeof(Collection<Project>));
                s.Serialize(stream, emptyPrjList);
            }
        }
    }
}
