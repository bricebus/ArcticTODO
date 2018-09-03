using log4net;
using log4net.Config;
using Ninject;
using StormshrikeTODO.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace StormshrikeTODO.WebAPI
{
    public class StormshrikeTODOWebApi : System.Web.HttpApplication
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(StormshrikeTODOWebApi));
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            //GlobalConfiguration.Configuration.DependencyResolver = 
            //    kernel.Get<System.Web.Http.Dependencies.IDependencyResolver>();

            XmlConfigurator.Configure(new System.IO.FileInfo("c:\\Temp\\StormshrikeTODO-WebApi-log4net-config.xml"));
            log.Info("Starting StormshrikeTODO.WebAPI");

            //IKernel kernel = new StandardKernel(new Bindings());

            //var session = kernel.Get<Session>();
        }
    }
}
