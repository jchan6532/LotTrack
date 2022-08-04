using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Server.Models.Constants
{
    /// <summary>
    /// All string and integer constants that relate to the wafer model
    /// </summary>
    public static class WaferConstants
    {
        public static int Undefined => -1;
        public static int Defined => 1;
        public static int ProcessingTime => Int32.Parse(ConfigurationManager.AppSettings.Get("WaferProcessingTime"));
        public static int InitialWaferAmount => Int32.Parse(ConfigurationManager.AppSettings.Get("InitialWaferAmount"));
        public static int WaferPerLot => Int32.Parse(ConfigurationManager.AppSettings.Get("MaxWafersPerLot"));
        public static int AddNewWaferPeriod => Int32.Parse(ConfigurationManager.AppSettings.Get("AddNewWaferPeriod"));


        // string column names for DB
        public static string TableName => "wafers";
        public static string PKCol => "waferID";
        public static string LotIDCol => "AtLotID";
        public const int NumCols = 2;
    }
}
