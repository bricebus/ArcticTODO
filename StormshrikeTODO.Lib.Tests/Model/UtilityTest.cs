using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StormshrikeTODO.Model;
using StormshrikeTODO.Model.Util;

namespace StormshrikeTODO.Tests.Model
{
    [TestClass]
    public class UtilityTest
    {
        [TestMethod]
        public void TestGetShortDateTime()
        {
            DateTime? dateTime = null;

            Assert.AreEqual("", Utility.GetDateTimeString(dateTime));
        }
    }
}
