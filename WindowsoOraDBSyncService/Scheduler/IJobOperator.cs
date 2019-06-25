using OracleProcedureManager;
using System.Threading.Tasks;

namespace OraDBSyncService.Scheduler
{
    public class JobOperatorResponce
    {
        public readonly bool IsSuccess;
        public readonly string Description;
        public JobOperatorResponce(bool isSuccess, string desc)
        {
            IsSuccess = isSuccess;
            Description = desc;
        }
    }
    public interface IJobOperator
    {
        MainScheduler Scheduler { get; }

        Task<JobOperatorResponce> Operate(SynchronizationTask taskJob);
    }
}
