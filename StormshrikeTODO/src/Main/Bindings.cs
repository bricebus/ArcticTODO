using Ninject.Modules;
using Ninject;
using StormshrikeTODO.Persistence;
using StormshrikeTODO.Model;

public class Bindings : NinjectModule
{
    public override void Load()
    {
        Bind<Session>().To<Session>();
        //Bind<IPersistence>().To<BinFilePersistence>();
        Bind<IPersistence>().To<XmlFilePersistence>();
    }
}