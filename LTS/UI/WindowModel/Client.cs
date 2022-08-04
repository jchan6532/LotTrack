using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Data;
using System.IO;

using UI.Models;
using UI.WindowModel.Constants;
using UI.Models.Constants;

namespace UI.WindowModel
{
    /// <summary>
    /// Class for encapsulating the entire client side architecture
    /// </summary>
    public class Client
    {
        #region Properties

        public TcpClient TcpClient;
        public string RemoteIP;
        public int RemotePort;
        public User User;
        public string UserID;
        public string Password;
        public NetworkStream UserStream;
        public volatile bool ResponseReceived;

        private Byte[] m_requestBuffer;
        private Byte[] m_responseBuffer;
        private string m_requestString;
        private string m_responseString;
        public int BytesRead;
        public Dictionary<string, string> PayloadData;

        public ObservableCollection<Lot> Lots = new ObservableCollection<Lot>();
        public ObservableCollection<Location> Locations = new ObservableCollection<Location>();
        public ObservableCollection<Wafer> Wafers = new ObservableCollection<Wafer>();
        public object LotLock = new object();
        public object LocationLock = new object();
        public object WaferLock = new object();
        public Thread UpdateDataThread;
        public volatile bool ClientStop;

        #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class initializing necessary variables
        /// </summary>
        public Client()
        {
            this.RemoteIP = ConfigurationManager.AppSettings.Get("LocalIP");
            this.RemotePort = Int32.Parse(ConfigurationManager.AppSettings.Get("LocalPort"));
            this.User = null;
            this.UserID = string.Empty;
            this.Password = string.Empty;
            this.UserStream = null;

            this.m_requestBuffer = null;
            this.m_responseBuffer = null;
            this.m_requestString = string.Empty;
            this.m_responseString = string.Empty;
            this.BytesRead = 0;
            this.PayloadData = new Dictionary<string, string>();
            this.ResponseReceived = false;

            this.Lots = new ObservableCollection<Lot>();
            this.Locations = new ObservableCollection<Location>();
            this.Wafers = new ObservableCollection<Wafer>();
            BindingOperations.EnableCollectionSynchronization(this.Lots, LotLock);
            BindingOperations.EnableCollectionSynchronization(this.Locations, LocationLock);
            BindingOperations.EnableCollectionSynchronization(this.Wafers, WaferLock);
            this.ClientStop = false;
            this.UpdateDataThread = null;
        }

        /// <summary>
        /// Background thread for listening to incoming TCP/IP messages from the server.
        /// </summary>
        public void ListenForMsgsTHREAD()
        {
            int bytesRead = 0;
            this.UserStream.ReadTimeout = 100;

            while (this.ClientStop == false)
            {
                this.m_responseBuffer = new byte[256];
                this.m_responseString = string.Empty;

                do
                {
                    try
                    {
                        bytesRead = this.UserStream.Read(this.m_responseBuffer, 0, this.m_responseBuffer.Length);
                        if (bytesRead > 0)
                        {
                            this.m_responseString = this.m_responseString + Encoding.ASCII.GetString(this.m_responseBuffer, 0, bytesRead);
                        }
                    }
                    catch (IOException ioe)
                    {
                        bytesRead = 256;
                        if (this.ClientStop == true)
                        {
                            break;
                        }
                        continue;
                    }
                } while (bytesRead == 256);
                if (this.ClientStop == true)
                {
                    break;
                }

                if (this.m_responseString.Contains(ServerPayloadElements.ResponseCode) == true)
                {
                    this.ParseAndStoreResponse();
                    this.ResponseReceived = true;
                }
                else if (this.m_responseString.Contains(LotConstants.LotIDStr) == true)
                {
                    this.ParseAndStoreLotInfoPayLoad(this.m_responseString);
                }
                else if (this.m_responseString.Contains(LocationConstants.LocationIDStr) == true)
                {
                    this.ParseAndStoreLocationInfoPayLoad(this.m_responseString);
                }
                else if (this.m_responseString.Contains(Waferconstants.WaferIDStr) == true)
                {
                    this.ParseAndStoreWaferInfoPayLoad(this.m_responseString);
                }
            }

            return;
        }

