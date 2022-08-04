using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.WindowModel.Constants
{
    /// <summary>
    /// Enumerated set of location states stored on the server side
    /// </summary>
    public static class ServerLocationState
    {
        public static int Vacant => 0;
        public static int Occupied => 1;

    }
}
