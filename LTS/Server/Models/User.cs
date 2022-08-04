using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Server.Models.Constants;
using Server.Controller.Constants;

namespace Server.Models
{
    /// <summary>
    /// Model for storing 1 user entry from the database table
    /// </summary>
    public class User
    {
        #region Properties

        public int UserID;
        public string Password;
        public bool IsAdmin;
        public NetworkStream UserStream;
        public TcpClient Client;
        public Dictionary<string, string> PayLoadData;
        public volatile bool EndConversation;
        public volatile bool IsLoggedIn;

        public static ThreadStart s_clientThreadStart;
        public static Thread s_clientThread;

        #endregion


        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        public User()
        {
            this.UserID = UserConstants.Undefined;
            this.Password = string.Empty;
            this.IsAdmin = false;
            this.UserStream = null;
            this.Client = null;

            this.PayLoadData = new Dictionary<string, string>();
            this.PayLoadData.Add(PayloadElements.UserID, string.Empty);
            this.PayLoadData.Add(PayloadElements.Password, string.Empty);
            this.PayLoadData.Add(PayloadElements.LotID, string.Empty);
            this.PayLoadData.Add(PayloadElements.WaferAmt, string.Empty);
            this.PayLoadData.Add(PayloadElements.ActionCode, string.Empty);
            this.PayLoadData.Add(PayloadElements.ResponseCode, string.Empty);
            this.PayLoadData.Add(PayloadElements.LotInfo, string.Empty);
            this.PayLoadData.Add(PayloadElements.LocationInfo, string.Empty);

            this.EndConversation = false;
            this.IsLoggedIn = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class with parameters.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="password">The password.</param>
        public User(int userID, string password)
        {
            this.UserID = UserConstants.Undefined;
            if(userID > 0)
            {
                this.UserID = userID;
            }
            this.Password = string.Empty;
            if(string.IsNullOrEmpty(password) == false)
            {
                this.Password = password;
            }

            this.IsAdmin = User.GetAccountTypeFromID(userID);
            this.UserStream = null; // fix later

            this.PayLoadData = new Dictionary<string, string>();
            this.PayLoadData.Add(PayloadElements.UserID, string.Empty);
            this.PayLoadData.Add(PayloadElements.Password, string.Empty);
            this.PayLoadData.Add(PayloadElements.LotID, string.Empty);
            this.PayLoadData.Add(PayloadElements.WaferAmt, string.Empty);

            this.EndConversation = false;
            this.IsLoggedIn = false;
        }

        #endregion


        #region Static Methods

        /// <summary>
        /// Gets the account type from user identifier.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <returns>bool: true if user is ADMIN type, false if user is not ADMIN type</returns>
        private static bool GetAccountTypeFromID(int userID)
        {
            int isAdmin = 0;
            isAdmin = Int32.Parse(StaticDBConnector.GetFieldFromEntry(UserConstants.TableName, UserConstants.IsAdminCol, string.Format("{0}={1}", UserConstants.PKCol, userID.ToString())).ToString());

            if(isAdmin == 1)
            {
                return true;
            }
            else if(isAdmin == 0)
            {
                return false;
            }
            return false;
        }

        #endregion
    }
}
