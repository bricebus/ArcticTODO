using log4net;
using StormshrikeTODO.Model;
using StormshrikeTODO.Persistence;
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
        //private ProjectApi[] projects = new ProjectApi[]
        //{
        //    new ProjectApi("Test Project 1.1c", "8e2498a5-4f73-4378-a265-eeaa2610f28c"),
        //    new ProjectApi("Test Project 2.1c", "d4d5f228-bf22-4501-ab8b-0f1d40f85569"),
        //    new ProjectApi("Test Project 3.1c", "5988c4c8-2fca-4612-850c-203c45eccdeb"),
        //};

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
