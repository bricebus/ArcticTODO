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
        ProjectApi[] projects = new ProjectApi[] 
        { 
            new ProjectApi("Test Projecgt 1"),
            new ProjectApi("Test Projecgt 2"),
            new ProjectApi("Test Projecgt 3")
        };

        public IEnumerable<ProjectApi> GetAllProject()
        {
            return projects;
        }

        public IHttpActionResult GetProject(string id)
        {
            if (id != "afdlh")
            {
                throw new ArgumentException("invalid id");
            }

            var product = projects.FirstOrDefault((p) => p.UniqueID.ToString() == id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
    }

    public class ProjectApi
    {
        public string UniqueID { get; set; }

        public string ProjectName { get; set; }

        public ProjectApi(string name)
        {
            this.ProjectName = name;
            this.UniqueID = Guid.NewGuid().ToString();
        }

    }
}
