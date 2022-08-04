using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Configuration;
using System.Threading;
using System.IO;

using Server.Controller.Constants;
using Server.Models;
using Server.Models.Constants;
using Server.Views;

namespace Server.Controller
{
    /// <summary>
    /// Class encapsulating the entire server architecture
    /// </summary>
    public class Server
    {
        #region Properties

        private IPAddress m_localIP;
        private IPEndPoint m_localEP;
        private int m_localPort;
        private TcpListener m_listener;

        public Dictionary<int, Location> Locations;
        public Dictionary<int, Lot> Lots;
        public Dictionary<int, Wafer> Wafers;
        public Dictionary<int, User> Sessions;
        public object LotLock = new object();
        public object LocationLock = new object();
        public object WaferLock = new object();
        public object UserLock = new object();

        public volatile bool StopRunning;
        public volatile bool WaitingLineVacant;
        public Dictionary<int, Thread> MoveLotThreads;
        public Dictionary<int, Thread> SessionThreads;
        public Thread AddNewWaferThread;

        private List<int> m_existingLocationIDs;
        private List<int> m_existingWaferIDs;
        private List<int> m_existingLotIDs;
        private List<int> m_onlineUserIDs;

        #endregion


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Server"/> class.
        /// </summary>
        public Server()
        {
            this.m_localIP = IPAddress.Parse(ConfigurationManager.AppSettings.Get("LocalIP"));
            this.m_localPort = Int32.Parse(ConfigurationManager.AppSettings.Get("LocalPort"));
            this.m_localEP = new IPEndPoint(this.m_localIP, this.m_localPort);
            this.m_listener = new TcpListener(this.m_localEP);
            try
            {
                StaticDBConnector.GetConnectionString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }

            this.Locations = new Dictionary<int, Location>();
            this.Lots = new Dictionary<int, Lot>();
            this.Wafers = new Dictionary<int, Wafer>();
            this.Sessions = new Dictionary<int, User>();
            this.LotLock = new object();
            this.LocationLock = new object();
            this.WaferLock = new object();
            this.UserLock = new object();

            this.StopRunning = false;
            this.WaitingLineVacant = true;
            this.MoveLotThreads = new Dictionary<int, Thread>();
            this.SessionThreads = new Dictionary<int, Thread>();

            this.m_existingLocationIDs = new List<int>();
            this.m_existingLotIDs = new List<int>();
            this.m_existingWaferIDs = new List<int>();
            this.m_onlineUserIDs = new List<int>();

            int nextID = 0;
            Location.s_lastReadLocationID = nextID;
            int iter = 0;
            for (iter = 0; iter < LocationConstants.NumberLocations; iter++)
            {
                nextID = (int)Location.GetNextLocationID();
                if(this.Locations.ContainsKey(nextID) == false)
                {
                    this.Locations.Add(nextID, new Location(nextID));
                    this.m_existingLocationIDs.Add(nextID);
                }
            }

            nextID = 0;
            Wafer.s_lastReadWaferID = nextID;
            for (iter = 0; iter < WaferConstants.InitialWaferAmount; iter++)
            {
                nextID = (int)Wafer.GetNextWaferID();
                if (this.Wafers.ContainsKey(nextID) == false)
                {
                    this.Wafers.Add(nextID, new Wafer(nextID));
                    this.m_existingWaferIDs.Add(nextID);
                }
                Wafer.s_lastAssignedWaferID = nextID;
            }
            nextID = 0;
            Wafer.s_lastAssignedWaferID = nextID;
            this.AddNewWaferThread = new Thread(this.AddNewWaferTHREAD);
            this.AddNewWaferThread.Start();

            Lot.s_lastReadLotID = (int)Lot.GetNextLotID();
        }

        #endregion

