using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;

namespace OracleProcedureManager
{
    public static class DBMSReader
    {
        public static List<string> ReadAllDBMSAfter(OracleConnection connection)
        {
            List<string> resultList = new List<string>();
            OracleCommand readerCommand = connection.CreateCommand();
            readerCommand.CommandText = "select READ_DBMS from dual";
            string result = readerCommand.ExecuteScalar().ToString();
            while (result != "")
            {
                resultList.Add(result.ToString());
                result = readerCommand.ExecuteScalar().ToString();
            }
            return resultList;
        }
    }
}