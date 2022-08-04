using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using Server.Models.Constants;
using Server.Controller.Constants;

namespace Server.Models
{
    /// <summary>
    /// Model for storing 1 lot entry from the database table
    /// </summary>
    public class Lot
    {
        #region Properties

        public int LotID;
        public int WaferAmount;
        public List<int> WaferIDs;
        public volatile int LotState;
        public volatile int AtLocationID;
        public volatile bool InUse;

        public static int s_lastReadLotID;

        #endregion


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Lot"/> class.
        /// </summary>
        public Lot()
        {
            this.LotID = Lot.s_lastReadLotID;
            this.WaferAmount = LotConstants.Undefined;
            this.LotState = LotConstants.WaitingState;
            this.AtLocationID = LotConstants.Undefined;
            this.WaferIDs = new List<int>();
            this.InUse = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Lot"/> class with parameters.
        /// </summary>
        /// <param name="waferAmount">The wafer amount.</param>
        /// <param name="waferIDs">The wafer IDs.</param>
        public Lot(int waferAmount, IEnumerable<int> waferIDs)
        {
            this.LotID = Lot.s_lastReadLotID;
            this.WaferAmount = waferAmount;
            this.LotState = LotConstants.WaitingState;
            this.AtLocationID = LotConstants.Undefined;
            this.WaferIDs = (List<int>)waferIDs;
            this.InUse = false;
        }

        #endregion


        #region Static Methods

        /// <summary>
        /// Gets the next lot identifier.
        /// </summary>
        /// <returns>Int64: the next lot ID read from the database</returns>
        public static Int64 GetNextLotID()
        {
            string whereClause = string.Empty;
            Int64 count = 0;
            try
            {
                whereClause = string.Empty;
                count = StaticDBConnector.GetCount(LotConstants.TableName, LotConstants.PKCOl, whereClause);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }

            if (count > (Int64)0)
            {
                Lot.s_lastReadLotID = (int)count;
            }
            return Lot.s_lastReadLotID;
        }

        #endregion


    }


}
