using System;
using System.Diagnostics;

namespace XTensionProject
{
    public static class TimeoutExt
    {
        /// <summary>
        /// Locks current thread temporarily, until given codition is met or timeout reached
        /// </summary>
        /// <param name="condition">condition to met</param>
        /// <param name="timeout">timeout time in ms</param>
        /// <param name="frequency">frequency check in ms</param>
        /// <returns></returns>
        public static bool WaitForCondition(Func<bool> condition, long timeout = 10000, int frequency = 25)
        {
            Stopwatch st = Stopwatch.StartNew();
            while (st.ElapsedMilliseconds < timeout)
            {
                if (condition())
                {
                    st.Stop();
                    return true;
                }
                System.Threading.Thread.Sleep(frequency);
            }
            st.Stop();
            return false;
        }
    }
}