        /// <summary>
        /// Connects to the remote server upon login with the specified user identifier and password combination.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="password">The password.</param>
        public void Connect(int userID, string password)
        {
            try
            {
                this.TcpClient = new TcpClient(this.RemoteIP, this.RemotePort);
            }
            catch (Exception e)
            {
                throw new Exception("connect to server failed");
            }

            if(this.UserStream == null)
            {
                try
                {
                    this.UserStream = this.TcpClient.GetStream();
                }
                catch (Exception)
                {
                    throw new Exception("network stream could not be created");
                }
            }

            if(this.UpdateDataThread == null)
            {
                this.ClientStop = false;
                this.UpdateDataThread = new Thread(this.ListenForMsgsTHREAD);
                this.UpdateDataThread.Name = "update lot info data background thread";
                this.UpdateDataThread.Start();
            }

            string actionPayload = string.Format("{0}={1}", ServerPayloadElements.ActionCode, ActionCodes.LogIn);
            string userIDPayload = string.Format("{0}={1}", ServerPayloadElements.UserID, userID.ToString());
            string passwordPayload = string.Format("{0}={1}", ServerPayloadElements.Password, password);
            this.m_requestString = string.Format("{0}|{1}|{2}", actionPayload, userIDPayload, passwordPayload);
            this.m_requestBuffer = Encoding.ASCII.GetBytes(this.m_requestString);

            this.ResponseReceived = false;
            try
            {
                this.UserStream.Write(this.m_requestBuffer, 0, this.m_requestBuffer.Length);
            }
            catch (Exception)
            {
                throw new Exception("write to server failed");
            }
            Thread.Sleep(10);
        }

        /// <summary>
        /// Disconnects from the remote server upon logout and joins any necessary threads and resets any necessary members.
        /// </summary>
        public void Disconnect()
        {
            this.m_requestString = string.Format("{0}={1}", ServerPayloadElements.ActionCode, ActionCodes.LogOut);
            this.m_requestBuffer = Encoding.ASCII.GetBytes(this.m_requestString);
            this.ResponseReceived = false;

            this.UserStream.Write(this.m_requestBuffer, 0, this.m_requestBuffer.Length);
            Thread.Sleep(10);
        }

        /// <summary>
        /// Sends the action to request for a new lot to be created to the server.
        /// </summary>
        /// <param name="waferAmount">The requested wafer amount.</param>
        public void SendNewLotInfo(string waferAmount)
        {
            string actionPayload = string.Format("{0}={1}", ServerPayloadElements.ActionCode, ActionCodes.NewLot);
            string waferAmountPayload = string.Format("{0}={1}", ServerPayloadElements.WaferAmt, waferAmount);
            this.m_requestString = string.Format("{0}|{1}", actionPayload, waferAmountPayload);

            this.m_requestBuffer = Encoding.ASCII.GetBytes(this.m_requestString);

            this.ResponseReceived = false;
            this.UserStream.Write(this.m_requestBuffer, 0, this.m_requestBuffer.Length);
            Thread.Sleep(10);
        }

        public void SendMoveLotInfo(string lotID)
        {
            string actionPayload = string.Format("{0}={1}", ServerPayloadElements.ActionCode, ActionCodes.MoveLot);
            string lotIDPayload = string.Format("{0}={1}", ServerPayloadElements.LotID, lotID);
            this.m_requestString = string.Format("{0}|{1}", actionPayload, lotIDPayload);

            this.m_requestBuffer = Encoding.ASCII.GetBytes(this.m_requestString);

            this.ResponseReceived = false;
            this.UserStream.Write(this.m_requestBuffer, 0, this.m_requestBuffer.Length);
            Thread.Sleep(10);
        }


        /// <summary>
        /// Parses and stores the server response into a dictionary.
        /// </summary>
        public void ParseAndStoreResponse()
        {
            string[] payLoadElements = null;
            payLoadElements = this.m_responseString.Split('=');
            string key = payLoadElements[0];
            string value = payLoadElements[1];
            if (this.PayloadData.ContainsKey(key) == true)
            {
                this.PayloadData[key] = value;
            }
            else
            {
                this.PayloadData.Add(key, value);
            }
            return;
        }

        /// <summary>
        /// Parses and stored wafer information into the wafer collection.
        /// </summary>
        /// <param name="newData">The incoming wafer information from the server.</param>
        public void ParseAndStoreWaferInfoPayLoad(string newData)
        {
            string[] waferInfos = null;

            waferInfos = newData.Split('&');


            lock (WaferLock)
            {
                Wafers.Clear();
            }
            foreach (string waferInfo in waferInfos)
            {
                Wafer wafer = new Wafer();
                Dictionary<string, string> waferPropertiesKVP = new Dictionary<string, string>();
                string[] waferProperties = waferInfo.Split('|');
                foreach (string waferProperty in waferProperties)
                {
                    string[] kvp = waferProperty.Split('=');
                    string key = kvp[0];
                    string value = kvp[1];
                    if (waferPropertiesKVP.ContainsKey(key) == true)
                    {
                        waferPropertiesKVP[key] = value;
                    }
                    else
                    {
                        waferPropertiesKVP.Add(key, value);
                    }
                }

                wafer.WaferID = Int32.Parse(waferPropertiesKVP[Waferconstants.WaferIDStr]);
                wafer.BelongsToLotID = waferPropertiesKVP[Waferconstants.BelongsToLotIDStr];
                if(wafer.BelongsToLotID == Waferconstants.Undefined.ToString())
                {
                    wafer.BelongsToLotID = "NOT ASSIGNED";
                }

                lock (WaferLock)
                {
                    Wafers.Add(wafer);
                }
            }
            return;
        }

