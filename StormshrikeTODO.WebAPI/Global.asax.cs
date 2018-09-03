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
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);


            var loggingConfigFile = System.Environment.GetEnvironmentVariable("STORMSHRIKETODO_LOGGING_CONFIG_WEBAPI");
            if (String.IsNullOrEmpty(loggingConfigFile))
            {
                loggingConfigFile = System.Environment.GetEnvironmentVariable("STORMSHRIKETODO_LOGGING_CONFIG");
            }

            if (String.IsNullOrEmpty(loggingConfigFile))
            {
                loggingConfigFile = "logging-config-webapi.xml";
            }

            var dbLoggingOutputDir = System.Environment.GetEnvironmentVariable("STORMSHRIKETODO_LOGGING_OUTPUT_DIR");
            if (String.IsNullOrEmpty(dbLoggingOutputDir))
            {
                var tempDir = System.Environment.GetEnvironmentVariable("TEMP");
                dbLoggingOutputDir = tempDir;
            }

            System.Environment.SetEnvironmentVariable("STORMSHRIKETODO_LOG_DIR", dbLoggingOutputDir);

            XmlConfigurator.Configure(new System.IO.FileInfo(loggingConfigFile));
            log.Info("Starting StormshrikeTODO.WebAPI");
        }

        protected void Application_Stop()
        {
            log.Info("Stopping StormshrikeTODO.WebAPI");
        }
    }
}
