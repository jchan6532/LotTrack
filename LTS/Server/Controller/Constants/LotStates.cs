using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Controller.Constants
{
    /// <summary>
    /// Enumerated set of all possible lot states a lot can be in
    /// </summary>B 
    public static class LotStates
    {
        public static int Undefined => -1;
        public static int WaitingState => 1;
        public static int ActiveState => 2;
        public static int ProcessingState => 3;
        public static int StandbyState => 4;
        public static int InactiveState => 5;
    }
}
