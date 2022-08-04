using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Server.Models;
using Server.Models.Constants;

namespace Server.Views
{
    /// <summary>
    /// Static class for providing payloads and response strings for lot/wafer/location information in the system
    /// </summary>
    public static class StaticView
    {
        #region Static Methods

        /// <summary>
        /// Generates the lot information view/string.
        /// </summary>
        /// <param name="lots">The lots in the system.</param>
        /// <returns>string: the response string with separation delimiters about all lot information in the system</returns>
        public static string GenerateLotInfoView(Dictionary<int, Lot> lots)
        {
            string finalView = string.Empty;

            try
            {
                foreach (KeyValuePair<int, Lot> lot in lots)
                {
                    finalView = finalView + string.Format("{0}={1}", LotConstants.PKCOl, lot.Value.LotID.ToString());
                    finalView = finalView + "|";
                    finalView = finalView + string.Format("{0}={1}", LotConstants.WaferAmountCol, lot.Value.WaferAmount.ToString());
                    finalView = finalView + "|";
                    finalView = finalView + string.Format("{0}={1}", LotConstants.LotStatusCol, lot.Value.LotState.ToString());
                    finalView = finalView + "|";
                    finalView = finalView + string.Format("{0}={1}", LotConstants.AtLocationNumberStr, lot.Value.AtLocationID.ToString()); //fix later

                    string listOfWaferIDs = string.Empty;
                    foreach (int waferID in lot.Value.WaferIDs)
                    {
                        listOfWaferIDs = listOfWaferIDs + waferID + ",";
                    }
                    listOfWaferIDs = listOfWaferIDs.Trim(',');
                    finalView = finalView + "|" + string.Format("{0}={1}", LotConstants.WaferIDsStr, listOfWaferIDs);
                    finalView = finalView + "&";
                }
                finalView = finalView.Trim('&');
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }

            return finalView;
        }

        /// <summary>
        /// Generates the location information view/string.
        /// </summary>
        /// <param name="locations">The locations in the system.</param>
        /// <returns>string: the response string with separation delimiters about all location information in the system</returns>
        public static string GenerateLocationInfoView(Dictionary<int, Location> locations)
        {
            string finalView = string.Empty;

            try
            {
                foreach (KeyValuePair<int, Location> location in locations)
                {
                    finalView = finalView + string.Format("{0}={1}", LocationConstants.PKCol, location.Value.LocationID.ToString());
                    finalView = finalView + "|";
                    finalView = finalView + string.Format("{0}={1}", LocationConstants.LocationNumberCol, location.Value.LocationNumber.ToString());
                    finalView = finalView + "|";
                    finalView = finalView + string.Format("{0}={1}", LocationConstants.LocationStateCol, location.Value.LocationState.ToString());
                    finalView = finalView + "|";
                    finalView = finalView + string.Format("{0}={1}", LocationConstants.HasLotIDCol, location.Value.HasLotID.ToString());

                    finalView = finalView + "&";
                }
                finalView = finalView.Trim('&');
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }

            return finalView;
        }

        /// <summary>
        /// Generates the wafer information view/string.
        /// </summary>
        /// <param name="wafers">The wafers.</param>
        /// <returns>string: the response string with separation delimiters about all wafer information in the system</returns>
        public static string GenerateWaferInfoView(Dictionary<int, Wafer> wafers)
        {
            string finalView = string.Empty;
            try
            {
                foreach (KeyValuePair<int, Wafer> wafer in wafers)
                {
                    finalView = finalView + string.Format("{0}={1}", WaferConstants.PKCol, wafer.Value.WaferID);
                    finalView = finalView + "|";
                    finalView = finalView + string.Format("{0}={1}", WaferConstants.LotIDCol, wafer.Value.AtLotID);

                    finalView = finalView + "&";
                }
                finalView = finalView.Trim('&');
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }

            return finalView;
        }

        #endregion
    }
}
