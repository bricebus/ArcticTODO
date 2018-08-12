using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StormshrikeTODO.Model.Tests
{
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class DefaultContextGeneratorTest
    {
        [TestMethod]
        public void TestGenerateDefaultContexts()
        {
            DefaultContextGenerator dcg = new DefaultContextGenerator();

            DefinedContexts dc = dcg.GenerateDefaultContexts();

            Assert.IsNotNull(dc.FindIdByDescr("Home"));
            Assert.IsNotNull(dc.FindIdByDescr("Office"));
            Assert.IsNotNull(dc.FindIdByDescr("Computer"));
            Assert.IsNotNull(dc.FindIdByDescr("Errands"));
        }

    }
}
