using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models.Constants
{
    /// <summary>
    /// All string and integer constants that relate to the user model
    /// </summary>
    public static class UserConstants
    {
        public static int Undefined => -1;
        public static int Defined => 1;

        // string columns names
        public static string TableName => @"users";
        public static string PKCol => @"UserID";
        public static string PasswordCol => @"Password";
        public static string IsAdminCol => "isADMIN";
        public const int NumCols = 3;
    }
}
