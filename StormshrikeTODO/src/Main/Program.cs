using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StormshrikeTODO.Model;
using StormshrikeTODO.CmdLine;
using StormshrikeTODO.Persistence;
using Ninject;

namespace StormshrikeTODO
{
    class Program
    {
        static int Main(string[] args)
        {
            IKernel kernel = new StandardKernel(new Bindings());

            var session = kernel.Get<Session>();

            //var xmlconfig = new XmlFilePersistenceConfig();
            //var binconfig = new BinFilePersistenceConfig();
            //var session = new Session(new XmlFilePersistence(xmlconfig), new BinFilePersistence(binconfig));

            return new CmdLineMain(session).Start(args);
        }
    }
}
