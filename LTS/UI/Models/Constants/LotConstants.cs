using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Models.Constants
{
    /// <summary>
    /// All string and integer constants that relate to the lot model
    /// </summary>
    public static class LotConstants
    {
        public static int Undefined => -1;

        public static int WaitingState => 1;
        public static int ActiveState => 2;
        public static int ProcessingState => 3;
        public static int StandbyState => 4;
        public static int InactiveState => 5;

        public static string LotIDStr => "lotID";
        public static string WaferAmountStr => "waferAmount";
        public static string LotStatusStr => "lotStatus";
        public static string AtLocationNumberStr => "atLocationNumber";
        public static string WaferIDsStr => "HasWaferIDs";
    }
}
