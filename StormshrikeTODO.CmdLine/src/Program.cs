using System;
using System.Runtime.InteropServices;
using log4net;
using log4net.Config;
using Ninject;
using StormshrikeTODO.Model;
using StormshrikeTODO.CmdLine;

namespace StormshrikeTODO.CmdLine
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static ConsoleEventDelegate handler;   // Keeps it from getting garbage collected

        // Pinvoke
        private delegate bool ConsoleEventDelegate(int eventType);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);

        static int Main(string[] args)
        {
            // Start logging
            StartLogging();

            // Wire up exit handler so if a user clicks the X in the cmd window,
            // it will get get logged
            handler = new ConsoleEventDelegate(ConsoleEventCallback);
            SetConsoleCtrlHandler(handler, true);

            // Start Ninject
            IKernel kernel = new StandardKernel(new CmdLineNinjectBindings());

            var session = kernel.Get<Session>();

            // Start the command line processor
            var cmdlineStatus = new CmdLineProcessor(session).Start(args);

            log.Info("Exiting StormshrikeTODO with status " + cmdlineStatus);
            return cmdlineStatus;
        }

        private static void StartLogging()
        {
            var loggingConfigFile = System.Environment.GetEnvironmentVariable("STORMSHRIKETODO_LOGGING_CONFIG_CMDLINE");
            if (String.IsNullOrEmpty(loggingConfigFile))
            {
                loggingConfigFile = System.Environment.GetEnvironmentVariable("STORMSHRIKETODO_LOGGING_CONFIG");
            }

            if (String.IsNullOrEmpty(loggingConfigFile))
            {
                loggingConfigFile = "logging-config-cmdline.xml";
            }

            var dbLoggingOutputDir = System.Environment.GetEnvironmentVariable("STORMSHRIKETODO_LOGGING_OUTPUT_DIR");
            if (String.IsNullOrEmpty(dbLoggingOutputDir))
            {
                var tempDir = System.Environment.GetEnvironmentVariable("TEMP");
                dbLoggingOutputDir = tempDir;
            }

            System.Environment.SetEnvironmentVariable("STORMSHRIKETODO_LOG_DIR", dbLoggingOutputDir);

            XmlConfigurator.Configure(new System.IO.FileInfo(loggingConfigFile));
            log.Info("Starting StormshrikeTODO");
            log.Debug("Using logging config file: " + loggingConfigFile);
        }

        static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2)
            {
                log.Info("Exiting StormshrikeTODO process");
            }
            return false;
        }
    }
}
