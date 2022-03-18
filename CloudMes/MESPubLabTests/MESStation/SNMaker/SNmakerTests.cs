using Microsoft.VisualStudio.TestTools.UnitTesting;
using MESPubLab.MESStation.SNMaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESDBHelper;
using System.Collections;

namespace MESPubLab.MESStation.SNMaker.Tests
{
    [TestClass()]
    public class SNmakerTests
    {
        [TestMethod()]
        public void GetNextSNTest()
        {

            OleExec DBB = new OleExec("Data Source = 10.18.136.73:1521 / JNPODB; User ID = TEST; Password = SFCTEST"); ;
            SNmaker SNmaker = new SNmaker();
            SNmaker.GetNextSN("Oracle_SN_2", DBB);
            ArrayList testArr = new ArrayList();

            for (int i = 0; i < 1000; i++)
            {
                testArr.Add(SNmaker.GetNextSN("Oracle_SN_2", DBB));

            }


            Assert.IsTrue(testArr.Count > 0);
        }
    }
}