        /// <summary>
        /// Listens to incoming TCP/IP requests from clients.
        /// </summary>
        public void Listen()
        {
            int threadID = 1;
            this.m_listener.Start();
            while(this.StopRunning == false)
            {
                if(this.m_listener.Pending() == true)
                {
                    TcpClient client = this.m_listener.AcceptTcpClient();
                    //if(this.SessionThreads.ContainsKey(threadID) == false)
                    //{
                    //    this.SessionThreads.Add(threadID, new Thread(this.HandleClient));
                    //}
                    //else if(this.SessionThreads.ContainsKey(threadID) == true)
                    //{
                    //    if(this.SessionThreads[threadID] != null)
                    //    {
                    //        this.SessionThreads[threadID].Join();
                    //    }
                    //    this.SessionThreads[threadID] = new Thread(this.HandleClient);
                    //}
                    //this.SessionThreads[threadID].Start(client);
                    //threadID++;
                    this.HandleClient(client);
                }
                Thread.Sleep(10);
            }
            this.m_listener.Stop();
        }

        /// <summary>
        /// Stops the server from running and waiting for all background threads to finish running.
        /// </summary>
        public void Stop()
        {
            this.StopRunning = true;

            if (this.AddNewWaferThread != null)
            {
                this.AddNewWaferThread.Join();
            }

            foreach(KeyValuePair<int, Thread> moveLotThread in this.MoveLotThreads)
            {
                if(moveLotThread.Value != null)
                {
                    moveLotThread.Value.Join();
                }
            }

            foreach (KeyValuePair<int, Thread> sessionThread in this.SessionThreads)
            {
                if (sessionThread.Value != null)
                {
                    sessionThread.Value.Join();
                }
            }
            return;
        }

