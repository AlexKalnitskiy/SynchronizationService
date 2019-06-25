using OracleProcedureManager;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OraDBSyncService.Scheduler
{
    public class MainScheduler
    {
        #region Singleton
        private static MainScheduler _singleton;

        public static MainScheduler GetMainScheduler()
        {
            if (_singleton != null)
                return _singleton;
            else
            {
                _singleton = new MainScheduler()
                {
                    _connectionString = ServiceSettings.Get().ConnectionString,
                    Scheduler = StdSchedulerFactory.GetDefaultScheduler().Result
            };
                _singleton.Scheduler.Start();
                return _singleton;
            }
        }
        #endregion Singleton

        #region Constructors
        private MainScheduler() { }
        #endregion Constructors

        #region Fields
        private string _connectionString;
        #endregion Fields

        //TODO Поле в бд, отвечающее за добавленность в расписание заданий. При запуске службы опрашивать это поле, чтобы узнать, что уже добавлено
        //TODO:??? Access Level?
        public IScheduler Scheduler;

        #region Methods: public

        public async Task StartTaskAsync(SynchronizationTask task)
        {
            if (CronExpression.IsValidExpression(task.CronExpression))
            {
                IJobDetail detail = JobBuilder.Create<BPMSyncJob>()
                    .WithIdentity(task.SyncTaskId) //Task key is equal to TaskId in bpm
                    .SetJobData(ImportTaskAsMap(task)).Build();
                await Scheduler.ScheduleJob(detail, GetTriggerFromCron(task.CronExpression));
            }
            else
                throw new SchedulerException("Unable to validate CronExpression to schedule job");

            ITrigger GetTriggerFromCron(string CronExpression)
            {
                return TriggerBuilder.Create()
                    .WithIdentity(task.SyncTaskId)
                    .StartNow().WithCronSchedule(CronExpression, x => x.WithMisfireHandlingInstructionFireAndProceed())
                    .Build();
            }
        }

        public async Task<bool> CheckTaskInSchedule(string taskId)
        {
            JobKey key = new JobKey(taskId);
            Scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            return await Scheduler.CheckExists(key);
        }

        public async Task<ITrigger> GetTriggetInfo(string taskId)
        {
            return await Scheduler.GetTrigger(new TriggerKey(taskId));
        }
        public async Task<bool> DeleteTaskWithInterruptAsync(string taskId)
        {
            await InterruptTask(taskId);
            JobKey key = new JobKey(taskId);
            Scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            return await Scheduler.DeleteJob(key);
        }
        public async Task<bool> InterruptTask(string taskId)
        {
            JobKey key = new JobKey(taskId);
            Scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            return await Scheduler.Interrupt(key);
        }
        public async Task TriggerTaskAsync(string taskId)
        {
            JobKey key = new JobKey(taskId);
            Scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await Scheduler.TriggerJob(key);
        }
        #endregion Methods: public

        #region Methods: private
        private JobDataMap ImportTaskAsMap(SynchronizationTask task)
        {
            return new JobDataMap() {
                new KeyValuePair<string, object>("connection", _connectionString),
                new KeyValuePair<string, object>("task", task) };
        }
        #endregion Methods: private
    }
}
