using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;

using Server.Controller.Constants;
using Server.Models.Constants;

namespace Server.Models
{
    /// <summary>
    /// Static class for encapsulating all functionality for querying the database
    /// </summary>
    public static class StaticDBConnector
    {
        #region Private Static Members

        private static MySqlConnection s_connection = new MySqlConnection();
        private static MySqlCommand s_command = null;

        private static DataTable s_table = new DataTable();
        private static MySqlDataAdapter s_adapter = null;

        #endregion


        #region Static Methods

        /// <summary>
        /// Connects to the database and gets the connection string.
        /// </summary>
        /// <returns>string: the database connection string</returns>
        public static string GetConnectionString()
        {
            string connectionString = string.Empty;
            string userName = ConfigurationManager.AppSettings.Get("UserName");
            string password = ConfigurationManager.AppSettings.Get("Password");
            string database = ConfigurationManager.AppSettings.Get("DataBase");
            string server = ConfigurationManager.AppSettings.Get("Server");
            string port = ConfigurationManager.AppSettings.Get("DBPort");
            connectionString = string.Format("server={0}; port={1}; userid={2}; password={3}; database={4};", server, port, userName, password, database);
            StaticDBConnector.s_connection.ConnectionString = connectionString;

            try
            {
                StaticDBConnector.s_connection.Open();
                StaticDBConnector.s_command = StaticDBConnector.s_connection.CreateCommand();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
            return connectionString;
        }

        /// <summary>
        /// Gets the number of entries of the column in the specified table with a specified where clause.
        /// </summary>
        /// <param name="table">The table name.</param>
        /// <param name="column">The column name.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <returns>Int64: the number of entries shown in the count column</returns>
        public static Int64 GetCount(string table, string column, string whereClause)
        {
            string query = string.Format("SELECT count({0}) FROM {1} WHERE {2}", column, table, whereClause);
            if(string.IsNullOrEmpty(whereClause) == true)
            {
                query = string.Format("SELECT count({0}) FROM {1}", column, table);
            }
            StaticDBConnector.s_command.CommandType = CommandType.Text;
            StaticDBConnector.s_command.CommandText = query;

            StaticDBConnector.s_command.ExecuteNonQuery();
            StaticDBConnector.s_adapter = new MySqlDataAdapter(StaticDBConnector.s_command);
            try
            {
                StaticDBConnector.s_table.Clear();
                StaticDBConnector.s_adapter.Fill(StaticDBConnector.s_table);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
            Int64 count = StaticDBConnector.s_table.Rows[0].Field<Int64>(string.Format("count({0})", column));
            return count;
        }

        /// <summary>
        /// Gets the specified field from an entry in a table.
        /// </summary>
        /// <param name="table">The table name.</param>
        /// <param name="column">The column name.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <returns>object: the field retrieved from the entry in the table</returns>
        public static object GetFieldFromEntry(string table, string column, string whereClause)
        {
            string query = string.Format("SELECT {0} FROM {1} WHERE {2};", column, table, whereClause);

            StaticDBConnector.s_command.CommandType = CommandType.Text;
            StaticDBConnector.s_command.CommandText = query;

            StaticDBConnector.s_command.ExecuteNonQuery();
            StaticDBConnector.s_adapter = new MySqlDataAdapter(StaticDBConnector.s_command);
            try
            {
                StaticDBConnector.s_table.Clear();
                StaticDBConnector.s_adapter.Fill(StaticDBConnector.s_table);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }

            object field = StaticDBConnector.s_table.Rows[0].Field<object>(column);
            return field;
        }

        /// <summary>
        /// Checks the login credentials with given user ID and password.
        /// </summary>
        /// <param name="enteredUserID">The entered user identifier.</param>
        /// <param name="enteredPassword">The entered password.</param>
        /// <returns>int: the response code specifying whether user has entered the correct credentials or not</returns>
        public static int CheckLoginCredentials(int enteredUserID, string enteredPassword)
        {
            Int64 count = StaticDBConnector.GetCount("users", "UserID", string.Format("UserID={0}", enteredUserID.ToString()));
            if (count != 1)
            {
                return ResponseCodes.UserNotRecognized;
            }
            string storedDBPassword = string.Empty;
            try
            {
                storedDBPassword = StaticDBConnector.GetFieldFromEntry("users", "Password", string.Format("UserID={0}", enteredUserID.ToString())).ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if(storedDBPassword != enteredPassword)
            {
                return ResponseCodes.IncorrectPassword;
            }

            return ResponseCodes.LogInSuccess;
        }

        /// <summary>
        /// Gets the amount of unassigned wafers left in the system.
        /// </summary>
        /// <param name="lastAssignedWaferID">The last assigned wafer identifier.</param>
        /// <returns>int: count of the amount of unassigned wafers left in the system</returns>
        public static int GetWafersLeft(int lastAssignedWaferID)
        {
            string whereClause = string.Format("{0}>{1}", WaferConstants.PKCol, lastAssignedWaferID);
            Int64 count =  StaticDBConnector.GetCount(WaferConstants.TableName, WaferConstants.PKCol, whereClause);
            return (int)count;
        }

        /// <summary>
        /// Inserts a specified entry to the table associating the columns to the values.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columns">The columns names.</param>
        /// <param name="values">The values of each column for the entry to be inserted.</param>
        public static void Insert(string tableName, string[] columns, string[] values)
        {
            string columnQuery = string.Empty;
            foreach(string column in columns)
            {
                columnQuery = columnQuery + column;
                columnQuery = columnQuery + ",";
            }
            columnQuery = columnQuery.Trim(',');

            string valueQuery = string.Empty;
            foreach (string value in values)
            {
                valueQuery = valueQuery + value;
                valueQuery = valueQuery + ",";
            }
            valueQuery = valueQuery.Trim(',');

            string query = string.Format("INSERT INTO {0} ({1}) values ({2})", tableName, columnQuery, valueQuery);

            StaticDBConnector.s_command.CommandType = CommandType.Text;
            StaticDBConnector.s_command.CommandText = query;

            StaticDBConnector.s_command.ExecuteNonQuery();
            return;
        }

        #endregion
    }
}
