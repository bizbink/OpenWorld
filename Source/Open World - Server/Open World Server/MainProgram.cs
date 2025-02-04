﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.IO;
using System.Threading;

namespace Open_World_Server
{
    [System.Serializable]
    public class MainProgram
    {
        //Instances
        public static MainProgram _MainProgram = new MainProgram();
        public static Threading _Threading = new Threading();
        public static Networking _Networking = new Networking();
        public static Encryption _Encryption = new Encryption();
        public static ServerUtils _ServerUtils = new ServerUtils();
        public static PlayerUtils _PlayerUtils = new PlayerUtils();
        public static WorldUtils _WorldUtils = new WorldUtils();

        //Paths
        public string mainFolderPath;
        public string serverSettingsPath;
        public string worldSettingsPath;
        public string playersFolderPath;
        public string modsFolderPath;
        public string whitelistedModsFolderPath;
        public string whitelistedUsersPath;
        public string logFolderPath;

        //Player Parameters
        public List<ServerClient> savedClients = new List<ServerClient>();
        public Dictionary<string, List<string>> savedSettlements = new Dictionary<string, List<string>>();

        //Server Parameters
        public string serverName = "";
        public string serverDescription = "";
        public string serverVersion = "v1.4.0";
        public int maxPlayers = 300;
        public int warningWealthThreshold = 10000;
        public int banWealthThreshold = 100000;
        public int idleTimer = 7;
        public bool usingIdleTimer = false;
        public bool allowDevMode = false;
        public bool usingWhitelist = false;
        public bool usingWealthSystem = false;
        public bool usingRoadSystem = false;
        public bool aggressiveRoadMode = false;
        public bool forceModlist = false;
        public bool forceModlistConfigs = false;
        public bool usingModVerification = false;
        public bool usingChat = false;
        public bool usingProfanityFilter = false;
        public List<string> whitelistedUsernames = new List<string>();
        public List<string> adminList = new List<string>();
        public List<string> modList = new List<string>();
        public List<string> whitelistedMods = new List<string>();
        public List<string> chatCache = new List<string>();
        public Dictionary<string, string> bannedIPs = new Dictionary<string, string>();

        //World Parameters
        public float globeCoverage;
        public string seed;
        public int overallRainfall;
        public int overallTemperature;
        public int overallPopulation;

        static void Main()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
            CultureInfo.CurrentUICulture = new CultureInfo("en-US", false);
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US", false);
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US", false);

            _MainProgram.mainFolderPath = AppDomain.CurrentDomain.BaseDirectory;
            _MainProgram.logFolderPath = _MainProgram.mainFolderPath + Path.DirectorySeparatorChar + "Logs";

            Console.ForegroundColor = ConsoleColor.Green;
            _ServerUtils.LogToConsole("Server Startup:");
            _ServerUtils.LogToConsole("Using Culture Info: [" + CultureInfo.CurrentCulture + "]");

            _ServerUtils.SetupPaths();
            _ServerUtils.CheckForFiles();

            _Threading.GenerateThreads(0);
            _MainProgram.ListenForCommands();
        }

