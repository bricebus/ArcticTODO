using System;

namespace StormshrikeTODO.Model
{
    public class DefaultContextGenerator
    {
        public DefinedContexts GenerateDefaultContexts()
        {
            var dc = new DefinedContexts();
            dc.Add(new Context(Guid.NewGuid().ToString(), "Home"));
            dc.Add(new Context(Guid.NewGuid().ToString(), "Office"));
            dc.Add(new Context(Guid.NewGuid().ToString(), "Computer"));
            dc.Add(new Context(Guid.NewGuid().ToString(), "Errands"));
            return dc;
        }
    }
}