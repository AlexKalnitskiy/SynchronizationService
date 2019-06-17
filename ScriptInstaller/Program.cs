using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScriptInstaller
{
    class Program
    {
        static List<string> Errors = new List<string>();
        static int errors = 0;
        static int success = 0;
        static object errorsLocker = new object();
        static object successLocker = new object();
        static void Main(string[] args)
        {
            Console.WriteLine("This programm tries to install all oracle.sql scripts from chosen folder into base..");
            Console.WriteLine("Starting..");
            try
            {
                ConfigSettings config = ConfigSettings.FromJson(System.IO.File.ReadAllText("Config.json"));
                var task = ExecuteAsync(config);
                /*OracleFolderInstaller installer = new OracleFolderInstaller(config.ConnectionString[i]);
                installer.ScriptEvent += OnTry;
                installer.ErrorEvent += OnFail;
                installer.ExecuteFiles(files);*/
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
            Console.ReadKey();
        }

        private static async Task ExecuteAsync(ConfigSettings config)
        {
            List<string> files = OracleFolderInstaller.GetAllFilesRec(new List<string>(), config.InstallFolder, config.Ignore);
            Task[] tasks = new Task[config.ConnectionString.Length];
            for (int i = 0; i < config.ConnectionString.Length; i++)
            {
                OracleFolderInstaller installer = new OracleFolderInstaller(config.ConnectionString[i]);
                installer.ScriptEvent += OnTry;
                installer.ErrorEvent += OnFail;
                tasks[i] = Task.Run(() => installer.ExecuteFiles(files));
            }
            await Task.WhenAll(tasks);
            Console.WriteLine($"Success count = {success}");
            Console.WriteLine($"Errors count = {errors}.");
            Errors.ForEach(x => Console.WriteLine(x));
            Console.WriteLine("Over!");
        }

        private static void OnFail(object sender, OracleFolderInstaller.EventInfo info, string source, Exception exception)
        {
            Console.WriteLine($"{info.ToString()} :: {exception.Message}");
            lock (errorsLocker)
            {
                errors++;
                Errors.Add($"{source} :: { exception.Message}");
            }
        }

        private static void OnTry(object sender, OracleFolderInstaller.EventInfo info, OracleFolderInstaller.ScriptEventArgs args)
        {
            if (info == OracleFolderInstaller.EventInfo.SUCCESS)
            {
                Console.WriteLine($"{info.ToString()} :: {args.File}");
                lock (successLocker)
                {
                    success++;
                }
            }
        }
    }
}
