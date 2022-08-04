using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Controller.Constants
{
    /// <summary>
    /// Class for storing an enumerated set of all possible client actions, each action corresponding to a unique code
    /// </summary>
    public static class ClientActionCodes
    {
        public static int NewLot => 1;
        public static int MoveLot => 2;
        public static int GetLotInfo => 3;
        public static int GetLocationInfo => 4;
        public static int GetAllInfo => 5;
        public static int LogIn => 6;
        public static int LogOut => 7;
    }
}
