using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

using UI.Models.Constants;
using UI.WindowModel.Constants;

namespace UI.Models
{
    /// <summary>
    /// Model for storing 1 user entry sent from the server
    /// </summary>
    public class User
    {
        #region Properties

        public int UserID;
        public string Password;

        #endregion


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        public User()
        {
            this.UserID = -1;
            this.Password = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class with parameters.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="password">The password.</param>
        public User(string userID, string password)
        {
            this.UserID = Int32.Parse(userID);
            this.Password = password;
        }

        #endregion
    }
}
