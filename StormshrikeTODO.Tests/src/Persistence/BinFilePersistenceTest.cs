using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StormshrikeTODO.Persistence;
using StormshrikeTODO.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace StormshrikeTODO.Tests.Persistence
{
    [TestClass]
    public class BinFilePersistenceTest
    {
        /// <summary>
        /// Test that BinFilePersistence will create a file.
        /// </summary>
        [TestMethod]
        public void TestCreateFile()
        {
            String prfTestFile = GetTestPrjFileName();
            String ctxTestFile = GetTestContextFileName();
            var binFilePersistenceConfig = new BinFilePersistenceConfig(prfTestFile, ctxTestFile);
            System.IO.File.Delete(prfTestFile);
            System.IO.File.Delete(ctxTestFile);
            Assert.IsFalse(System.IO.File.Exists(prfTestFile));
            Assert.IsFalse(System.IO.File.Exists(ctxTestFile));

            var fp = new BinFilePersistence(binFilePersistenceConfig);
            fp.CreateProjectFile();
            fp.CreateContextFile();
            Assert.IsTrue(System.IO.File.Exists(prfTestFile), "Cannot find Project file!");
            Assert.IsTrue(System.IO.File.Exists(ctxTestFile), "Cannot find Context file!");
        }

        [TestMethod]
        public void TestWriteProjectFile()
        {
            var binFilePersistenceConfig = new BinFilePersistenceConfig(GetTestPrjFileName(), GetTestContextFileName());
            var fp = new BinFilePersistence(binFilePersistenceConfig);
            var prjList = new Collection<Project>();
            var prj = new Project("Test Project 1");
            prjList.Add(prj);
            fp.SaveProjects(prjList);

            var newPrjList = fp.LoadProjects();
            Assert.AreEqual(1, newPrjList.Count);
            var newPrj = newPrjList[0];

            Assert.AreEqual("Test Project 1", newPrj.ProjectName);
        }

        [TestMethod]
        public void TestReadNonexistentProjectFile()
        {
            String testPrjFile = GetTestPrjFileName();
            var binFilePersistenceConfig = new BinFilePersistenceConfig(testPrjFile, GetTestContextFileName());
            System.IO.File.Delete(testPrjFile);
            Assert.IsFalse(System.IO.File.Exists(testPrjFile));

            var fp = new BinFilePersistence(binFilePersistenceConfig);
            var newPrjList = fp.LoadProjects();
            Assert.AreEqual(0, newPrjList.Count);
        }

        private static string GetTestPrjFileName()
        {
            String tempDir = System.Environment.GetEnvironmentVariable("TEMP");
            String testFile = tempDir + "\\PrjTestFile.stormtodo";
            return testFile;
        }

        private static string GetTestContextFileName()
        {
            String tempDir = System.Environment.GetEnvironmentVariable("TEMP");
            String testFile = tempDir + "\\CtxTestFile.stormtodo";
            return testFile;
        }
    }
}