        private void ListenForCommands()
        {
            Console.ForegroundColor = ConsoleColor.White;

            string command = Console.ReadLine();

            if (command == "Help" || command == "help")
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | List Of Available Commands:", DateTime.Now);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("[{0}] | Help - Displays Help Menu", DateTime.Now);
                Console.WriteLine("[{0}] | Settings - Displays Settings Menu", DateTime.Now);
                Console.WriteLine("[{0}] | Reload - Reloads All Available Settings Into The Server", DateTime.Now);
                Console.WriteLine("[{0}] | Status - Shows A General Overview Menu", DateTime.Now);
                Console.WriteLine("[{0}] | Settlements - Displays Settlements Menu", DateTime.Now);
                Console.WriteLine("[{0}] | List - Displays Player List Menu", DateTime.Now);
                Console.WriteLine("[{0}] | Whitelist - Shows All Whitelisted Players", DateTime.Now);
                Console.WriteLine("[{0}] | Clear - Clears The Console", DateTime.Now);
                Console.WriteLine("[{0}] | Exit - Closes The Server", DateTime.Now);
                Console.WriteLine(Environment.NewLine);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Communication:", DateTime.Now);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("[{0}] | Say - Send A Chat Message", DateTime.Now);
                Console.WriteLine("[{0}] | Broadcast - Send A Letter To Every Player Connected", DateTime.Now);
                Console.WriteLine("[{0}] | Notify - Send A Letter To X Player", DateTime.Now);
                Console.WriteLine("[{0}] | Chat - Displays Chat Menu", DateTime.Now);
                Console.WriteLine(Environment.NewLine);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Interaction:", DateTime.Now);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("[{0}] | Invoke - Invokes An Event To X Player", DateTime.Now);
                Console.WriteLine("[{0}] | Plague - Invokes An Event To All Connected Players", DateTime.Now);
                Console.WriteLine("[{0}] | Eventlist - Shows All Available Events", DateTime.Now);
                Console.WriteLine("[{0}] | GiveItem - Gives An Item To X Player", DateTime.Now);
                Console.WriteLine("[{0}] | GiveItemAll - Gives An Item To All Players", DateTime.Now);
                Console.WriteLine("[{0}] | Protect - Protects A Player From Any Event Temporarily", DateTime.Now);
                Console.WriteLine("[{0}] | Deprotect - Disables All Protections Given To X Player", DateTime.Now);
                Console.WriteLine("[{0}] | Immunize - Protects A Player From Any Event Permanently", DateTime.Now);
                Console.WriteLine("[{0}] | Deimmunize - Disables The Immunity Given To X Player", DateTime.Now);
                Console.WriteLine(Environment.NewLine);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Admin Control:", DateTime.Now);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("[{0}] | Investigate - Displays All Data About X Player", DateTime.Now);
                Console.WriteLine("[{0}] | Promote - Promotes X Player To Admin", DateTime.Now);
                Console.WriteLine("[{0}] | Demote - Demotes X Player", DateTime.Now);
                Console.WriteLine("[{0}] | Adminlist - Shows All Server Admins", DateTime.Now);
                Console.WriteLine("[{0}] | Kick - Kicks X Player", DateTime.Now);
                Console.WriteLine("[{0}] | Ban - Bans X Player", DateTime.Now);
                Console.WriteLine("[{0}] | Pardon - Pardons X Player", DateTime.Now);
                Console.WriteLine("[{0}] | Banlist - Shows All Banned Players", DateTime.Now);
                Console.WriteLine("[{0}] | Wipe - Deletes Every Player Data In The Server", DateTime.Now);
                Console.WriteLine(Environment.NewLine);
            }