        /// <summary>
        /// Parses and stores location information into the location collection.
        /// </summary>
        /// <param name="newData">The incoming location information from the server.</param>
        public void ParseAndStoreLocationInfoPayLoad(string newData)
        {
            string[] locationInfos = null;

            locationInfos = newData.Split('&');


            lock (LocationLock)
            {
                Locations.Clear();
            }
            foreach (string locationInfo in locationInfos)
            {
                Location location = new Location();
                Dictionary<string, string> locationPropertiesKVP = new Dictionary<string, string>();
                string[] locationProperties = locationInfo.Split('|');
                foreach (string locationProperty in locationProperties)
                {
                    string[] kvp = locationProperty.Split('=');
                    string key = kvp[0];
                    string value = kvp[1];
                    if (locationPropertiesKVP.ContainsKey(key))
                    {
                        locationPropertiesKVP[key] = value;
                    }
                    else
                    {
                        locationPropertiesKVP.Add(key, value);
                    }
                }

                location.LocationID = Int32.Parse(locationPropertiesKVP[LocationConstants.LocationIDStr]);
                location.LocationNumber = Int32.Parse(locationPropertiesKVP[LocationConstants.LocationNumberStr]);
                location.LocationState = Int32.Parse(locationPropertiesKVP[LocationConstants.LocationStateStr]);
                location.HasLotID = locationPropertiesKVP[LocationConstants.HasLotIDStr];
                if(location.HasLotID == LocationConstants.Undefined.ToString())
                {
                    location.HasLotID = "NO LOT";
                }

                if(location.LocationState == ServerLocationState.Occupied)
                {
                    location.LocationStateStr = "OCCUPIED";
                }
                else if (location.LocationState == ServerLocationState.Vacant)
                {
                    location.LocationStateStr = "VACANT";
                }

                lock (LocationLock)
                {
                    Locations.Add(location);
                }
            }
            return;
        }

        /// <summary>
        /// Parses and stores the lot information into the lot collection
        /// </summary>
        /// <param name="newData">The incoming lot information from the server.</param>
        public void ParseAndStoreLotInfoPayLoad(string newData)
        {
            string[] lotInfos = null;

            lotInfos = newData.Split('&');


            lock (LotLock)
            {
                Lots.Clear();
            }
            foreach (string lotInfo in lotInfos)
            {
                Lot lot = new Lot();
                Dictionary<string, string> lotPropertiesKVP = new Dictionary<string, string>();
                string[] lotProperties = lotInfo.Split('|');
                foreach(string lotProperty in lotProperties)
                {
                    string[] kvp = lotProperty.Split('=');
                    string key = kvp[0];
                    string value = kvp[1];
                    if(lotPropertiesKVP.ContainsKey(key) == true)
                    {
                        lotPropertiesKVP[key] = value;
                    }
                    else
                    {
                        lotPropertiesKVP.Add(key, value);
                    }
                }

                lot.LotID = Int32.Parse(lotPropertiesKVP[LotConstants.LotIDStr]);
                lot.WaferAmount = Int32.Parse(lotPropertiesKVP[LotConstants.WaferAmountStr]);
                lot.LotState = Int32.Parse(lotPropertiesKVP[LotConstants.LotStatusStr]);
                lot.AtLocationNumber = lotPropertiesKVP[LotConstants.AtLocationNumberStr];
                if(lot.AtLocationNumber == LotConstants.Undefined.ToString())
                {
                    lot.AtLocationNumber = "IN TRANSIT";
                }

                if(lot.LotState == ServerLotStates.ActiveState)
                {
                    lot.LotStateStr = "ACTIVE";
                }
                else if (lot.LotState == ServerLotStates.WaitingState)
                {
                    lot.LotStateStr = "WAITING";
                    lot.AtLocationNumber = "WAITING LINE";
                }
                else if (lot.LotState == ServerLotStates.InactiveState)
                {
                    lot.LotStateStr = "INACTIVE";
                }
                else if (lot.LotState == ServerLotStates.ProcessingState)
                {
                    lot.LotStateStr = "PROCESSING";
                }
                else if (lot.LotState == ServerLotStates.StandbyState)
                {
                    lot.LotStateStr = "STANDBY";
                }

                string[] waferIDsStr = lotPropertiesKVP[LotConstants.WaferIDsStr].Split(',');
                foreach(string waferIDStr in waferIDsStr)
                {
                    lot.WaferIDs.Add(Int32.Parse(waferIDStr));
                }
                lock (LotLock)
                {
                    Lots.Add(lot);
                }
            }
            return;
        }

