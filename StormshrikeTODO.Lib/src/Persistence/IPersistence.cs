using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StormshrikeTODO.Model;
using System.Collections.ObjectModel;

namespace StormshrikeTODO.Persistence
{
    public interface IPersistence
    {
        Collection<Project> LoadProjects();
        DefinedContexts LoadContexts();
        void SaveProjects(Collection<Project> projectList);
        void SaveContexts(DefinedContexts definedContexts);
    }
}
