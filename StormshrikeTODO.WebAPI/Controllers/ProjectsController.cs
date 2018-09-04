using log4net;
using StormshrikeTODO.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace StormshrikeTODO.WebAPI.Controllers
{
    public class ProjectsController : ApiController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IPersistence _persistence;

        public ProjectsController(IPersistence persistence)
        {
            log.Info("Starting ProjectsController");
            _persistence = persistence;
        }

        public IEnumerable<ProjectApi> GetAllProjects()
        {
            log.Debug("ProjectsController.GetAllProjects");
            return GetProjectApiList();
        }

        public IHttpActionResult GetProject(string id)
        {
            log.Debug("ProjectsController.GetProject:" + id);
            List<ProjectApi> prjList = GetProjectApiList();
            var prj = prjList.FirstOrDefault((p) => p.UniqueID.ToString() == id);
            if (prj == null)
            {
                log.Debug("Project " + id + " not found");
                return NotFound();
            }
            log.Debug("Project " + id + " found");
            return Ok(prj);
        }

        private List<ProjectApi> GetProjectApiList()
        {
            log.Debug("Loading project list");
            var projects = new List<ProjectApi>();
            var prjList = _persistence.LoadProjects();
            foreach (var prj in prjList)
            {
                projects.Add(new ProjectApi(prj));
            }

            log.Debug("Done loading project list");
            return projects;
        }
    }

    public class ProjectApi
    {
        public string UniqueID { get; set; }
        public string ProjectName { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime DateTimeCreated { get; set; }

        public ProjectApi(Project prj)
        {
            ProjectName = prj.ProjectName;
            UniqueID = prj.UniqueID.ToString();
            DueDate = prj.DueDate;
            DateTimeCreated = prj.DateTimeCreated;
        }

        public ProjectApi(string name, string id)
        {
            ProjectName = name;
            UniqueID = id;
        }
    }
}
