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
            Assert.AreEqual("Home", ctxList[0].ToString());
            Assert.AreEqual("Work", ctxList[1].ToString());
        }

        [TestMethod]
        public void TestGetContextDescriptions()
        {
            var contexts = CreateTwo();
            var ctx1 = contexts.FindIdByDescr("Work");
            var id = ctx1.ID;

            Assert.AreEqual("Work", contexts.GetDescription(id));
        }

        [TestMethod]
        public void TestGetContextDescriptionsWithInvalidID()
        {
            var contexts = CreateTwo();
            var ctx1 = contexts.FindIdByDescr("Work");

            Assert.AreEqual("<No Description Available>", contexts.GetDescription("ID"));
        }

        [TestMethod]
        public void TestContainsIDTrue()
        {
            string id1;
            string id2;
            DefinedContexts dc = CreateTwo(out id1, out id2);

            Assert.IsTrue(dc.ContainsID(id1));
        }

        [TestMethod]
        public void TestContainsIDFalse()
        {
            string id;
            DefinedContexts dc = CreateOne(out id);

            Assert.IsFalse(dc.ContainsID("blah"));
        }

        [TestMethod]
        public void TestContainsIDEmptyList()
        {
            DefinedContexts dc = new DefinedContexts();

            Assert.IsFalse(dc.ContainsID("blah"));
        }

        [TestMethod]
        public void TestContainsIDNull()
        {
            DefinedContexts dc = new DefinedContexts();

            Assert.IsFalse(dc.ContainsID(null));
        }

        [TestMethod]
        public void TestFindChangedContexts_Same()
        {
            DefinedContexts dc1 = CreateTwo();
            DefinedContexts dc2 = CreateTwo();

            var areDifferences = DefinedContexts.IdentifyDifferences(dc1, dc2, out List<Context> newList,
                out List<Context> chgList, out List<Context> delList);

            Assert.IsFalse(areDifferences);
            Assert.IsFalse(DefinedContexts.AreDifferences(dc1, dc2));
            Assert.AreEqual(0, newList.Count);
            Assert.AreEqual(0, chgList.Count);
            Assert.AreEqual(0, delList.Count);
        }

        [TestMethod]
        public void TestFindChangedContexts_Changed()
        {
            DefinedContexts dc1 = CreateTwo();
            DefinedContexts dc2 = CreateAnotherTwo();

            var areDifferences = DefinedContexts.IdentifyDifferences(dc1, dc2, out List<Context> newList,
                out List<Context> chgList, out List<Context> delList);

            Assert.IsTrue(areDifferences);
            Assert.IsTrue(DefinedContexts.AreDifferences(dc1, dc2));
            Assert.AreEqual(0, newList.Count);
            Assert.AreEqual(1, chgList.Count);
            Assert.AreEqual(0, delList.Count);

            Assert.IsTrue(chgList.Exists(c => c.ID == "63418228-3a88-4765-96a7-f659d79df70e"));
            Assert.AreEqual("Errands", chgList.First().Description);
        }

        [TestMethod]
        public void TestFindChangedContexts_Deleted()
        {
            DefinedContexts dc1 = CreateTwo();
            DefinedContexts dc2 = CreateOne(out string id);

            var areDifferences = DefinedContexts.IdentifyDifferences(dc1, dc2, out List<Context> newList,
                out List<Context> chgList, out List<Context> delList);

            Assert.IsTrue(DefinedContexts.AreDifferences(dc1, dc2));
            Assert.IsTrue(areDifferences);
            Assert.AreEqual(0, newList.Count);
            Assert.AreEqual(0, chgList.Count);
            Assert.AreEqual(1, delList.Count);

            Assert.IsTrue(delList.Exists(c => c.ID == "1b156765-886d-47a6-80b7-a63427203ca7"));
            Assert.AreEqual("Work", delList.First().Description);
        }

        [TestMethod]
        public void TestFindChangedContexts_New()
        {
            DefinedContexts dc1 = CreateAnotherOne(out string id);
            DefinedContexts dc2 = CreateTwo();

            var areDifferences = DefinedContexts.IdentifyDifferences(dc1, dc2, out List<Context> newList,
                out List<Context> chgList, out List<Context> delList);

            Assert.IsTrue(areDifferences);
            Assert.IsTrue(DefinedContexts.AreDifferences(dc1, dc2));
            Assert.AreEqual(1, newList.Count);
            Assert.AreEqual(0, chgList.Count);
            Assert.AreEqual(0, delList.Count);

            Assert.IsTrue(newList.Exists(c => c.ID == "63418228-3a88-4765-96a7-f659d79df70e"));
            Assert.AreEqual("Home", newList.First().Description);
        }

        [TestMethod]
        public void TestFindChangedContexts_Chg_And_New()
        {
            DefinedContexts dc1 = CreateErrandsContext();
            DefinedContexts dc2 = CreateTwo();

            var areDifferences = DefinedContexts.IdentifyDifferences(dc1, dc2, out List<Context> newList,
                out List<Context> chgList, out List<Context> delList);

            Assert.IsTrue(areDifferences);
            Assert.IsTrue(DefinedContexts.AreDifferences(dc1, dc2));
            Assert.AreEqual(1, newList.Count);
            Assert.AreEqual(1, chgList.Count);
            Assert.AreEqual(0, delList.Count);

            Assert.IsTrue(newList.Exists(c => c.ID == "1b156765-886d-47a6-80b7-a63427203ca7"));
            Assert.AreEqual("Work", newList.First().Description);

            Assert.IsTrue(chgList.Exists(c => c.ID == "63418228-3a88-4765-96a7-f659d79df70e"));
            Assert.AreEqual("Home", chgList.First().Description);
        }

        private static DefinedContexts CreateOne(out string id)
        {
            id = Guid.Parse("63418228-3a88-4765-96a7-f659d79df70e").ToString();
            Context context = new Context(id, "Home");
            DefinedContexts dc = new DefinedContexts();
            dc.Add(context);
            return dc;
        }

        private static DefinedContexts CreateAnotherOne(out string id)
        {
            id = Guid.Parse("1b156765-886d-47a6-80b7-a63427203ca7").ToString();
            Context context = new Context(id, "Work");
            DefinedContexts dc = new DefinedContexts();
            dc.Add(context);
            return dc;
        }

        private static DefinedContexts CreateErrandsContext()
        {
            String id = Guid.Parse("63418228-3a88-4765-96a7-f659d79df70e").ToString();
            Context context = new Context(id, "Errands");
            DefinedContexts dc = new DefinedContexts();
            dc.Add(context);
            return dc;
        }

        private static DefinedContexts CreateTwo()
        {
            String id1 = Guid.Parse("63418228-3a88-4765-96a7-f659d79df70e").ToString();
            String id2 = Guid.Parse("1b156765-886d-47a6-80b7-a63427203ca7").ToString();
            Context context1 = new Context(id1, "Home");
            Context context2 = new Context(id2, "Work");
            DefinedContexts dc = new DefinedContexts();
            dc.Add(context1);
            dc.Add(context2);
            return dc;
        }

        private static DefinedContexts CreateAnotherTwo()
        {
            String id1 = Guid.Parse("63418228-3a88-4765-96a7-f659d79df70e").ToString();
            String id2 = Guid.Parse("1b156765-886d-47a6-80b7-a63427203ca7").ToString();
            Context context1 = new Context(id1, "Errands");
            Context context2 = new Context(id2, "Work");
            DefinedContexts dc = new DefinedContexts();
            dc.Add(context1);
            dc.Add(context2);
            return dc;
        }

        private static DefinedContexts CreateTwo(out string id1, out string id2)
        {
            id1 = Guid.Parse("63418228-3a88-4765-96a7-f659d79df70e").ToString();
            id2 = Guid.Parse("1b156765-886d-47a6-80b7-a63427203ca7").ToString();
            Context context1 = new Context(id1, "Home");
            Context context2 = new Context(id2, "Work");
            DefinedContexts dc = new DefinedContexts();
            dc.Add(context1);
            dc.Add(context2);
            return dc;
        }
    }
}
