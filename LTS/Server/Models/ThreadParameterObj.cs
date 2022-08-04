using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    /// <summary>
    /// thread parameter class for holding all the parameter information required in the thread when a lot is being moved
    /// </summary>
    public class ThreadParameterObj
    {
        public int OldLocationID;
        public int LotToMoveID;
        public int NewLocationID;
        public int LotProcessingTime;
    }
}
