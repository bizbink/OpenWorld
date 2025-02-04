﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Open_World_Server
{
    public class PlayerUtils
    {
        public void SaveNewPlayerFile(string username, string password)
        {
            foreach (ServerClient savedClient in MainProgram._MainProgram.savedClients)
            {
                if (savedClient.username == username)
                {
                    if (!string.IsNullOrWhiteSpace(savedClient.homeTileID)) MainProgram._WorldUtils.RemoveSettlement(savedClient, savedClient.homeTileID);
                    savedClient.wealth = 0;
                    savedClient.pawnCount = 0;
                    SaveSystem.SaveUserData(savedClient);
                    return;
                }
            }

            ServerClient dummy = new ServerClient(null);
            dummy.username = username;
            dummy.password = password;
            dummy.homeTileID = null;

            MainProgram._MainProgram.savedClients.Add(dummy);
            SaveSystem.SaveUserData(dummy);
        }

        public void GiveSavedDataToPlayer(ServerClient client)
        {
            foreach (ServerClient savedClient in MainProgram._MainProgram.savedClients)
            {
                if (savedClient.username == client.username)
                {
                    client.homeTileID = savedClient.homeTileID;
                    client.giftString = savedClient.giftString;
                    client.tradeString = savedClient.tradeString;
                    client.pawnCount = savedClient.pawnCount;
                    client.wealth = savedClient.wealth;
                    client.isImmunized = savedClient.isImmunized;
                    return;
                }
            }
        }

        public void CheckSavedPlayers()
        {
            if (!Directory.Exists(MainProgram._MainProgram.playersFolderPath))
            {
                Directory.CreateDirectory(MainProgram._MainProgram.playersFolderPath);
                MainProgram._ServerUtils.LogToConsole("No Players Folder Found, Generating");
                return;
            }

            else
            {
                string[] playerFiles = Directory.GetFiles(MainProgram._MainProgram.playersFolderPath);

                foreach (string file in playerFiles)
                {
                    if (MainProgram._MainProgram.usingIdleTimer)
                    {
                        FileInfo fi = new FileInfo(file);
                        if (fi.LastAccessTime < DateTime.Now.AddDays(-MainProgram._MainProgram.idleTimer))
                        {
                            fi.Delete();
                            continue;
                        }
                    }

                    MainDataHolder data = SaveSystem.LoadUserData(Path.GetFileNameWithoutExtension(file));
                    {
                        ServerClient dummy = data.serverclient;
                        MainProgram._MainProgram.savedClients.Add(dummy);
                        if (!string.IsNullOrWhiteSpace(dummy.homeTileID))
                        {
                            try { MainProgram._MainProgram.savedSettlements.Add(dummy.homeTileID, new List<string>() { dummy.username }); }
                            catch 
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                MainProgram._ServerUtils.LogToConsole("Error! Player " + dummy.username + " Is Using A Cloned Entry! Skipping Entry");
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                        }
                    }
                }

                if (MainProgram._MainProgram.savedClients.Count == 0) MainProgram._ServerUtils.LogToConsole("No Saved Players Found, Ignoring");
                else MainProgram._ServerUtils.LogToConsole("Loaded [" + MainProgram._MainProgram.savedClients.Count + "] Player Files");
            }
        }

        public void CheckForPlayerWealth(ServerClient client)
        {
            if (MainProgram._MainProgram.usingWealthSystem == false) return;
            if (MainProgram._MainProgram.banWealthThreshold == 0 && MainProgram._MainProgram.warningWealthThreshold == 0) return;
            if (client.isAdmin) return;

            int wealthToCompare = (int) MainProgram._MainProgram.savedClients.Find(fetch => fetch.username == client.username).wealth;

            if (client.wealth - wealthToCompare > MainProgram._MainProgram.banWealthThreshold && MainProgram._MainProgram.banWealthThreshold > 0)
            {
                SaveSystem.SaveUserData(client);
                MainProgram._MainProgram.savedClients.Find(fetch => fetch.username == client.username).wealth = client.wealth;
                MainProgram._MainProgram.savedClients.Find(fetch => fetch.username == client.username).pawnCount = client.pawnCount;

                MainProgram._MainProgram.bannedIPs.Add(((IPEndPoint)client.tcp.Client.RemoteEndPoint).Address.ToString(), client.username);
                client.disconnectFlag = true;
                SaveSystem.SaveBannedIPs(MainProgram._MainProgram.bannedIPs);

                Console.ForegroundColor = ConsoleColor.Red;
                MainProgram._ServerUtils.LogToConsole("Player [" + client.username + "]'s Wealth Triggered Alarm [" + wealthToCompare + " > " + (int)client.wealth + "], Banning");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if (client.wealth - wealthToCompare > MainProgram._MainProgram.warningWealthThreshold && MainProgram._MainProgram.warningWealthThreshold > 0)
            {
                SaveSystem.SaveUserData(client);
                MainProgram._MainProgram.savedClients.Find(fetch => fetch.username == client.username).wealth = client.wealth;
                MainProgram._MainProgram.savedClients.Find(fetch => fetch.username == client.username).pawnCount = client.pawnCount;

                Console.ForegroundColor = ConsoleColor.Yellow;
                MainProgram._ServerUtils.LogToConsole("Player [" + client.username + "]'s Wealth Triggered Warning [" + wealthToCompare + " > " + (int) client.wealth + "]");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                SaveSystem.SaveUserData(client);
                MainProgram._MainProgram.savedClients.Find(fetch => fetch.username == client.username).wealth = client.wealth;
                MainProgram._MainProgram.savedClients.Find(fetch => fetch.username == client.username).pawnCount = client.pawnCount;
            }
        }

        public bool CheckForConnectedPlayers(string tileID)
        {
            foreach (ServerClient client in MainProgram._Networking.connectedClients)
            {
                if (client.homeTileID == tileID) return true;
            }

            return false;
        }

        public bool CheckForPlayerShield(string tileID)
        {
            foreach (ServerClient client in MainProgram._Networking.connectedClients)
            {
                if (client.homeTileID == tileID && !client.eventShielded && !client.isImmunized)
                {
                    client.eventShielded = true;
                    return true;
                }
            }

            return false;
        }

        public bool CheckForPvpAvailability(string tileID)
        {
            foreach (ServerClient client in MainProgram._Networking.connectedClients)
            {
                if (client.homeTileID == tileID && !client.inRTSE && !client.isImmunized)
                {
                    client.inRTSE = true;
                    return true;
                }
            }

            return false;
        }

        public string GetSpyData(string tileID, ServerClient origin)
        {
            foreach (ServerClient client in MainProgram._Networking.connectedClients)
            {
                if (client.homeTileID == tileID)
                {
                    string dataToReturn = client.pawnCount.ToString() + "»" + client.wealth.ToString() + "»" + client.eventShielded + "»" + client.inRTSE;

                    if (client.giftString.Count > 0) dataToReturn += "»" + "True";
                    else dataToReturn += "»" + "False";

                    if (client.tradeString.Count > 0) dataToReturn += "»" + "True";
                    else dataToReturn += "»" + "False";

                    Random rnd = new Random();
                    int chance = rnd.Next(0, 2);
                    if (chance == 1) MainProgram._Networking.SendData(client, "Spy│" + origin.username);

                    MainProgram._ServerUtils.LogToConsole("Spy Done Between [" + origin.username + "] And [" + client.username + "]");

                    return dataToReturn;
                }
            }

            return "";
        }

        public void SendEventToPlayer(ServerClient invoker, string data)
        {
            string dataToSend = "ForcedEvent│" + data.Split('│')[1];

            foreach (ServerClient sc in MainProgram._Networking.connectedClients)
            {
                if (sc.homeTileID == data.Split('│')[2])
                {
                    MainProgram._ServerUtils.LogToConsole("Player [" + invoker.username + "] Has Sent Forced Event [" + data.Split('│')[1] + "] To [" + sc.username + "]");
                    MainProgram._Networking.SendData(sc, dataToSend);
                    break;
                }
            }
        }

        public void SendGiftToPlayer(ServerClient invoker, string data)
        {
            string tileToSend = data.Split('│')[1];
            string dataToSend = "GiftedItems│" + data.Split('│')[2];

            try
            {
                string sendMode = data.Split('│')[3];

                if (!string.IsNullOrWhiteSpace(sendMode) && sendMode == "Pod")
                {
                    foreach (ServerClient sc in MainProgram._Networking.connectedClients)
                    {
                        if (sc == invoker) continue;
                        if (sc.homeTileID == tileToSend) continue;

                        MainProgram._Networking.SendData(sc, "│RenderTransportPod│" + invoker.homeTileID + "│" + tileToSend + "│");
                    }
                }
            }
            catch { }

            foreach (ServerClient sc in MainProgram._Networking.connectedClients)
            {
                if (sc.homeTileID == tileToSend)
                {
                    MainProgram._Networking.SendData(sc, dataToSend);
                    MainProgram._ServerUtils.LogToConsole("Gift Done Between [" + invoker.username + "] And [" + sc.username + "]");
                    return;
                }
            }

            dataToSend = dataToSend.Replace("GiftedItems│", "");

            foreach(ServerClient sc in MainProgram._MainProgram.savedClients)
            {
                if (sc.homeTileID == tileToSend)
                {
                    sc.giftString.Add(dataToSend);
                    SaveSystem.SaveUserData(sc);
                    MainProgram._ServerUtils.LogToConsole("Gift Done Between [" + invoker.username + "] And [" + sc.username + "] But Was Offline. Saving");
                    return;
                }
            }
        }

        public void SendTradeRequestToPlayer(ServerClient invoker, string data)
        {
            string dataToSend = "TradeRequest│" + invoker.username + "│" + data.Split('│')[2] + "│" + data.Split('│')[3];

            foreach (ServerClient sc in MainProgram._Networking.connectedClients)
            {
                if (sc.homeTileID == data.Split('│')[1])
                {
                    MainProgram._Networking.SendData(sc, dataToSend);
                    return;
                }
            }
        }

        public void SendBarterRequestToPlayer(ServerClient invoker, string data)
        {
            string dataToSend = "BarterRequest│" + invoker.homeTileID + "│" + data.Split('│')[2];

            foreach (ServerClient sc in MainProgram._Networking.connectedClients)
            {
                if (sc.homeTileID == data.Split('│')[1])
                {
                    MainProgram._Networking.SendData(sc, dataToSend);
                    return;
                }
            }
        }
    }
}