            else if (command.StartsWith("Say ") || command.StartsWith("say "))
            {
                string message = "";
                try { message = command.Remove(0, 4); }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] | Missing Parameters", DateTime.Now);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Environment.NewLine);
                    ListenForCommands();
                }

                string messageForConsole = "Chat - [Console] " + message;

                _ServerUtils.LogToConsole(messageForConsole);

                _MainProgram.chatCache.Add("[" + DateTime.Now + "]" + " │ " + messageForConsole);

                try
                {
                    foreach (ServerClient sc in _Networking.connectedClients)
                    {
                        _Networking.SendData(sc, "ChatMessage│SERVER│" + message);
                    }
                }
                catch { }
            }

            else if (command.StartsWith("Broadcast ") || command.StartsWith("broadcast "))
            {
                Console.Clear();

                string text = "";

                try 
                {
                    command = command.Remove(0, 10);
                    text = command;

                    if (string.IsNullOrWhiteSpace(text))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("[{0}] | Missing Parameters", DateTime.Now);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(Environment.NewLine);
                        ListenForCommands();
                    }
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] | Missing Parameters", DateTime.Now);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Environment.NewLine);
                    ListenForCommands();
                }

                foreach (ServerClient sc in _Networking.connectedClients)
                {
                    _Networking.SendData(sc, "Notification│" + text);
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Letter Sent To Every Connected Player", DateTime.Now);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(Environment.NewLine);
                ListenForCommands();
            }

            else if (command.StartsWith("Notify ") || command.StartsWith("notify "))
            {
                Console.Clear();

                string target = "";
                string text = "";

                try
                {
                    command = command.Remove(0, 7);
                    target = command.Split(' ')[0];
                    text = command.Replace(target + " ", "");

                    if (string.IsNullOrWhiteSpace(text))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("[{0}] | Missing Parameters", DateTime.Now);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(Environment.NewLine);
                        ListenForCommands();
                    }
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] | Missing Parameters", DateTime.Now);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Environment.NewLine);
                    ListenForCommands();
                }

                ServerClient targetClient = _Networking.connectedClients.Find(fetch => fetch.username == target);

                if (targetClient == null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] | Player [{1}] not found", DateTime.Now, target);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Environment.NewLine);
                    ListenForCommands();
                }
                else
                {
                    _Networking.SendData(targetClient, "Notification│" + text);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] | Sent Letter To [{1}]", DateTime.Now, targetClient.username);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Environment.NewLine);
                    ListenForCommands();
                }
            }

            else if (command == "Settings" || command == "settings")
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Server Settings:", DateTime.Now);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("[{0}] | Server Name: {1}", DateTime.Now, serverName);
                Console.WriteLine("[{0}] | Server Description: {1}", DateTime.Now, serverDescription);
                Console.WriteLine("[{0}] | Server Local IP: {1}", DateTime.Now, _Networking.localAddress);
                Console.WriteLine("[{0}] | Server Port: {1}", DateTime.Now, _Networking.serverPort);

                Console.WriteLine(Environment.NewLine);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | World Settings:", DateTime.Now);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("[{0}] | Globe Coverage: {1}", DateTime.Now, globeCoverage);
                Console.WriteLine("[{0}] | Seed: {1}", DateTime.Now, seed);
                Console.WriteLine("[{0}] | Overall Rainfall: {1}", DateTime.Now, overallRainfall);
                Console.WriteLine("[{0}] | Overall Temperature: {1}", DateTime.Now, overallTemperature);
                Console.WriteLine("[{0}] | Overall Population: {1}", DateTime.Now, overallPopulation);

                Console.WriteLine(Environment.NewLine);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Server Mods: [{1}]", DateTime.Now, modList.Count);
                Console.ForegroundColor = ConsoleColor.White;

                if (modList.Count() == 0) Console.WriteLine("[{0}] | No Mods Found", DateTime.Now);
                else foreach (string mod in modList) Console.WriteLine("[{0}] | {1}", DateTime.Now, mod);

                Console.WriteLine(Environment.NewLine);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Server Whitelisted Mods: [{1}]", DateTime.Now, whitelistedMods.Count);
                Console.ForegroundColor = ConsoleColor.White;

                if (whitelistedMods.Count == 0) Console.WriteLine("[{0}] | No Whitelisted Mods Found", DateTime.Now);
                else foreach (string whitelistedMod in whitelistedMods) Console.WriteLine("[{0}] | {1}", DateTime.Now, whitelistedMod);

                Console.WriteLine(Environment.NewLine);
            }

            else if (command == "Reload" || command == "reload")
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Reloading All Current Mods", DateTime.Now);
                Console.ForegroundColor = ConsoleColor.White;
                _ServerUtils.CheckMods();
                _ServerUtils.CheckWhitelistedMods();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Mods Have Been Reloaded", DateTime.Now);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(Environment.NewLine);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Reloading All Whitelisted Players", DateTime.Now);
                Console.ForegroundColor = ConsoleColor.White;
                _ServerUtils.CheckForWhitelistedPlayers();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Whitelisted Players Have Been Reloaded", DateTime.Now);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(Environment.NewLine);
            }

            else if (command == "Status" || command == "status")
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Server Status", DateTime.Now);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("[{0}] | Version: {1}", DateTime.Now, MainProgram._MainProgram.serverVersion);
                Console.WriteLine("[{0}] | Connection: Online", DateTime.Now);
                Console.WriteLine("[{0}] | Uptime: [{1}]", DateTime.Now, DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime());
                Console.WriteLine(Environment.NewLine);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Mods:", DateTime.Now);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("[{0}] | Mods: [{1}]", DateTime.Now, _MainProgram.modList.Count());
                Console.WriteLine("[{0}] | Whitelisted Mods: [{1}]", DateTime.Now, _MainProgram.whitelistedMods.Count());
                Console.WriteLine(Environment.NewLine);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Players:", DateTime.Now);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("[{0}] | Connected Players: [{1}]", DateTime.Now, _Networking.connectedClients.Count());
                Console.WriteLine("[{0}] | Saved Players: [{1}]", DateTime.Now, _MainProgram.savedClients.Count());
                Console.WriteLine("[{0}] | Saved Settlements: [{1}]", DateTime.Now, _MainProgram.savedSettlements.Count());
                Console.WriteLine("[{0}] | Whitelisted Players: [{1}]", DateTime.Now, _MainProgram.whitelistedUsernames.Count());
                Console.WriteLine("[{0}] | Max Players: [{1}]", DateTime.Now, _MainProgram.maxPlayers);
                Console.WriteLine(Environment.NewLine);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Modlist Settings:", DateTime.Now);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("[{0}] | Using Modlist Check: [{1}]", DateTime.Now, _MainProgram.forceModlist);
                Console.WriteLine("[{0}] | Using Modlist Config Check: [{1}]", DateTime.Now, _MainProgram.forceModlistConfigs);
                Console.WriteLine("[{0}] | Using Mod Verification: [{1}]", DateTime.Now, _MainProgram.usingModVerification);
                Console.WriteLine(Environment.NewLine);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Chat Settings:", DateTime.Now);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("[{0}] | Using Chat: [{1}]", DateTime.Now, _MainProgram.usingChat);
                Console.WriteLine("[{0}] | Using Profanity Filter: [{1}]", DateTime.Now, _MainProgram.usingProfanityFilter);
                Console.WriteLine(Environment.NewLine);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Wealth Settings:", DateTime.Now);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("[{0}] | Using Wealth System: [{1}]", DateTime.Now, _MainProgram.usingWealthSystem);
                Console.WriteLine("[{0}] | Warning Threshold: [{1}]", DateTime.Now, _MainProgram.warningWealthThreshold);
                Console.WriteLine("[{0}] | Ban Threshold: [{1}]", DateTime.Now, _MainProgram.banWealthThreshold);
                Console.WriteLine(Environment.NewLine);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Idle Settings:", DateTime.Now);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("[{0}] | Using Idle System: [{1}]", DateTime.Now, _MainProgram.usingIdleTimer);
                Console.WriteLine("[{0}] | Idle Threshold: [{1}]", DateTime.Now, _MainProgram.idleTimer);
                Console.WriteLine(Environment.NewLine);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Road Settings:", DateTime.Now);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("[{0}] | Using Road System: [{1}]", DateTime.Now, _MainProgram.usingRoadSystem);
                Console.WriteLine("[{0}] | Aggressive Road Mode: [{1}]", DateTime.Now, _MainProgram.aggressiveRoadMode);
                Console.WriteLine(Environment.NewLine);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Miscellaneous Settings", DateTime.Now);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("[{0}] | Using Whitelist: [{1}]", DateTime.Now, _MainProgram.usingWhitelist);
                Console.WriteLine("[{0}] | Allow Dev Mode: [{1}]", DateTime.Now, _MainProgram.allowDevMode);
                Console.WriteLine(Environment.NewLine);
            }

            else if (command.StartsWith("Invoke ") || command.StartsWith("invoke "))
            {
                Console.Clear();

                string clientID = "";
                string eventID = "";
                ServerClient target = null;

                try
                {
                    clientID = command.Split(' ')[1];
                    eventID = command.Split(' ')[2];
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] | Missing Parameters", DateTime.Now);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Environment.NewLine);
                    ListenForCommands();
                }

                foreach(ServerClient client in _Networking.connectedClients)
                {
                    if (client.username == clientID)
                    {
                        target = client;
                        break;
                    }
                }

                if (target == null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] | Player [{1}] Not Found", DateTime.Now, clientID);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Environment.NewLine);
                    ListenForCommands();
                }

                _Networking.SendData(target, "ForcedEvent│" + eventID);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Sent Event [{1}] to [{2}]", DateTime.Now, eventID, clientID);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(Environment.NewLine);
            }

            else if (command.StartsWith("Plague ") || command.StartsWith("plague "))
            {
                Console.Clear();

                string eventID = "";

                try { eventID = command.Split(' ')[1]; }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] | Missing Parameters", DateTime.Now);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Environment.NewLine);
                    ListenForCommands();
                }

                foreach (ServerClient client in _Networking.connectedClients)
                {
                    _Networking.SendData(client, "ForcedEvent│" + eventID);
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Sent Event [{1}] To Every Player", DateTime.Now, eventID);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(Environment.NewLine);
            }

            else if (command == "Eventlist" || command == "eventlist")
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | List Of Available Events:", DateTime.Now);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("[{0}] | Raid", DateTime.Now);
                Console.WriteLine("[{0}] | Infestation", DateTime.Now);
                Console.WriteLine("[{0}] | MechCluster", DateTime.Now);
                Console.WriteLine("[{0}] | ToxicFallout", DateTime.Now);
                Console.WriteLine("[{0}] | Manhunter", DateTime.Now);
                Console.WriteLine("[{0}] | Wanderer", DateTime.Now);
                Console.WriteLine("[{0}] | FarmAnimals", DateTime.Now);
                Console.WriteLine("[{0}] | ShipChunk", DateTime.Now);
                Console.WriteLine("[{0}] | GiveQuest", DateTime.Now);
                Console.WriteLine("[{0}] | TraderCaravan", DateTime.Now);

                Console.WriteLine(Environment.NewLine);
            }

            else if (command == "Chat" || command == "chat")
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Server Chat:", DateTime.Now);
                Console.ForegroundColor = ConsoleColor.White;

                if (chatCache.Count == 0) Console.WriteLine("[{0}] | No Chat Messages", DateTime.Now);
                else foreach (string message in chatCache)
                {
                    Console.WriteLine(message);
                }

                Console.WriteLine(Environment.NewLine);
            }

            else if (command == "List" || command == "list")
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Connected Players: [{1}]", DateTime.Now, _Networking.connectedClients.Count);
                Console.ForegroundColor = ConsoleColor.White;

                if (_Networking.connectedClients.Count() == 0) Console.WriteLine("[{0}] | No Players Connected", DateTime.Now);
                else foreach (ServerClient client in _Networking.connectedClients)
                {
                    try { Console.WriteLine("[{0}] | " + client.username, DateTime.Now); }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[{0}] | Error Processing Player With IP [{1}]", DateTime.Now, ((IPEndPoint)client.tcp.Client.RemoteEndPoint).Address.ToString());
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }

                Console.WriteLine(Environment.NewLine);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Saved Players: [{1}]", DateTime.Now, _MainProgram.savedClients.Count);
                Console.ForegroundColor = ConsoleColor.White;

                if (_MainProgram.savedClients.Count() == 0) Console.WriteLine("[{0}] | No Players Saved", DateTime.Now);
                else foreach (ServerClient savedClient in _MainProgram.savedClients)
                {
                    try { Console.WriteLine("[{0}] | " + savedClient.username, DateTime.Now); }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[{0}] | Error Processing Player With IP [{1}]", DateTime.Now, ((IPEndPoint)savedClient.tcp.Client.RemoteEndPoint).Address.ToString());
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }

                Console.WriteLine(Environment.NewLine);
            }

            else if (command.StartsWith("Investigate ") || command.StartsWith("investigate "))
            {
                Console.Clear();

                string clientID = "";
                try { clientID = command.Split(' ')[1]; }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] | Missing Parameters", DateTime.Now);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Environment.NewLine);
                    ListenForCommands();
                }

                foreach (ServerClient client in savedClients)
                {
                    if (client.username == clientID)
                    {
                        ServerClient clientToInvestigate = null;

                        bool isConnected = false;
                        string ip = "None";

                        if (_Networking.connectedClients.Find(fetch => fetch.username == client.username) != null)
                        {
                            clientToInvestigate = _Networking.connectedClients.Find(fetch => fetch.username == client.username);
                            isConnected = true;
                            ip = ((IPEndPoint)clientToInvestigate.tcp.Client.RemoteEndPoint).Address.ToString();
                        }

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("[{0}] | Player Details: ", DateTime.Now);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("[{0}] | Username: [{1}]", DateTime.Now, client.username);
                        Console.WriteLine("[{0}] | Password: [{1}]", DateTime.Now, client.password);
                        Console.WriteLine("[{0}] | Admin: [{1}]", DateTime.Now, client.isAdmin);
                        Console.WriteLine("[{0}] | Online: [{1}]", DateTime.Now, isConnected);
                        Console.WriteLine("[{0}] | Connection IP: [{1}]", DateTime.Now, ip);
                        Console.WriteLine("[{0}] | Home Tile ID: [{1}]", DateTime.Now, client.homeTileID);
                        Console.WriteLine("[{0}] | Stored Gifts: [{1}]", DateTime.Now, client.giftString.Count());
                        Console.WriteLine("[{0}] | Stored Trades: [{1}]", DateTime.Now, client.tradeString.Count());
                        Console.WriteLine("[{0}] | Wealth Value: [{1}]", DateTime.Now, client.wealth);
                        Console.WriteLine("[{0}] | Pawn Count: [{1}]", DateTime.Now, client.pawnCount);
                        Console.WriteLine("[{0}] | Immunized: [{1}]", DateTime.Now, client.isImmunized);
                        Console.WriteLine("[{0}] | Event Shielded: [{1}]", DateTime.Now, client.eventShielded);
                        Console.WriteLine("[{0}] | In RTSE: [{1}]", DateTime.Now, client.inRTSE);
                        Console.WriteLine(Environment.NewLine);
                        ListenForCommands();
                    }
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Player [{1}] Not Found", DateTime.Now, clientID);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(Environment.NewLine);
            }

            else if (command == "Settlements" || command == "settlements")
            {
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Server Settlements: [{1}]", DateTime.Now, savedSettlements.Count);
                Console.ForegroundColor = ConsoleColor.White;

                if (savedSettlements.Count == 0) Console.WriteLine("[{0}] | No Active Settlements", DateTime.Now);
                else foreach (KeyValuePair<string, List<string>> pair in savedSettlements)
                {
                    Console.WriteLine("[{0}] | {1} - {2} ", DateTime.Now, pair.Key, pair.Value[0]);
                }

                Console.WriteLine(Environment.NewLine);
            }

            else if (command == "Banlist" || command == "banlist")
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Banned players: [{1}]", DateTime.Now, bannedIPs.Count());
                Console.ForegroundColor = ConsoleColor.White;

                if (bannedIPs.Count == 0) Console.WriteLine("[{0}] | No Banned Players", DateTime.Now);
                else foreach (KeyValuePair<string, string> pair in bannedIPs)
                {
                    Console.WriteLine("[{0}] | [{1}] - [{2}]", DateTime.Now, pair.Value, pair.Key);
                }

                Console.WriteLine(Environment.NewLine);
            }

            else if (command.StartsWith("Kick ") || command.StartsWith("kick "))
            {
                Console.Clear();

                string clientID = "";
                try { clientID = command.Split(' ')[1]; }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] | Missing Parameters", DateTime.Now);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Environment.NewLine);
                    ListenForCommands();
                }

                foreach (ServerClient client in _Networking.connectedClients)
                {
                    if (client.username == clientID)
                    {
                        client.disconnectFlag = true;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("[{0}] | Player [{1}] Has Been Kicked", DateTime.Now, clientID);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(Environment.NewLine);
                        ListenForCommands();
                    }
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Player [{1}] Not Found", DateTime.Now, clientID);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(Environment.NewLine);
            }

            else if (command.StartsWith("Ban ") || command.StartsWith("ban "))
            {
                Console.Clear();

                string clientID = "";
                try { clientID = command.Split(' ')[1]; }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] | Missing Parameters", DateTime.Now);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Environment.NewLine);
                    ListenForCommands();
                }

                foreach (ServerClient client in _Networking.connectedClients)
                {
                    if (client.username == clientID)
                    {
                        bannedIPs.Add(((IPEndPoint)client.tcp.Client.RemoteEndPoint).Address.ToString(), client.username);
                        client.disconnectFlag = true;
                        SaveSystem.SaveBannedIPs(bannedIPs);
                        Console.ForegroundColor = ConsoleColor.Green;
                        _ServerUtils.LogToConsole("Player [" + client.username + "] Has Been Banned");
                        Console.ForegroundColor = ConsoleColor.White;
                        _ServerUtils.LogToConsole(Environment.NewLine);
                        ListenForCommands();
                    }
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Player [{1}] Not Found", DateTime.Now, clientID);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(Environment.NewLine);
            }

            else if (command.StartsWith("Pardon ") || command.StartsWith("pardon "))
            {
                Console.Clear();

                string clientUsername = "";
                try { clientUsername = command.Split(' ')[1]; }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] | Missing Parameters", DateTime.Now);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Environment.NewLine);
                    ListenForCommands();
                }

                foreach (KeyValuePair<string, string> pair in bannedIPs)
                {
                    if (pair.Value == clientUsername)
                    {
                        bannedIPs.Remove(pair.Key);
                        SaveSystem.SaveBannedIPs(bannedIPs);
                        Console.ForegroundColor = ConsoleColor.Green;
                        _ServerUtils.LogToConsole("Player [" + clientUsername + "] Has Been Unbanned");
                        Console.ForegroundColor = ConsoleColor.White;
                        _ServerUtils.LogToConsole(Environment.NewLine);
                        ListenForCommands();
                    }
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Player [{1}] Not Found", DateTime.Now, clientUsername);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(Environment.NewLine);
            }

            else if (command.StartsWith("Promote ") || command.StartsWith("promote "))
            {
                Console.Clear();

                string clientID = "";
                try { clientID = command.Split(' ')[1]; }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] | Missing Parameters", DateTime.Now);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Environment.NewLine);
                    ListenForCommands();
                }

                foreach (ServerClient client in _Networking.connectedClients)
                {
                    if (client.username == clientID)
                    {
                        if (client.isAdmin == true)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            _ServerUtils.LogToConsole("Player [" + client.username + "] Was Already An Administrator");
                            Console.ForegroundColor = ConsoleColor.White;
                            _ServerUtils.LogToConsole(Environment.NewLine);
                        }

                        else
                        {
                            client.isAdmin = true;
                            _MainProgram.savedClients.Find(fetch => fetch.username == client.username).isAdmin = true;
                            SaveSystem.SaveUserData(client);

                            _Networking.SendData(client, "│Promote│");

                            Console.ForegroundColor = ConsoleColor.Green;
                            _ServerUtils.LogToConsole("Player [" + client.username + "] Has Been Promoted");
                            Console.ForegroundColor = ConsoleColor.White;
                            _ServerUtils.LogToConsole(Environment.NewLine);
                        }

                        ListenForCommands();
                    }
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Player [{1}] Not Found", DateTime.Now, clientID);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(Environment.NewLine);
            }

            else if (command.StartsWith("Demote ") || command.StartsWith("demote "))
            {
                Console.Clear();

                string clientID = "";
                try { clientID = command.Split(' ')[1]; }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] | Missing Parameters", DateTime.Now);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Environment.NewLine);
                    ListenForCommands();
                }

                foreach (ServerClient client in _Networking.connectedClients)
                {
                    if (client.username == clientID)
                    {
                        if (!client.isAdmin)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            _ServerUtils.LogToConsole("Player [" + client.username + "] Is Not An Administrator");
                            Console.ForegroundColor = ConsoleColor.White;
                            _ServerUtils.LogToConsole(Environment.NewLine);
                        }

                        else
                        {
                            client.isAdmin = false;
                            _MainProgram.savedClients.Find(fetch => fetch.username == client.username).isAdmin = false;
                            SaveSystem.SaveUserData(client);

                            _Networking.SendData(client, "│Demote│");

                            Console.ForegroundColor = ConsoleColor.Green;
                            _ServerUtils.LogToConsole("Player [" + client.username + "] Has Been Demoted");
                            Console.ForegroundColor = ConsoleColor.White;
                            _ServerUtils.LogToConsole(Environment.NewLine);
                        }

                        ListenForCommands();
                    }
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Player [{1}] Not Found", DateTime.Now, clientID);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(Environment.NewLine);
            }

            else if (command.StartsWith("Giveitem ") || command.StartsWith("giveitem "))
            {
                Console.Clear();

                string clientID = "";
                try { clientID = command.Split(' ')[1]; }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] | Missing Parameters", DateTime.Now);
                    Console.WriteLine("[{0}] | Usage: Giveitem [username] [itemID] [itemQuantity] [itemQuality]", DateTime.Now);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Environment.NewLine);
                    ListenForCommands();
                }

                string itemID = "";
                try { itemID = command.Split(' ')[2]; }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] | Missing Parameters", DateTime.Now);
                    Console.WriteLine("[{0}] | Usage: GiveItem [username] [itemID] [itemQuantity] [itemQuality]", DateTime.Now);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Environment.NewLine);
                    ListenForCommands();
                }

                string itemQuantity = "";
                try { itemQuantity = command.Split(' ')[3]; }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] | Missing Parameters", DateTime.Now);
                    Console.WriteLine("[{0}] | Usage: GiveItem [username] [itemID] [itemQuantity] [itemQuality]", DateTime.Now);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Environment.NewLine);
                    ListenForCommands();
                }

                string itemQuality = "";
                try { itemQuality = command.Split(' ')[4]; }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] | Missing Parameters", DateTime.Now);
                    Console.WriteLine("[{0}] | Usage: GiveItem [username] [itemID] [itemQuantity] [itemQuality]", DateTime.Now);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Environment.NewLine);
                    ListenForCommands();
                }

                foreach (ServerClient client in _Networking.connectedClients)
                {
                    if (client.username == clientID)
                    {
                        _Networking.SendData(client, "GiftedItems│" + itemID + "┼" + itemQuantity + "┼" + itemQuality + "┼");

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("[{0}] | Item Has Neen Gifted To Player [{1}]", DateTime.Now, client.username);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(Environment.NewLine);
                        ListenForCommands();
                    }
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Player [{1}] Not Found", DateTime.Now, clientID);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(Environment.NewLine);
            }

            else if (command.StartsWith("Giveitemall ") || command.StartsWith("giveitemall "))
            {
                Console.Clear();

                string itemID = "";
                try { itemID = command.Split(' ')[1]; }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] | Missing Parameters", DateTime.Now);
                    Console.WriteLine("[{0}] | Usage: Giveitemall [itemID] [itemQuantity] [itemQuality]", DateTime.Now);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Environment.NewLine);
                    ListenForCommands();
                }

                string itemQuantity = "";
                try { itemQuantity = command.Split(' ')[2]; }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] | Missing Parameters", DateTime.Now);
                    Console.WriteLine("[{0}] | Usage: Giveitemall [itemID] [itemQuantity] [itemQuality]", DateTime.Now);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Environment.NewLine);
                    ListenForCommands();
                }

                string itemQuality = "";
                try { itemQuality = command.Split(' ')[3]; }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] | Missing Parameters", DateTime.Now);
                    Console.WriteLine("[{0}] | Usage: Giveitemall [itemID] [itemQuantity] [itemQuality]", DateTime.Now);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Environment.NewLine);
                    ListenForCommands();
                }

                foreach (ServerClient client in _Networking.connectedClients)
                {
                    _Networking.SendData(client, "GiftedItems│" + itemID + "┼" + itemQuantity + "┼" + itemQuality + "┼");

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] | Item Has Neen Gifted To All Players", DateTime.Now);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Environment.NewLine);
                    ListenForCommands();
                }
            }

            else if (command.StartsWith("Protect ") || command.StartsWith("protect "))
            {
                Console.Clear();

                string clientID = "";
                try { clientID = command.Split(' ')[1]; }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] | Missing Parameters", DateTime.Now);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Environment.NewLine);
                    ListenForCommands();
                }

                foreach (ServerClient client in _Networking.connectedClients)
                {
                    if (client.username == clientID)
                    {
                        client.eventShielded = true;
                        _MainProgram.savedClients.Find(fetch => fetch.username == client.username).eventShielded = true;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("[{0}] | Player [{1}] Has Been Protected", DateTime.Now, client.username);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(Environment.NewLine);
                        ListenForCommands();
                    }
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Player [{1}] Not Found", DateTime.Now, clientID);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(Environment.NewLine);
            }

            else if (command.StartsWith("Deprotect ") || command.StartsWith("deprotect "))
            {
                Console.Clear();

                string clientID = "";
                try { clientID = command.Split(' ')[1]; }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] | Missing Parameters", DateTime.Now);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Environment.NewLine);
                    ListenForCommands();
                }

                foreach (ServerClient client in _Networking.connectedClients)
                {
                    if (client.username == clientID)
                    {
                        client.eventShielded = false;
                        _MainProgram.savedClients.Find(fetch => fetch.username == client.username).eventShielded = false;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("[{0}] | Player [{1}] Has Been Deprotected", DateTime.Now, client.username);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(Environment.NewLine);
                        ListenForCommands();
                    }
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Player [{1}] Not Found", DateTime.Now, clientID);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(Environment.NewLine);
            }

            else if (command.StartsWith("Immunize ") || command.StartsWith("immunize "))
            {
                Console.Clear();

                string clientID = "";
                try { clientID = command.Split(' ')[1]; }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] | Missing Parameters", DateTime.Now);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Environment.NewLine);
                    ListenForCommands();
                }

                foreach (ServerClient client in _Networking.connectedClients)
                {
                    if (client.username == clientID)
                    {
                        client.isImmunized = true;
                        _MainProgram.savedClients.Find(fetch => fetch.username == client.username).isImmunized = true;
                        SaveSystem.SaveUserData(client);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("[{0}] | Player [{1}] Has Been Inmmunized", DateTime.Now, client.username);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(Environment.NewLine);
                        ListenForCommands();
                    }
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Player [{1}] Not Found", DateTime.Now, clientID);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(Environment.NewLine);
            }

            else if (command.StartsWith("Deimmunize ") || command.StartsWith("deimmunize "))
            {
                Console.Clear();

                string clientID = "";
                try { clientID = command.Split(' ')[1]; }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[{0}] | Missing Parameters", DateTime.Now);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Environment.NewLine);
                    ListenForCommands();
                }

                foreach (ServerClient client in _Networking.connectedClients)
                {
                    if (client.username == clientID)
                    {
                        client.isImmunized = false;
                        _MainProgram.savedClients.Find(fetch => fetch.username == client.username).isImmunized = false;
                        SaveSystem.SaveUserData(client);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("[{0}] | Player [{1}] Has Been Deinmmunized", DateTime.Now, client.username);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(Environment.NewLine);
                        ListenForCommands();
                    }
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Player [{1}] Not Found", DateTime.Now, clientID);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(Environment.NewLine);
            }

            else if (command == "Adminlist" || command == "adminlist")
            {
                adminList.Clear();

                foreach (ServerClient client in savedClients)
                {
                    if (client.isAdmin) adminList.Add(client.username);
                }

                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Server Administrators: [{1}]", DateTime.Now, adminList.Count);
                Console.ForegroundColor = ConsoleColor.White;

                if (adminList.Count() == 0) Console.WriteLine("[{0}] | No Administrators Found", DateTime.Now);
                else foreach (string str in adminList) Console.WriteLine("[{0}] | {1}", DateTime.Now, str);

                Console.WriteLine(Environment.NewLine);
            }

            else if (command == "Whitelist" || command == "whitelist")
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Whitelisted Players: [{1}]", DateTime.Now, whitelistedUsernames.Count);
                Console.ForegroundColor = ConsoleColor.White;

                if (whitelistedUsernames.Count() == 0) Console.WriteLine("[{0}] | No Whitelisted Players Found", DateTime.Now);
                else foreach (string str in whitelistedUsernames) Console.WriteLine("[{0}] | {1}", DateTime.Now, str);

                Console.WriteLine(Environment.NewLine);
            }

            else if (command == "Wipe" || command == "wipe")
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[{0}] | WARNING! THIS ACTION WILL DELETE ALL PLAYER DATA. DO YOU WANT TO PROCEED? (Y/N)", DateTime.Now, whitelistedUsernames.Count);
                Console.ForegroundColor = ConsoleColor.White;

                string response = Console.ReadLine();

                if (response == "Y")
                {
                    foreach(ServerClient client in _Networking.connectedClients)
                    {
                        client.disconnectFlag = true;
                    }

                    Thread.Sleep(1000);

                    foreach(ServerClient client in _MainProgram.savedClients)
                    {
                        client.wealth = 0;
                        client.pawnCount = 0;
                        SaveSystem.SaveUserData(client);
                    }

                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[{0}] | All Player Files Have Been Set To Wipe", DateTime.Now, whitelistedUsernames.Count);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.Clear();
                    ListenForCommands();
                }
            }

            else if (command == "Clear" || command == "clear") Console.Clear();

            else if (command == "Exit" || command == "exit")
            {
                List<ServerClient> clientsToKick = new List<ServerClient>();

                foreach(ServerClient sc in _Networking.connectedClients)
                {
                    clientsToKick.Add(sc);
                }

                foreach (ServerClient sc in clientsToKick)
                {
                    _Networking.SendData(sc, "Disconnect│Closing");
                    sc.disconnectFlag = true;
                }

                Environment.Exit(0);
            }

            else
            {
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[{0}] | Command [{1}] Not Found", DateTime.Now, command);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(Environment.NewLine);
            }

            ListenForCommands();
        }
    }
}