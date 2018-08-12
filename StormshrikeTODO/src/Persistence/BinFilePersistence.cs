using StormshrikeTODO.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;

namespace StormshrikeTODO.Persistence
{
    public class BinFilePersistence : IPersistence
    {
        /// <summary>
        /// The File that will be written to/from
        /// </summary>
        public String ProjectPersistenceFile { get; private set; }
        public String ContextPersistenceFile { get; private set; }

        public BinFilePersistence(BinFilePersistenceConfig config)
        {
            this.ProjectPersistenceFile = config.ProjectFileLocation;
            this.ContextPersistenceFile = config.ContextFileLocation;
        }

        public void CreateContextFile()
        {
            using (Stream stream = File.Open(this.ContextPersistenceFile, FileMode.Create))
            {
                BinaryFormatter bin = new BinaryFormatter();
                bin.Serialize(stream, new DefinedContexts());
            }

        }

        public void CreateProjectFile()
        {
            Collection<Project> emptyPrjList = new Collection<Project>();
            using (Stream stream = File.Open(this.ProjectPersistenceFile, FileMode.Create))
            {
                BinaryFormatter bin = new BinaryFormatter();
                bin.Serialize(stream, emptyPrjList);
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
                BinaryFormatter bin = new BinaryFormatter();
                return (Collection<Project>) bin.Deserialize(stream);
            }
        }

        public void SaveProjects(Collection<Project> projectList)
        {
            using (Stream stream = File.Open(this.ProjectPersistenceFile, FileMode.Create))
            {
                BinaryFormatter bin = new BinaryFormatter();
                bin.Serialize(stream, projectList);
            }
        }

        public DefinedContexts LoadContexts()
        {
            if (!File.Exists(this.ContextPersistenceFile))
            {
                CreateContextFile();
            }

            using (Stream stream = File.Open(this.ContextPersistenceFile, FileMode.Open))
            {
                BinaryFormatter bin = new BinaryFormatter();
                return (DefinedContexts) bin.Deserialize(stream);
            }
        }

        public void SaveContexts(DefinedContexts definedContexts)
        {
            using (Stream stream = File.Open(this.ContextPersistenceFile, FileMode.Create))
            {
                BinaryFormatter bin = new BinaryFormatter();
                bin.Serialize(stream, definedContexts);
            }
        }
    }
}
