using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Server.Models;
using Server.Controller.Constants;
using Server.Controller;

namespace Server
{
    /// <summary>
    /// Class for containing the main entry point for the server
    /// </summary>
    class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {
            Controller.Server server = new Controller.Server();
            server.Listen();
        }
    }
}
