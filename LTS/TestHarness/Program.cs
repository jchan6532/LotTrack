using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Server.Views;
using Server.Models;

namespace TestHarness
{
    /// <summary>
    /// Class for the test harness project
    /// </summary>
    public class Program
    {
        public static List<clientLot> lots;

        /// <summary>
        /// Dummy funciton for listening to incoming TCP/IP requests
        /// </summary>
        public static void listen()
        {
            TcpListener listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 13000);
            listener.Start();
            while (true)
            {
                if (listener.Pending() == true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    NetworkStream networkStream = client.GetStream();
                    Byte[] buf = new Byte[256];
                    while (true)
                    {
                        networkStream.Read(buf, 0, buf.Length);
                        Console.WriteLine(Encoding.ASCII.GetString(buf));
                    }
                }
            }

        }

        /// <summary>
        /// Defines the entry point of the test harness.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {

            Dictionary<int, Lot> lots = new Dictionary<int, Lot>();
            List<int> list1 = new List<int>();
            list1.Add(1); list1.Add(2);
            lots.Add(1, new Lot(2, list1));

            List<int> list2 = new List<int>();
            list2.Add(100); list2.Add(300);
            lots.Add(2, new Lot(2, list2));

            string s = StaticView.GenerateLotInfoView(lots);

            Program.lots = new List<clientLot>();
            Program.ParseLotInfoPayLoad(s);

            Dictionary<int, Location> locations = new Dictionary<int, Location>();
            locations.Add(1, new Location());
            locations.Add(2, new Location());
            s = StaticView.GenerateLocationInfoView(locations);

            Console.ReadKey();
        }

        /// <summary>
        /// Dummy function for parsing lot information
        /// </summary>
        /// <param name="s">The s.</param>
        public static void ParseLotInfoPayLoad(string s)
        {
            string[] lotInfos = null;

            lotInfos = s.Split('&');

            Program.lots.Clear();
            foreach (string lotInfo in lotInfos)
            {
                clientLot lot = new clientLot();
                Dictionary<string, string> lotPropertiesKVP = new Dictionary<string, string>();
                string[] lotProperties = lotInfo.Split('|');
                foreach (string lotProperty in lotProperties)
                {
                    string[] kvp = lotProperty.Split('=');
                    string key = kvp[0];
                    string value = kvp[1];
                    lotPropertiesKVP.Add(key, value);
                }

                lot.lotID = Int32.Parse(lotPropertiesKVP[LotConstants.LOTID_STR]);
                lot.waferAmount = Int32.Parse(lotPropertiesKVP[LotConstants.WAFERAMOUNT_STR]);
                lot.lotState = Int32.Parse(lotPropertiesKVP[LotConstants.LOTSTATUS_STR]);
                lot.atLocationNumber = Int32.Parse(lotPropertiesKVP[LotConstants.ATLOCATIONNUMBER_STR]);

                string[] waferIDsStr = lotPropertiesKVP[LotConstants.WAFERIDS_STR].Split(',');
                foreach (string waferIDStr in waferIDsStr)
                {
                    lot.waferIDs.Add(Int32.Parse(waferIDStr));
                }
                Program.lots.Add(lot);
            }
            return;
        }
    }

    /// <summary>
    /// Dummy class for the Lot model
    /// </summary>
    public class clientLot
    {
        public int lotID;
        public int waferAmount;
        public int lotState;
        public int atLocationNumber;
        public List<int> waferIDs;

        public clientLot()
        {
            this.waferIDs = new List<int>();
        }
    }

    /// <summary>
    /// Dummy class for enumerating all constants related to the lot model
    /// </summary>
    public static class LotConstants
    {
        public static int UNDEFINED => -1;

        public static int WAITINGSTATE => 1;
        public static int ACTIVESTATE => 2;
        public static int PROCESSINGSTATE => 3;
        public static int STANDBYSTATE => 4;
        public static int INACTIVESTATE => 5;

        public static string LOTID_STR => "lotID";
        public static string WAFERAMOUNT_STR => "waferAmount";
        public static string LOTSTATUS_STR => "lotStatus";
        public static string ATLOCATIONNUMBER_STR => "atLocationNumber";
        public static string WAFERIDS_STR => "waferIDs";
    }
}
