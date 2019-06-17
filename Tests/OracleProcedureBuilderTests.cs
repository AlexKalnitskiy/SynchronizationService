using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OracleProcedureManager;
using Oracle.ManagedDataAccess.Client;

namespace Tests
{
    [TestClass]
    public class OracleProcedureBuilderTests
    {
        [TestMethod]
        public void AddStringParameter()
        {
            //Arrange
            string expected = "TestParam1";

            //Act
            OracleCommand command = new OracleCommand("testText").AddStringParameter("TestParam1");
            string actual = command.Parameters[0].Value.ToString();

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void AddDateTimeParameter()
        {
            //Arrange
            DateTime expected = DateTime.Now;

            //Act
            OracleCommand command = new OracleCommand("testText").AddDateTimeParameter(expected);
            DateTime actual = (DateTime)command.Parameters[0].Value;

            //Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
