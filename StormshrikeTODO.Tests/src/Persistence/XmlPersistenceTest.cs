using Microsoft.VisualStudio.TestTools.UnitTesting;
using StormshrikeTODO.Model;
using StormshrikeTODO.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StormshrikeTODO.Tests.Persistence
{
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class XmlPersistenceTest
    {

        [TestMethod]
        public void TestCreateFile()
        {
            String prfTestFile = GetTestPrjFileName();
            String ctxTestFile = GetTestContextFileName();
            var xmlFilePersistenceConfig = new XmlFilePersistenceConfig(prfTestFile, ctxTestFile);
            System.IO.File.Delete(prfTestFile);
            System.IO.File.Delete(ctxTestFile);
            Assert.IsFalse(System.IO.File.Exists(prfTestFile));
            Assert.IsFalse(System.IO.File.Exists(ctxTestFile));

            var xp = new XmlFilePersistence(xmlFilePersistenceConfig);
            xp.CreateProjectFile();
            xp.CreateContextFile();
            Assert.IsTrue(System.IO.File.Exists(ctxTestFile), "Cannot find Context file!");
            Assert.IsTrue(System.IO.File.Exists(prfTestFile), "Cannot find Project file!");
        }

        [TestMethod]
        public void TestWriteProjectFile()
        {
            var xmlFilePersistenceConfig = new XmlFilePersistenceConfig(GetTestPrjFileName(), GetTestContextFileName());
            var xp = new XmlFilePersistence(xmlFilePersistenceConfig);
            var prjList = new Collection<Project>();
            var prj = new Project("Test Project 1");
            prjList.Add(prj);
            xp.SaveProjects(prjList);

            var newPrjList = xp.LoadProjects();
            Assert.AreEqual(1, newPrjList.Count);
            var newPrj = newPrjList[0];

            Assert.AreEqual("Test Project 1", newPrj.ProjectName);
        }


        [TestMethod]
        public void TestReadNonexistentProjectFile()
        {
            String testPrjFile = GetTestPrjFileName();
            var xmlFilePersistenceConfig = new XmlFilePersistenceConfig(testPrjFile, GetTestContextFileName());
            System.IO.File.Delete(testPrjFile);
            Assert.IsFalse(System.IO.File.Exists(testPrjFile));

            var xp = new XmlFilePersistence(xmlFilePersistenceConfig);
            var newPrjList = xp.LoadProjects();
            Assert.AreEqual(0, newPrjList.Count);
        }

        private static string GetTestPrjFileName()
        {
            String tempDir = System.Environment.GetEnvironmentVariable("TEMP");
            String testFile = tempDir + "\\PrjTestFile.xml";
            return testFile;
        }

        private static string GetTestContextFileName()
        {
            String tempDir = System.Environment.GetEnvironmentVariable("TEMP");
            String testFile = tempDir + "\\CtxTestFile.xml";
            return testFile;
        }
    }
}
