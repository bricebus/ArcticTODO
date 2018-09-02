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
        Project[] projects = new Project[] 
        { 
            new Project { ProjectName = "Test Projecgt 1" },
            new Project { ProjectName = "Test Projecgt 2" },
            new Project { ProjectName = "Test Projecgt 3" },
        };

        public IEnumerable<Project> GetAllProject()
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
}
