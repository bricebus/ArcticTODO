using Microsoft.VisualStudio.TestTools.UnitTesting;
using StormshrikeTODO.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StormshrikeTODO.Tests
{
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class ContextText
    {
        [TestMethod]
        public void TestContextCreation()
        {
            Guid id = Guid.NewGuid();
            Context context = new Context(id.ToString(), "Home");

            Assert.AreEqual("Home", context.ToString());
            Assert.AreEqual(id.ToString(), context.ID);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestContextNullDescr()
        {
            Context context = new Context(Guid.NewGuid().ToString(), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestContextBlankDescr()
        {
            Context context = new Context(Guid.NewGuid().ToString(), "");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBlankContextID()
        {
            Context context = new Context("", "Home");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestNullContextID()
        {
            Context context = new Context(null, "Home");
        }
    }
}
