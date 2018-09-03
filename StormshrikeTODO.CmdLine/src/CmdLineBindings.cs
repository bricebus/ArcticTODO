using Ninject.Modules;
using Ninject;
using StormshrikeTODO.Data;
using StormshrikeTODO.Model;

namespace StormshrikeTODO
{
    public class CmdLineBindings : NinjectModule
    {
        public override void Load()
        {
            Bind<Session>().To<Session>();
            //Bind<IPersistence>().To<BinFilePersistence>();
            //Bind<IPersistence>().To<XmlFilePersistence>();
            Bind<IPersistence>().To<SQLitePersistence>();
        }
    }
}