        /// <summary>
        /// Client side validation for the user identifier.
        /// </summary>
        /// <param name="userIDStr">The user identifier.</param>
        /// <returns>string: message representation if the user identifier passed the client side check</returns>
        public string CheckUserID(string userIDStr)
        {
            int userID = 0;
            bool userIDNotNumeric = !Int32.TryParse(userIDStr, out userID);
            if (userIDNotNumeric == true)
            {
                return ServerResponses.UserIDNotNumeric;
            }
            this.UserID = userIDStr;
            return ServerResponses.UserIDNumeric;
        }

        /// <summary>
        /// Client side validation for the password
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns>string: message representation if the password passed the client side check</returns>
        public string CheckPassword(string password)
        {
            if (string.IsNullOrEmpty(password) == true)
            {
                return ServerResponses.PasswordEmpty;
            }
            this.Password = password;
            return ServerResponses.PasswordNonEmpty;
        }

        /// <summary>
        /// Client side check of the wafer amount, including a range check and a numeric check on the entered wafer amount.
        /// </summary>
        /// <param name="waferAmountStr">The wafer amount.</param>
        /// <returns>string: message representation if the entered wafer amount passed the client side check</returns>
        public string CheckWaferAmount(string waferAmountStr)
        {
            int waferAmount = 0;
            bool WaferAmountNotNumeric = !Int32.TryParse(waferAmountStr, out waferAmount);
            if (WaferAmountNotNumeric == true)
            {
                return ServerResponses.WaferAmtNotNumeric;
            }

            if(waferAmount > Waferconstants.MaxWaferAmount)
            {
                return ServerResponses.WaferAmtTooMuch;
            }

            if(waferAmount <= 0)
            {
                return ServerResponses.WaferTooLittle;
            }
            return ServerResponses.WaferAmtClientSideChecked;
        }

        /// <summary>
        /// Checks the response code sent from the server.
        /// </summary>
        /// <returns>string: message representation of the response code the server sent</returns>
        public string CheckResponseCode()
        {
            int responseCode = 0;
            if(this.PayloadData.ContainsKey(ServerPayloadElements.ResponseCode) == true)
            {
                responseCode = Int32.Parse(this.PayloadData[ServerPayloadElements.ResponseCode]);
            }

            if (responseCode == ServerResponseCodes.UserNotRecognized)
            {
                return ServerResponses.UserNotRecognized;
            }
            else if (responseCode == ServerResponseCodes.IncorrectPassword)
            {
                return ServerResponses.IncorrectPassword;
            }
            else if (responseCode == ServerResponseCodes.LogInSuccess)
            {
                this.User = new User(this.UserID, this.Password);
                return ServerResponses.LogInSuccess;
            }
            else if(responseCode == ServerResponseCodes.LogOUtSuccess)
            {
                this.ClientStop = true;
                this.UpdateDataThread.Join();
                this.UpdateDataThread = null;
                this.UserStream.Close();
                this.UserStream = null;
                this.TcpClient.Close();
                this.TcpClient = null;
                return ServerResponses.LogOutSuccess;
            }
            else if (responseCode == ServerResponseCodes.LogOutFail)
            {
                return ServerResponses.LogOutFailed;
            }
            else if(responseCode == ServerResponseCodes.AddLotSuccess)
            {
                return ServerResponses.AddLotSuccess;
            }
            else if (responseCode == ServerResponseCodes.WaferAmtIs0)
            {
                return ServerResponses.WaferAmountIs0;
            }
            else if (responseCode == ServerResponseCodes.WaferAmtIsLess)
            {
                return ServerResponses.WaferAmtIsLess;
            }
            else if (responseCode == ServerResponseCodes.InvalidLotID)
            {
                return ServerResponses.InvalidLotID;
            }
            else if (responseCode == ServerResponseCodes.LocationOccupied)
            {
                return ServerResponses.LocationOccupied;
            }
            else if (responseCode == ServerResponseCodes.LotStillInProcess)
            {
                return ServerResponses.LotStillProcessing;
            }
            else if (responseCode == ServerResponseCodes.MoveLotSuccess)
            {
                return ServerResponses.MoveLotSuccess;
            }
            else if (responseCode == ServerResponseCodes.LocationOccupied)
            {
                return ServerResponses.LocationOccupied;
            }
            else if (responseCode == ServerResponseCodes.LotEndedJourney)
            {
                return ServerResponses.LotEndedJourney;
            }
            else if (responseCode == ServerResponseCodes.LotInUse)
            {
                return ServerResponses.LotInUse;
            }

            return string.Empty;
        }
    }
}
