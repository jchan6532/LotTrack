using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Controller.Constants
{
    /// <summary>
    /// Enumerated set of all possible response codes sent by the server
    /// </summary>
    public static class ResponseCodes
    {
        public static int LotEndedJourney => 6;
        public static int LogOutSuccess => 5;
        public static int LogInSuccess => 4;
        public static int AddLotSuccess => 3;
        public static int GetInfoSuccess => 2;
        public static int MoveLotSuccess => 1;
        public static int Failure => 0;
        public static int UserNotRecognized => -1;
        public static int IncorrectPassword => -2;
        public static int InvalidLotID => -3;
        public static int LocationOccupied => -4;
        public static int UnableGetLotInfo => -5;
        public static int UnableGetLocationInfo => -6;
        public static int WaferAmtTooMuch => -7;
        public static int WaferAmtIs0 => -8;
        public static int WaferAmtIsLess => -9;
        public static int WaitingLineFull => -10;
        public static int LogOutFail => -11;
        public static int LotStillInProcess => -12;
        public static int LotInUse => -13;
    }
}
