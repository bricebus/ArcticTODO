using Microsoft.VisualStudio.TestTools.UnitTesting;
using StormshrikeTODO.Model;
using StormshrikeTODO.Persistence;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StormshrikeTODO.Tests.Persistence
{
    [TestClass]
    public class SQLItePersistenceTest
    {
        private static string _testDbLocation = System.Environment.GetEnvironmentVariable("TEMP")
            + "\\StormshrikeTest-" + System.Diagnostics.Process.GetCurrentProcess().Id + ".db";

        public static int RunPowershellScript(string ps, string args)
        {
            int errorLevel;
            ProcessStartInfo processInfo;
            Process process;


            if (!System.IO.File.Exists(ps))
            {
                throw new ArgumentException(ps + " file not found!");
            }

            string ps1arg = "-File " + ps + " " + args;

            processInfo = new ProcessStartInfo("powershell.exe", ps1arg)
            {
                CreateNoWindow = false,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            
            using (process = Process.Start(processInfo))
            {
                //
                // Read in all the text from the process with the StreamReader.
                //
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.Write(result);
                }

                string stdErr = "";
                using (StreamReader reader = process.StandardError)
                {
                    stdErr = reader.ReadToEnd();
                }
                if (!String.IsNullOrEmpty(stdErr))
                {
                    throw new Exception("Error running " + ps + ":" + stdErr);
                }

                process.WaitForExit();
                errorLevel = process.ExitCode;
                process.Close();
            }


            return errorLevel;
        }

        [ClassInitialize]
        public static void CreateTestDB(TestContext testContext)
        {
            string origCwd = System.IO.Directory.GetCurrentDirectory();

            System.IO.Directory.SetCurrentDirectory("..\\..\\..\\StormshrikeTODO\\bin\\Debug\\Data\\SQLite");
            RunPowershellScript("CreateDB.ps1", "-dblocation \"" + _testDbLocation + "\"");
            System.IO.Directory.SetCurrentDirectory(origCwd);
            Assert.IsTrue(System.IO.File.Exists(_testDbLocation), "Cannot find DB file!");
        }


        [ClassCleanup]
        public static void Cleanup()
        {
            System.IO.File.Delete(_testDbLocation);
        }

        [TestMethod]
        public void TestLoadContexts()
        {
            using (SQLitePersistence sqlite = new SQLitePersistence(new SQLitePersistenceConfig(_testDbLocation)))
            {
                DefinedContexts dc = sqlite.LoadContexts();
                Assert.AreEqual(5, dc.Count);
                Assert.AreEqual("2ad57821-dad5-4e0a-abb4-47d99b314f21", dc.FindIdByDescr("Home").ID);
                Assert.AreEqual("be17f3e2-764b-43b5-b943-63faf6223863", dc.FindIdByDescr("Office").ID);
                Assert.AreEqual("f89d513b-c24d-468e-99f3-b841e5ceca6f", dc.FindIdByDescr("Computer").ID);
                Assert.AreEqual("ae7491da-4a83-4cc6-ad26-cd090e81417b", dc.FindIdByDescr("Errands").ID);
                Assert.AreEqual("c50d02de-d22c-475b-9fef-6e24c05f966b", dc.FindIdByDescr("Phone").ID);
            }

        }
    }
}
