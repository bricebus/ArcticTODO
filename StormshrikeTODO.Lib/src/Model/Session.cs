using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using log4net;

namespace StormshrikeTODO.Model
{
    public class Session
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IPersistence _persistence;

        private Collection<Project> _projectList;

        public DefinedContexts Contexts { get; set; }
        public bool Initialized { get { return _projectList != null; } }

        public int ProjectCount { get { return _projectList == null ? 0 : _projectList.Count; } }

        public Session(IPersistence persistence)
        {
            log.Info("Starting Session");
            _persistence = persistence;
        }

        public void LoadContexts()
        {
            this.Contexts = _persistence.LoadContexts();
        }

        public void LoadProjects()
        {
            _projectList = _persistence.LoadProjects();
            SetTaskOrderIncrement();
        }

        public void Save()
        {
            SetTaskOrderIncrement();

            _persistence.SaveProjects(_projectList);
            _persistence.SaveContexts(this.Contexts);
        }

        private void SetTaskOrderIncrement()
        {
            // Make sure each Task's Order property is set "correctly"
            foreach (var prj in _projectList)
            {
                prj.OrderTasks();
            }
        }

        public IEnumerable<Project> ListProjectsWithNoTasks()
        {
            return _projectList != null ? _projectList.Where(p => !p.HasTasks) : null;
        }

        public Project FindProjectByID(string prjId)
        {
            return _projectList?.Where(value => value.UniqueID.ToString() == prjId).FirstOrDefault();
        }

        public void AddProject(Project prj)
        {
            _projectList.Add(prj);
        }

        public System.Collections.Generic.IEnumerable<Project> ProjectEnumerable()
        {
            if (_projectList == null)
            {
                yield break;
            }

            foreach (var prj in _projectList)
            {
                yield return prj;
            }
        }

        public void RemoveProject(String prjID)
        {
            var prj = FindProjectByID(prjID);
            _projectList.Remove(prj);
        }

        public bool LoadDefaultContexts()
        {
            if (this.Contexts != null && this.Contexts.Count > 0)
            {
                return false;
            }

            this.Contexts = new DefaultContextGenerator().GenerateDefaultContexts();
            return true;
        }

        public List<Task> GetTaskList(Guid prjID)
        {
            var prj = FindProjectByID(prjID.ToString());
            if (prj == null)
            {
                throw new ArgumentException("Invalid ProjectID");
            }
            return prj.GetTaskList().OrderBy(t => t.Order).ToList();
        }

        public void RemoveContext(Context ctx)
        {
            if (IsContextUsed(ctx))
            {
                throw new ArgumentException("Cannot remove a Context that is being used");
            }

            Contexts.Remove(ctx.ID);
        }

        private bool IsContextUsed(Context ctx)
        {
            foreach (var prj in _projectList)
            {
                foreach (var task in prj.TaskList)
                {
                    if (task.ContextID == ctx.ID)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}