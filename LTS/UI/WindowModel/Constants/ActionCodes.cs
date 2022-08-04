using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.WindowModel.Constants
{
    /// <summary>
    /// Enumerated set of action codes client can send to the server
    /// </summary>
    public static class ActionCodes
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
