using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OracleProcedureManager
{
    public class TaskStateConstants
    {
        public const int NotInSchedule = 0;
        public const int Ready = 1;
        public const int Executing = 2;
        public const int Failed = 3;
    }
}

