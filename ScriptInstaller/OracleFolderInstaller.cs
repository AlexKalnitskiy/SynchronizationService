using System;
using System.Collections.Generic;
using System.Linq;
using Oracle.ManagedDataAccess.Client;
using System.IO;

namespace ScriptInstaller
{
    public class OracleFolderInstaller
    {
        public OracleConnection Connection;

        #region Events
        public enum EventInfo { TRY, SUCCESS, FAIL }

        public delegate void ScriptEventHandler(object sender, EventInfo info, ScriptEventArgs args);

        public delegate void ErrorEventHandler(object sender, EventInfo info, string source, Exception exception);

        public event ScriptEventHandler ScriptEvent;

        public event ErrorEventHandler ErrorEvent;
        public class ScriptEventArgs
        {
            public OracleCommand Command;
            public string File;
            public ScriptEventArgs(OracleCommand oracleCommand, string fileName)
            {
                Command = oracleCommand;
                File = fileName;
            }
        }
        #endregion Events

        public OracleFolderInstaller(string connectionString)
        {
            Connection = new OracleConnection(connectionString);
            Connection.Open();
        }

        public static List<string> GetAllFilesRec(List<string> files, string path)
        {
            Directory.GetDirectories(path)
                .ToList()
                .ForEach(subPath => {
                    if (Directory.GetDirectories(subPath).Length == 0)
                        files.AddRange(Directory.GetFiles(subPath).ToList());
                    else
                    {
                        GetAllFilesRec(files, subPath);
                        files.AddRange(Directory.GetFiles(subPath).ToList());
                    }
                });
            return files;
        }

        public static List<string> GetAllFilesRec(List<string> files, string path, string[] ignore)
        {
            Directory.GetDirectories(path)
                .Where(pred => !ignore.Contains(pred.Split('\\').Last()))
                .ToList()
                .ForEach(subPath => {
                    files.AddRange(Directory.GetFiles(subPath).ToList());
                    if (Directory.GetDirectories(subPath).Length != 0)
                        GetAllFilesRec(files, subPath, ignore);
                });
            return files;
        }
        public OracleCommand GetCommandFromFile(string filename)
        {
            string text = File.ReadAllText(filename);
            OracleCommand result = new OracleCommand(text, Connection);
            return result;
        }
        public void ExecuteFiles(List<string> files)
        {
            foreach (string filename in files)
            {
                OracleCommand command = GetCommandFromFile(filename);
                try
                {
                    ScriptEvent(this, EventInfo.TRY, new ScriptEventArgs(command, filename.Split('\\').Last()));
                    command.ExecuteNonQuery();
                    ScriptEvent(this, EventInfo.SUCCESS, new ScriptEventArgs(command, filename.Split('\\').Last()));
                }
                catch (Exception exc)
                {
                    ErrorEvent(this, EventInfo.FAIL, filename.Split('\\').Last(), exc);
                }
            }
        }
        public void WithTransaction(Action action)
        {
            OracleTransaction act = Connection.BeginTransaction();
            action.Invoke();
            act.Commit();
        }
    }
}
