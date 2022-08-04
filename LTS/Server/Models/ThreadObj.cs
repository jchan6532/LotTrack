using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Server.Models
{
    /// <summary>
    /// Thread class for encapsulating all thread objects including threads, thread starts, and parameterized thread starts
    /// </summary>
    public class ThreadObj
    {
        public Thread Thread;
        public ThreadStart ThreadStart;
        public ParameterizedThreadStart ParameterizedThreadStart;
    }
}
