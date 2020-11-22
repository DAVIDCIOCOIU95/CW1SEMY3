using Microsoft.VisualStudio.TestTools.UnitTesting;
using NapierBankingApp.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace NPMTest
{
    [TestClass]
    public class ProcessorTest: Processor
    {
        [TestMethod]
        public void SobstituteAbbreviations_CorrectlyProcessed_ShouldPass()
        {
            List<string> fields = new List<string>() { "+1234567", "LOL" };
            Assert.AreEqual("LOL <Laughing out loud>", SobstituteAbbreviations(fields[1]));
        }
    }
}
