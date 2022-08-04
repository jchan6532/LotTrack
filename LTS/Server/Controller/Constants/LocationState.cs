using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Controller.Constants
{
    /// <summary>
    /// Class for storing the 2 states a location can be, occupied and vacant
    /// </summary>
    public static class LocationState
    {
        public static int Vacant => 0;
        public static int Occupied => 1;

    }
}
