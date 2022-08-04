using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models.Constants
{
    /// <summary>
    /// All string and integer constants that relate to the dictionary for storing threads to move lots in the server
    /// </summary>
    public static class ThreadConstants
    {
        public static int MoveToLoc1Thread => 1;
        public static int MoveToLoc2Thread => 2;
        public static int MoveToLoc3Thread => 3;
        public static int MoveToLoc4Thread => 4;
        public static int MoveToLoc5Thread => 5;
        public static int MoveToLoc6Thread => 6;
        public static int MoveToLoc7Thread => 7;
        public static int MoveToLoc8Thread => 8;
        public static int MoveToLoc9Thread => 9;
        public static int MoveToLoc10Thread => 10;
        public static int MoveToWaitingLineThread => 11;
    }
}
