using System.Runtime.InteropServices;
using log4net;
using log4net.Config;
using Ninject;
using StormshrikeTODO.Model;
using StormshrikeTODO.CmdLine;

namespace StormshrikeTODO
{
    class StormshrikeTODO
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(StormshrikeTODO));

        static ConsoleEventDelegate handler;   // Keeps it from getting garbage collected

        // Pinvoke
        private delegate bool ConsoleEventDelegate(int eventType);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);

        static int Main(string[] args)
        {
            XmlConfigurator.Configure(new System.IO.FileInfo("StormshrikeTODO-log4net-config.xml"));
            log.Info("Starting StormshrikeTODO");

            handler = new ConsoleEventDelegate(ConsoleEventCallback);
            SetConsoleCtrlHandler(handler, true);

            IKernel kernel = new StandardKernel(new Bindings());

            var session = kernel.Get<Session>();

            var cmdlineStatus = new CmdLineMain(session).Start(args);

            log.Info("Exiting StormshrikeTODO with status " + cmdlineStatus);
            return cmdlineStatus;
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
