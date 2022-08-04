using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models.Constants
{
    /// <summary>
    /// All string and integer constants that relate to the location model
    /// </summary>
    public static class LocationConstants
    {
        public static int Undefined => -1;
        public static int VacantState => 0;
        public static int OccupiedState => 1;


        // string column names for DB
        public static string TableName => @"locations";
        public static string PKCol => @"locationID";
        public static string LocationNumberCol => @"locationNumber";
        public static string LocationStateCol => @"locationState";
        public static string HasLotIDCol => @"hasLotID";
        public const int NumCols = 4;

        public static int NumberLocations => 10;
        public static int Location1 => 1;
        public static int Location2 => 2;
        public static int Location3 => 3;
        public static int Location4 => 4;
        public static int Location5 => 5;
        public static int Location6 => 6;
        public static int Location7 => 7;
        public static int Location8 => 8;
        public static int Location9 => 9;
        public static int Location10 => 10;
        public static int WaiitngLine => 0;
        public static int LotEnded => 11;

        public static int TransitTime => 5000;
    }
}
