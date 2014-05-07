using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EEGETAnalysis.Library;
using System.Collections.Generic;

namespace EEGETAnalysis.LibraryTest
{
    [TestClass]
    public class LibraryTest
    {
        [TestMethod]
        public void CsvParseTest()
        {
            CsvParser parser = new CsvParser(@"C:\Users\Thomas Klytta\Documents\Studienarbeit\iPhone5_Ad_Thomas_mit_EEG.csv");
            List<List<string>> result = parser.Parse();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void EEGTDataTest()
        {
            CsvParser parser = new CsvParser(@"C:\Users\Thomas Klytta\Documents\Studienarbeit\iPhone5_Ad_Thomas_mit_EEG.csv");
            List<List<string>> result = parser.Parse();

            EEGETData data = new EEGETData(result);
        }
    }
}
