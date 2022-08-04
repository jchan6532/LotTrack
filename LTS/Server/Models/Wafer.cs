using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Server.Models.Constants;

namespace Server.Models
{
    /// <summary>
    /// Model for storing 1 wafer entry from the database table
    /// </summary>
    public class Wafer
    {
        #region Properties

        public int WaferID;
        public int AtLotID;
        public bool IsAssigned;

        public static int s_lastReadWaferID; // wafers going into LTS
        public static int s_lastAssignedWaferID; // wafers going out of LTS in FIFO fashion

        #endregion


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Wafer"/> class.
        /// </summary>
        public Wafer()
        {
            this.WaferID = Wafer.s_lastReadWaferID;
            this.AtLotID = WaferConstants.Undefined;
            this.IsAssigned = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Wafer"/> class with parameters.
        /// </summary>
        /// <param name="waferID">The wafer identifier.</param>
        public Wafer(int waferID)
        {
            this.WaferID = waferID;
            this.AtLotID = WaferConstants.Undefined;
            this.IsAssigned = false;
        }

        #endregion


        #region Static Methods

        /// <summary>
        /// Gets the next wafer identifier.
        /// </summary>
        /// <returns>Int64: the next wafer ID read from the database</returns>
        public static Int64 GetNextWaferID()
        {
            int nextWaferID = Wafer.s_lastReadWaferID + 1;

            string whereClause = string.Empty;
            Int64 count = 0;
            try
            {
                whereClause = string.Format("{0}={1}", WaferConstants.PKCol, nextWaferID.ToString());
                count = StaticDBConnector.GetCount(WaferConstants.TableName, WaferConstants.PKCol, whereClause);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }

            if (count == (Int64)1)
            {
                Wafer.s_lastReadWaferID = nextWaferID;
            }
            return Wafer.s_lastReadWaferID;
        }

        #endregion
    }
}
