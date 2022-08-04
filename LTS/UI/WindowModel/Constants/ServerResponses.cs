using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.WindowModel.Constants
{
    /// <summary>
    /// Enumerated set of string constants containing messages that correlate to a response code the server sends
    /// </summary>
    public static class ServerResponses
    {
        public static string UserIDNotNumeric => "User ID has to be numeric";
        public static string UserIDNumeric => "";
        public static string PasswordEmpty => "Password cannot be empty";
        public static string PasswordNonEmpty => "";
        public static string UserNotRecognized => "User ID is invalid";
        public static string IncorrectPassword => "Incorrect password";
        public static string LogInSuccess => "";
        public static string LogOutSuccess => "";
        public static string LogOutFailed => "something happened on the server and logout failed";
        public static string WaferAmtNotNumeric => "wafer amount has to be a number";
        public static string WaferAmtClientSideChecked => "";
        public static string AddLotSuccess => "lot added successfully";
        public static string WaferAmountIs0 => "wafers left in the system has reached 0";
        public static string WaferTooLittle => "wafer amount must be more than 0";
        public static string WaferAmtIsLess => "wafers left in the system is less than the requested amount, requested amount changed to that instead";
        public static string WaferAmtTooMuch => "requested amount is too much for 1 lot";
        public static string InvalidLotID => "lot ID is invalid as it does not exist in the system";
        public static string LotIDNotNumeric => "lot ID has to be numeric";
        public static string LocationOccupied => "next location is currently occupied";
        public static string LotStillProcessing => "lot is still being processed, be patient";
        public static string MoveLotSuccess => "lot was moved to the next location successfully";
        public static string LotEndedJourney => "lot ended journey";
        public static string LotInUse => "lot still being used by operator";
    }
}
