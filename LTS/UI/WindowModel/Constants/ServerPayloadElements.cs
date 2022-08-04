using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.WindowModel.Constants
{
    /// <summary>
    /// Enumerated set of payload string keys stored on the server side
    /// </summary>
    public static class ServerPayloadElements
    {
        public static string ActionCode => "action";

        public static string UserID => "userID";
        public static string Password => "Password";
        public static string IsAdmin => "isAdmin";
        public static string WaferAmt => "waferAmount";
        public static string LotID => "lotID";
        public static string LotInfo => "lotinfo";
        public static string LocationInfo => "locationinfo";

        public static string ResponseCode => "response";
    }
}
