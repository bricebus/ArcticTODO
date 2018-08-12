using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StormshrikeTODO.Model;

namespace StormshrikeTODO.Tests
{
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class DefinedContextsTest
    {
        [TestMethod]
        public void TestFindContextByDescr()
        {
            String id = Guid.NewGuid().ToString();
            Context context = new Context(id, "Home");
            DefinedContexts dc = new DefinedContexts();
            dc.Add(context);

            Assert.AreEqual(id, dc.FindIdByDescr("Home").ID);
            Assert.AreEqual("Home", dc.FindIdByDescr("Home").ToString());
        }

        [TestMethod]
        public void TestFindContextByDescrInvalid()
        {
            String id = Guid.NewGuid().ToString();
            Context context = new Context(id, "Home");
            DefinedContexts dc = new DefinedContexts();
            dc.Add(context);

            Assert.IsNull(dc.FindIdByDescr("blah"));
        }

        [TestMethod]
        public void TestFindContextByID()
        {
            String id = Guid.NewGuid().ToString();
            Context context = new Context(id, "Home");
            DefinedContexts dc = new DefinedContexts();
            dc.Add(context);

            Assert.AreEqual("Home", dc.FindIdByID(id).ToString());
            Assert.AreEqual(id, dc.FindIdByID(id).ID);
        }

        [TestMethod]
        public void TestFindContextByIDInvalid()
        {
            String id = Guid.NewGuid().ToString();
            Context context = new Context(id, "Home");
            DefinedContexts dc = new DefinedContexts();
            dc.Add(context);

            Assert.IsNull(dc.FindIdByID("blah"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestDuplicateID()
        {
            String id = Guid.NewGuid().ToString();
            Context context1 = new Context(id, "Home1");
            Context context2 = new Context(id, "Home2");
            DefinedContexts dc = new DefinedContexts();
            dc.Add(context1);
            dc.Add(context2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestDuplicateDescr()
        {
            String id1 = Guid.NewGuid().ToString();
            String id2 = Guid.NewGuid().ToString();
            Context context1 = new Context(id1, "Home");
            Context context2 = new Context(id2, "Home");
            DefinedContexts dc = new DefinedContexts();
            dc.Add(context1);
            dc.Add(context2);
        }

        [TestMethod]
        public void TestCount()
        {
            DefinedContexts dc = CreateTwo();

            Assert.AreEqual(2, dc.Count);
        }

        [TestMethod]
        public void TestCountEmpty()
        {
            DefinedContexts dc = new DefinedContexts();
            Assert.AreEqual(0, dc.Count);
        }

        [TestMethod]
        public void TestRemove()
        {
            String id1 = Guid.NewGuid().ToString();
            String id2 = Guid.NewGuid().ToString();
            String id3 = Guid.NewGuid().ToString();
            Context context1 = new Context(id1, "Home1");
            Context context2 = new Context(id2, "Home2");
            DefinedContexts dc = new DefinedContexts();
            dc.Add(context1);
            dc.Add(context2);

            dc.Remove(id1);

            Assert.AreEqual(1, dc.Count);

            context1 = new Context(id1, "Home1");
            dc.Add(context1);
            Assert.AreEqual(2, dc.Count);
        }

        [TestMethod]
        public void RemoveInvalid()
        {
            DefinedContexts dc = new DefinedContexts();
            dc.Remove("blah");
        }

        [TestMethod]
        public void TestGetList0()
        {
            DefinedContexts dc = new DefinedContexts();
            Assert.AreEqual(0, dc.GetList().Count);
        }

        [TestMethod]
        public void TestGetList2()
        {
            DefinedContexts dc = CreateTwo();

            List<Context> ctxList = dc.GetList();
            Assert.AreEqual(2, ctxList.Count);
            Assert.AreEqual("Home1", ctxList[0].ToString());
            Assert.AreEqual("Home2", ctxList[1].ToString());
        }

        [TestMethod]
        public void TestGetContextDescriptions()
        {
            var contexts = CreateTwo();
            var ctx1 = contexts.FindIdByDescr("Home2");
            var id = ctx1.ID;

            Assert.AreEqual("Home2", contexts.GetDescription(id));
        }

        [TestMethod]
        public void TestGetContextDescriptionsWithInvalidID()
        {
            var contexts = CreateTwo();
            var ctx1 = contexts.FindIdByDescr("Home2");

            Assert.AreEqual("<No Description Available>", contexts.GetDescription("ID"));
        }

        private static DefinedContexts CreateTwo()
        {
            String id1 = Guid.NewGuid().ToString();
            String id2 = Guid.NewGuid().ToString();
            Context context1 = new Context(id1, "Home1");
            Context context2 = new Context(id2, "Home2");
            DefinedContexts dc = new DefinedContexts();
            dc.Add(context1);
            dc.Add(context2);
            return dc;
        }
    }
}
