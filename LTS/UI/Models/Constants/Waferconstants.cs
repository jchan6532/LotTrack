using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Models.Constants
{
    /// <summary>
    /// All string and integer constants that relate to the wafer model
    /// </summary>
    public static class Waferconstants
    {
        public static int WaferProcessingTime => 10;
        public static int MaxWaferAmount => 50;
        public static string WaferIDStr => "waferID";
        public static string BelongsToLotIDStr => "AtLotID";
        public static int Undefined => -1;
        public static int Defined => 1;
    }
}