        /// <summary>
        /// Sends all lot information updates to all active clients subscribed to the server.
        /// </summary>
        public void SendLotUpdate()
        {
            string view = StaticView.GenerateLotInfoView(this.Lots);
            Byte[] responseBuffer = Encoding.ASCII.GetBytes(view);

            if(string.IsNullOrEmpty(view) == false)
            {
                foreach (KeyValuePair<int, User> user in this.Sessions)
                {
                    if (user.Value.IsLoggedIn == true)
                    {
                        try
                        {
                            if (user.Value.UserStream != null)
                            {
                                user.Value.UserStream.Write(responseBuffer, 0, responseBuffer.Length);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }

                }
            }

            Thread.Sleep(10);
        }

        /// <summary>
        /// Sends all location information updates to all active clients subscribed to the server.
        /// </summary>
        public void SendLocationUpdate()
        {
            string view = StaticView.GenerateLocationInfoView(this.Locations);
            Byte[] responseBuffer = Encoding.ASCII.GetBytes(view);

            foreach (KeyValuePair<int, User> user in this.Sessions)
            {
                if(user.Value.IsLoggedIn == true)
                {
                    try
                    {
                        if (user.Value.UserStream != null)
                        {
                            user.Value.UserStream.Write(responseBuffer, 0, responseBuffer.Length);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            Thread.Sleep(10);
        }

        /// <summary>
        /// Sends all wafer information updates to all active clients subscribed to the server.
        /// </summary>
        public void SendWaferUpdate()
        {
            string view = StaticView.GenerateWaferInfoView(this.Wafers);
            Byte[] responseBuffer = Encoding.ASCII.GetBytes(view);

            foreach (KeyValuePair<int, User> user in this.Sessions)
            {
                if (user.Value.IsLoggedIn == true)
                {
                    try
                    {
                        if (user.Value.UserStream != null)
                        {
                            user.Value.UserStream.Write(responseBuffer, 0, responseBuffer.Length);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }

            Thread.Sleep(10);
        }

        /// <summary>
        /// Handles 1 client the server received
        /// </summary>
        /// <param name="clientObj">The TCP client object.</param>
        public void HandleClient(object clientObj)
        {
            TcpClient client = clientObj as TcpClient;
            User currentUser = new User();
            currentUser.UserStream = client.GetStream();
            currentUser.UserStream.ReadTimeout = 100;
            currentUser.Client = client;

            Byte[] requestBuffer = new byte[256];
            string requestString = string.Empty;
            int bytesRead = 0;

            while ((currentUser.EndConversation == false) && (this.StopRunning == false))
            {
                requestString = string.Empty;
                do
                {
                    try
                    {
                        if(currentUser.UserStream != null)
                        {
                            bytesRead = currentUser.UserStream.Read(requestBuffer, 0, requestBuffer.Length);

                            if (bytesRead > 0)
                            {
                                requestString = requestString + Encoding.ASCII.GetString(requestBuffer, 0, bytesRead);
                            }
                        }
                    }
                    catch (IOException ioe)
                    {
                        bytesRead = 256;
                        if ((currentUser.EndConversation == true) || (this.StopRunning == true))
                        {
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        if ((currentUser.EndConversation == true) || (this.StopRunning == true))
                        {
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }
                } while (bytesRead == 256);

                string responseString = this.ParsePayLoad(requestString, currentUser);
                Byte[] responseBuffer = Encoding.ASCII.GetBytes(responseString);

                try
                {
                    if (currentUser.UserStream != null)
                    {
                        currentUser.UserStream.Write(responseBuffer, 0, responseBuffer.Length);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    if ((currentUser.EndConversation == true) || (this.StopRunning == true))
                    {
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            if ((currentUser.EndConversation == true) || (this.StopRunning == true))
            {
                lock (this.UserLock)
                {
                    this.Sessions.Remove(currentUser.UserID);
                }
                currentUser = null;
            }
            //client.Close();
        }

        /// <summary>
        /// Parses the request pay load the client sent.
        /// </summary>
        /// <param name="requestString">The request string.</param>
        /// <param name="currentUser">The current user.</param>
        /// <returns>string: the response string the server sends back to the client</returns>
        public string ParsePayLoad(string requestString, User currentUser)
        {
            string[] payLoadElements = null;
            string key = string.Empty;
            string value = string.Empty;
            if(requestString.Contains('|') == true) {
                payLoadElements = requestString.Split('|');
                foreach (string payLoadElement in payLoadElements)
                {
                    string[] keyValuePair = payLoadElement.Split('=');
                    key = keyValuePair[0];
                    value = keyValuePair[1];
                    if(currentUser.PayLoadData.ContainsKey(key) == true)
                    {
                        currentUser.PayLoadData[key] = value;
                    }
                    else
                    {
                        currentUser.PayLoadData.Add(key, value);
                    }
                }
            }
            else if (requestString.Contains('=') == true)
            {
                string[] keyValuePair = requestString.Split('=');
                key = keyValuePair[0];
                value = keyValuePair[1];
                if (currentUser.PayLoadData.ContainsKey(key) == true)
                {
                    currentUser.PayLoadData[key] = value;
                }
                else
                {
                    currentUser.PayLoadData.Add(key, value);
                }
            }


            string responseString = string.Empty;
            if (currentUser.PayLoadData.ContainsKey(PayloadElements.ActionCode) == true)
            {
                int actionCode = Int32.Parse(currentUser.PayLoadData[PayloadElements.ActionCode]);
                responseString = this.CheckActionCode(actionCode, currentUser);
            }
            return responseString;
        }

        /// <summary>
        /// Checks the action code sent from the client.
        /// </summary>
        /// <param name="actionCode">The action code.</param>
        /// <param name="currentUser">The current user.</param>
        /// <returns></returns>
        public string CheckActionCode(int actionCode, User currentUser)
        {
            int responseCode = 0;
            string responseString = string.Empty;
            if (actionCode == ClientActionCodes.LogIn)
            {
                string userID = currentUser.PayLoadData[PayloadElements.UserID];
                string password = currentUser.PayLoadData[PayloadElements.Password];
                responseCode = StaticDBConnector.CheckLoginCredentials(Int32.Parse(userID), password);

                if (responseCode == ResponseCodes.LogInSuccess)
                {
                    currentUser.UserID = Int32.Parse(userID);
                    currentUser.Password = password;
                    currentUser.IsLoggedIn = true;
                    if (this.Sessions.ContainsKey(currentUser.UserID) == false)
                    {
                        lock (this.UserLock)
                        {
                            this.Sessions.Add(currentUser.UserID, currentUser);
                        }
                        Thread.Sleep(10);

                        int userIDInt = currentUser.UserID;
                        Thread updateUserThread = new Thread(this.UpdateUserOnLoginTHREAD);
                        updateUserThread.Start();
                    }
                }
            }
            else if (actionCode == ClientActionCodes.LogOut){
                currentUser.EndConversation = true;
                currentUser.IsLoggedIn = false;
                foreach(KeyValuePair<int, Thread> moveLotThread in this.MoveLotThreads)
                {
                    if(moveLotThread.Value != null)
                    {
                        moveLotThread.Value.Join();
                    }
                }
                responseCode = ResponseCodes.LogOutSuccess;
            }
            else if (actionCode == ClientActionCodes.NewLot)
            {
                int wafersLeft = Wafer.s_lastReadWaferID - Wafer.s_lastAssignedWaferID;
                if(wafersLeft == 0)
                {
                    responseCode = ResponseCodes.WaferAmtIs0;
                }
                else
                {
                    int requestedAmount = 0;
                    if (currentUser.PayLoadData.ContainsKey(PayloadElements.WaferAmt) == true)
                    {
                        requestedAmount = Int32.Parse(currentUser.PayLoadData[PayloadElements.WaferAmt]);
                    }

                    if(requestedAmount > wafersLeft)
                    {
                        requestedAmount = wafersLeft;
                    }

                    if (this.WaitingLineVacant == false)
                    {
                        responseCode = ResponseCodes.WaitingLineFull;
                    }
                    else if (this.WaitingLineVacant == true)
                    {
                        this.WaitingLineVacant = false;
                        responseCode = this.CreateNewLot(requestedAmount);

                        if (requestedAmount > wafersLeft)
                        {
                            responseCode = ResponseCodes.WaferAmtIsLess;
                        }
                    }
                }               
            }
            else if (actionCode == ClientActionCodes.MoveLot)
            {
                int movelotID = 0;
                if(currentUser.PayLoadData.ContainsKey(PayloadElements.LotID) == true)
                {
                    movelotID = Int32.Parse(currentUser.PayLoadData[PayloadElements.LotID]);
                }

                if(this.Lots.ContainsKey(movelotID) == false)
                {
                    responseCode = ResponseCodes.InvalidLotID; // invalid lot ID
                }
                else
                {
                    int currentLocationID = this.Lots[movelotID].AtLocationID;
                    int nextLocationID = currentLocationID + 1;
                    
                    if(this.Lots[movelotID].LotState == LotStates.InactiveState)
                    {
                        responseCode = ResponseCodes.LotEndedJourney;
                    }
                    else if(this.Lots[movelotID].LotState != LotStates.StandbyState)
                    {
                        responseCode = ResponseCodes.LotStillInProcess;
                    }
                    else if(this.Lots[movelotID].LotState == LotStates.StandbyState)
                    {
                        if (this.Locations.ContainsKey(nextLocationID) == true)
                        {
                            if (this.Locations[nextLocationID].IsVacant == false)
                            {
                                responseCode = ResponseCodes.LocationOccupied; // location occupied
                            }
                        }

                        if (responseCode == 0)
                        {
                            if (this.Lots[movelotID].InUse == false)
                            {
                                lock (this.LotLock)
                                {
                                    this.Lots[movelotID].InUse = true;
                                }
                                lock (this.LocationLock)
                                {
                                    this.Locations[nextLocationID].IsAssigned = true;
                                }

                                ThreadParameterObj parameter = new ThreadParameterObj()
                                {
                                    OldLocationID = currentLocationID,
                                    NewLocationID = nextLocationID,
                                    LotToMoveID = movelotID,
                                    LotProcessingTime = this.Lots[movelotID].WaferAmount * WaferConstants.ProcessingTime
                                };
                                if (this.MoveLotThreads.ContainsKey(nextLocationID) == true)
                                {
                                    if (this.MoveLotThreads[nextLocationID] != null)
                                    {
                                        this.MoveLotThreads[nextLocationID].Join();
                                    }
                                    this.MoveLotThreads[nextLocationID] = new Thread(this.MoveAndProcessLotTHREAD);
                                }
                                else
                                {
                                    this.MoveLotThreads.Add(nextLocationID, new Thread(this.MoveAndProcessLotTHREAD));
                                }
                                this.MoveLotThreads[nextLocationID].Start(parameter);
                                responseCode = ResponseCodes.MoveLotSuccess;
                            }
                            else if(this.Lots[movelotID].InUse == true)
                            {
                                responseCode = ResponseCodes.LotInUse;
                            }
                        }
                    }
                }
            }
            responseString = string.Format("{0}={1}", PayloadElements.ResponseCode, responseCode);
            return responseString;
        }



        /// <summary>
        /// Creates a new lot.
        /// </summary>
        /// <param name="requestedAmount">The requested amount of wafers.</param>
        /// <returns>int: response code showing that the creation of the lot is successful</returns>
        public int CreateNewLot(int requestedAmount)
        {
            lock (this.LocationLock)
            {
                this.Locations[LocationConstants.Location1].IsAssigned = true;
            }

            int newLotID = Lot.s_lastReadLotID + 1;
            Lot.s_lastReadLotID = newLotID;
            List<int> chosenWaferIDs = new List<int>();
            for(int iter = 0; iter < requestedAmount; iter++)
            {
                int assigningWaferID = Wafer.s_lastAssignedWaferID + 1;
                Wafer.s_lastAssignedWaferID = assigningWaferID;
                chosenWaferIDs.Add(assigningWaferID);
                lock (this.WaferLock)
                {
                    this.Wafers[assigningWaferID].IsAssigned = true;
                    this.Wafers[assigningWaferID].AtLotID = newLotID;
                }
            }
            this.SendWaferUpdate();

            Lot newLot = new Lot(requestedAmount, chosenWaferIDs);
            newLot.InUse = false;
            newLot.AtLocationID = LocationConstants.WaiitngLine;
            lock (this.LotLock)
            {
                this.Lots.Add(newLotID, newLot);
            }

            string[] columns = new string[LotConstants.NumCols] { LotConstants.PKCOl, LotConstants.WaferAmountCol, LotConstants.LotStatusCol, LotConstants.AtLocationIDCol };
            string[] values = new string[LotConstants.NumCols] { Lot.s_lastReadLotID.ToString(), requestedAmount.ToString(), LotStates.WaitingState.ToString(), LocationConstants.WaiitngLine.ToString() };
            StaticDBConnector.Insert(LotConstants.TableName, columns, values);



            ThreadParameterObj parameter = new ThreadParameterObj()
            {
                OldLocationID = LocationConstants.WaiitngLine,
                LotToMoveID = newLotID,
                NewLocationID = LocationConstants.Location1,
                LotProcessingTime = this.Lots[newLotID].WaferAmount * WaferConstants.ProcessingTime
            };
            if (this.MoveLotThreads.ContainsKey(LocationConstants.Location1) == true)
            {
                if (this.MoveLotThreads[LocationConstants.Location1] != null)
                {
                    this.MoveLotThreads[LocationConstants.Location1].Join();
                }
                this.MoveLotThreads[LocationConstants.Location1] = new Thread(this.MoveAndProcessLotTHREAD);
            }
            else
            {
                this.MoveLotThreads.Add(LocationConstants.Location1, new Thread(this.MoveAndProcessLotTHREAD));
            }
            this.MoveLotThreads[LocationConstants.Location1].Start(parameter);
            return ResponseCodes.AddLotSuccess;
        }

        /// <summary>
        /// Background thread for moving and processing the lot for 1 location.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public void MoveAndProcessLotTHREAD(object parameter)
        {
            ThreadParameterObj LotAndLocations = parameter as ThreadParameterObj;

            this.SendLotUpdate();
            this.SendLocationUpdate();

            if (LotAndLocations.OldLocationID == LocationConstants.WaiitngLine)
            {
                do
                {
                    if(this.Locations[LotAndLocations.NewLocationID].IsVacant == true)
                    {
                        break;
                    }
                    Thread.Sleep(100);
                } while (this.Locations[LotAndLocations.NewLocationID].IsVacant == false);
            }
            else if (LotAndLocations.OldLocationID == LocationConstants.Location10)
            {
                lock (this.LotLock)
                {
                    this.Lots[LotAndLocations.LotToMoveID].LotState = LotStates.InactiveState;
                    this.Lots[LotAndLocations.LotToMoveID].AtLocationID = LocationConstants.Undefined;
                }

                lock (this.LocationLock)
                {
                    this.Locations[LotAndLocations.OldLocationID].IsVacant = true;
                    this.Locations[LotAndLocations.OldLocationID].LocationState = LocationState.Vacant;
                    this.Locations[LotAndLocations.OldLocationID].HasLotID = LocationConstants.Undefined;
                }
                this.SendLotUpdate();
                this.SendLocationUpdate();
                return;
            }


            // ON ROUTE
            Thread.Sleep(1000);
            lock (this.LotLock)
            {
                this.Lots[LotAndLocations.LotToMoveID].AtLocationID = LocationConstants.Undefined;
                this.Lots[LotAndLocations.LotToMoveID].LotState = LotStates.ActiveState;
            }

            if(LotAndLocations.OldLocationID == LocationConstants.WaiitngLine)
            {
                this.WaitingLineVacant = true;
            }
            else if (LotAndLocations.OldLocationID != LocationConstants.WaiitngLine)
            {
                lock (this.LocationLock)
                {
                    this.Locations[LotAndLocations.OldLocationID].LocationState = LocationState.Vacant;
                    this.Locations[LotAndLocations.OldLocationID].HasLotID = LocationConstants.Undefined;
                    this.Locations[LotAndLocations.OldLocationID].IsVacant = true;
                }
            }
            this.SendLotUpdate();
            this.SendLocationUpdate();



            // ARRIVED
            Thread.Sleep(LocationConstants.TransitTime);
            lock (this.LotLock)
            {
                this.Lots[LotAndLocations.LotToMoveID].AtLocationID = LotAndLocations.NewLocationID;
                this.Lots[LotAndLocations.LotToMoveID].LotState = LotStates.ProcessingState;
                this.Lots[LotAndLocations.LotToMoveID].InUse = false;
            }

            lock (this.LocationLock)
            {
                this.Locations[LotAndLocations.NewLocationID].IsAssigned = false;
                this.Locations[LotAndLocations.NewLocationID].IsVacant = false;
                this.Locations[LotAndLocations.NewLocationID].LocationState = LocationState.Occupied;
                this.Locations[LotAndLocations.NewLocationID].HasLotID = LotAndLocations.LotToMoveID;
            }
            this.SendLotUpdate();
            this.SendLocationUpdate();


            // AFTER PROCESSING
            Thread.Sleep(LotAndLocations.LotProcessingTime);
            lock (this.LotLock)
            {
                this.Lots[LotAndLocations.LotToMoveID].LotState = LotStates.StandbyState;
            }
            this.SendLotUpdate();
            this.SendLocationUpdate();


            return;
        }

        /// <summary>
        /// Background thread for updating all users upon user login.
        /// </summary>
        public void UpdateUserOnLoginTHREAD()
        {
            Thread.Sleep(10);
            this.SendLocationUpdate(); // WEARY
            this.SendLotUpdate();
            this.SendWaferUpdate();
        }

        /// <summary>
        /// Background thread for adding new wafers periodically to the system and database.
        /// </summary>
        public void AddNewWaferTHREAD()
        {
            do
            {
                Thread.Sleep(WaferConstants.AddNewWaferPeriod);
                this.AddNewWafer();
                this.SendWaferUpdate();
            } while (this.StopRunning == false);
        }

        /// <summary>
        /// Adds 1 new wafer to the system and the database.
        /// </summary>
        /// <returns></returns>
        public int AddNewWafer()
        {
            int nextWaferID = Wafer.s_lastReadWaferID + 1;
            Wafer.s_lastReadWaferID = nextWaferID;

            Wafer newWafer = new Wafer(nextWaferID);

            if (this.Wafers.ContainsKey(nextWaferID) == false)
            {
                this.Wafers.Add(nextWaferID, newWafer);
                this.m_existingWaferIDs.Add(nextWaferID);
            }

            //string[] columns = new string[WaferConstants.NUMCOLS] { WaferConstants.PK_COL, WaferConstants.LOTID_COL };
            //string[] values = new string[WaferConstants.NUMCOLS] { Wafer.lastReadWaferID.ToString(), WaferConstants.UNDEFINED.ToString()};
            //StaticDBConnector.Insert(WaferConstants.TABLENAME, columns, values);
            return 0;
        }
    }
}
