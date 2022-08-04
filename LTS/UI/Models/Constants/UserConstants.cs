using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Models.Constants
{
    /// <summary>
    /// All string and integer constants that relate to the user model
    /// </summary>
    public static class UserConstants
    {
        public static int UserIDIsNotNumeric => -1;
        public static int PasswordIsEmpty => -2;
        public static int UserIDIsNumeric => 1;
        public static int PasswordNonEmpty => 2;
    }
}
