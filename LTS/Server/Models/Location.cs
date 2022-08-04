using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Server.Models.Constants;

namespace Server.Models
{
    /// <summary>
    /// Model for storing 1 location entry from the database table
    /// </summary>
    public class Location
    {
        #region Properties

        public int LocationID;
        public int LocationNumber;
        public volatile int LocationState;
        public volatile int HasLotID;
        public volatile bool IsAssigned;

        public volatile bool IsVacant;
        public static int s_lastReadLocationID;

        #endregion


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Models.Location"/> class.
        /// </summary>
        public Location()
        {
            this.LocationID = Location.s_lastReadLocationID;
            int nextLocationNumber = 0;
            nextLocationNumber = int.Parse(StaticDBConnector.GetFieldFromEntry(LocationConstants.TableName, LocationConstants.LocationNumberCol, string.Format("{0}={1}", LocationConstants.PKCol, Location.s_lastReadLocationID)).ToString());
            this.LocationNumber = LocationConstants.Undefined;
            if (nextLocationNumber > 0)
            {
                this.LocationNumber = nextLocationNumber;
            }
            this.LocationState = LocationConstants.VacantState;
            this.HasLotID = LocationConstants.Undefined;
            this.IsVacant = true;
            this.IsAssigned = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Models.Location"/> class with parameters.
        /// </summary>
        /// <param name="locationID">The location identifier.</param>
        public Location(int locationID)
        {
            this.LocationID = locationID;
            int nextLocationNumber = 0;
            nextLocationNumber = Int32.Parse(StaticDBConnector.GetFieldFromEntry(LocationConstants.TableName, LocationConstants.LocationNumberCol, string.Format("{0}={1}", LocationConstants.PKCol, locationID.ToString())).ToString());
            this.LocationNumber = LocationConstants.Undefined;
            if(nextLocationNumber > 0)
            {
                this.LocationNumber = nextLocationNumber;
            }
            this.LocationState = LocationConstants.VacantState;
            this.HasLotID = LocationConstants.Undefined;
            this.IsVacant = true;
            this.IsAssigned = false;
        }

        #endregion


        #region Static Methods

        /// <summary>
        /// Gets the next location identifier.
        /// </summary>
        /// <returns>Int64: the next location ID read from the database</returns>
        public static Int64 GetNextLocationID()
        {
            int nextLocationID = Location.s_lastReadLocationID + 1;

            string whereClause = string.Empty;
            Int64 count = 0;
            try
            {
                whereClause = string.Format("{0}={1}", LocationConstants.PKCol, nextLocationID.ToString());
                count = StaticDBConnector.GetCount(LocationConstants.TableName, LocationConstants.PKCol, whereClause);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }

            if(count == (Int64)1)
            {
                Location.s_lastReadLocationID = nextLocationID;
            }
            return Location.s_lastReadLocationID;
        }

        /// <summary>
        /// Gets the next location number.
        /// </summary>
        /// <param name="currentLocationID">The current location identifier.</param>
        /// <returns>Int: the next location number</returns>
        public static int GetNextLocation(int currentLocationID)
        {
            if(currentLocationID == LocationConstants.WaiitngLine)
            {
                return LocationConstants.Location1;
            }

            if(currentLocationID == LocationConstants.Location10)
            {
                return LocationConstants.LotEnded;
            }

            if (currentLocationID == LocationConstants.Undefined)
            {
                return LocationConstants.LotEnded;
            }

            int nextLocationID = currentLocationID + 1;
            string whereClause = string.Format("{0}={1}", LocationConstants.PKCol, nextLocationID);
            string nextLocationNumber = StaticDBConnector.GetFieldFromEntry(LocationConstants.TableName, LocationConstants.LocationNumberCol, whereClause).ToString();
            return Int32.Parse(nextLocationNumber);
        }

        #endregion
    }
}
