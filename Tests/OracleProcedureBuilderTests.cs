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
        public void ParseToProcedurePack()
        {
            //Arrange
            List<SynchronizationObject> list = new List<SynchronizationObject>()
            {
                new SynchronizationObject()
                {
                    Order = 1,
                    ProceduresList =
                    new List<Procedure>() {
                        new Procedure() { Order = 1, ProcedureName = "TestProcedure", ProcedureParams =
                            new List<string>() { "TestParams" } },
                        new Procedure() {Order = 2, ProcedureName = "TestProcedure2", ProcedureParams =
                            new List<string>()}},
                    SchemaName = "TestSchema",
                    WithNoIndex = true
                }
            };
            List<OracleProcedurePack> expected = new List<OracleProcedurePack>()
            {
                new OracleProcedurePack("TestSchema")
                {
                    WithNoIndex = true,
                    Procedures = new SortedDictionary<int, OracleCommand>()
                    {
                        { 1, new OracleCommand("TestProcedure").AddStringParameter("TestParams") },
                        { 2, new OracleCommand("TestProcedure2") }
                    }
                }
            };

            //Act
            List<OracleProcedurePack> actual = OracleProcedureBuilder.ParseToProcedurePack(list);

            //Assert
            Assert.AreEqual(expected[0].SchemaName, actual[0].SchemaName);
            Assert.AreEqual(expected[0].SuccessfullyCompleted, actual[0].SuccessfullyCompleted);
            Assert.AreEqual(expected[0].WithNoIndex, actual[0].WithNoIndex);
            Assert.AreEqual(expected[0].Procedures[1].Parameters[0].Value, actual[0].Procedures[1].Parameters[0].Value);
            Assert.AreEqual(expected[0].Procedures[1].CommandText, actual[0].Procedures[1].CommandText);
            Assert.AreEqual(expected[0].Procedures[2].CommandText, actual[0].Procedures[2].CommandText);
        }

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
