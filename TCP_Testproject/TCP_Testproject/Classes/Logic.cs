﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace TCP_Testproject.Classes
{
    class Logic
    {
        public static Objects chatObjects = new Objects();
        public static Constants.ProgramState chatState = Constants.ProgramState.initializing;
        public static Regex RegexIpAddress = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b"); // Regex to recognize IP addresses
        public static int scrollOffset = 0;
        public static string enteredMessage = String.Empty;
        public static bool doPrintScreen = true; // To disable the PrintScreen for the client if necessary
        public static bool allowInput = true; // To disable the input for the client if necessary
        public static bool waitForOnlineData = false; // Determines whether a specific client is waiting to receive online data or not
        public static Constants.bcHaXxOrType bcHaXxOrMode = Constants.bcHaXxOrType.Disabled;

        public static void InitClientServer()
        {
            string _input = String.Empty;

            Output.PrintScreen();

            _input = Console.ReadKey().KeyChar.ToString().ToUpper();
            Console.Clear();
            chatState = Constants.ProgramState.initialized;

            if (_input == "H")
            {
                ServerCreate();
            }
            else if (_input == "C")
            {
                ClientCreate();
            }

            Console.ReadKey();
        }

        private static void ServerCreate()
        {
            Int32 port = 13000;

            // we want to listen for connections from any IPv4 adress
            IPAddress ipAddress = IPAddress.Any;

            chatObjects.server = new TcpListener(ipAddress, port);

            // Start listening for client requests.
            chatObjects.server.Start();

            ThreadPool.QueueUserWorkItem(ServerConsoleInput, 1);

            ServerHandleClients();
        }

        private static void ServerHandleClients()
        {
            try
            {
                // Enter the listening loop.
                while (true)
                {
                    Console.WriteLine("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also user server.AcceptSocket() here.
                    TcpClient client = chatObjects.server.AcceptTcpClient();

                    ThreadPool.QueueUserWorkItem(ServerListenSend, client);

                    Console.WriteLine("{0} Connection accepted ip=\"{1}\"", GetTimestampString(), ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                chatObjects.server.Stop();
            }
        }

        private static void ServerListenSend(object client)
        {
            TcpClient _tcpClient = (TcpClient)client;
            Objects.onlineListElement _newOnlineListEntry = new Objects.onlineListElement();

            NetworkStream _clientStream = _tcpClient.GetStream();

            UTF8Encoding _encoder = new UTF8Encoding();

            byte[] _message = new byte[4096];
            int _bytesRead;

            string _bufferincmessage;
            bool _doWorkMsg = true; // Determines if the received message has to be worked or if it is just a status message (e.g. 'Connected client username' message)

            while (true)
            {
                _bytesRead = 0;

                try
                {
                    //blocks until a client sends a message
                    _bytesRead = _clientStream.Read(_message, 0, 4096);
                    string _decodedMessage = _encoder.GetString(_message, 0, _bytesRead);

                    if (_decodedMessage.StartsWith(Constants.delimOnlineData))
                    {
                        Console.WriteLine(_decodedMessage.Remove(0, Constants.delimOnlineData.Length));

                        // Retrieve IP + client username from message and add it to the online list
                        _newOnlineListEntry.TcpClient = _tcpClient;
                        _newOnlineListEntry.IP = ((IPEndPoint)_tcpClient.Client.RemoteEndPoint).Address.ToString();
                        _newOnlineListEntry.Username = _decodedMessage.Substring(Constants.delimOnlineData.Length + GetTimestampString().Length + Constants.txtClientUsrNameEQ.Length);
                        
                        Objects.onlineList.Add(_newOnlineListEntry);

                        // Do no further work with this message because it isn't needed
                        _doWorkMsg = false;
                    }
                    else
                    {
                        Console.WriteLine("{0} Received from ip=\"{1}\" string=\"{2}\"",
                                          GetTimestampString(),
                                          ((IPEndPoint)_tcpClient.Client.RemoteEndPoint).Address.ToString(),
                                          _decodedMessage);
                        _doWorkMsg = true;
                    }
                }
                catch
                {
                    //a socket error has occured
                    Console.WriteLine("{0} Connection disconnect ip=\"{1}\"", GetTimestampString(), ((IPEndPoint)_tcpClient.Client.RemoteEndPoint).Address.ToString());

                    break;
                }

                if (_bytesRead == 0)
                {
                    //the client has disconnected from the server
                    break;
                }

                if (_doWorkMsg)
                {
                    // message has successfully been received
                    // Broadcast recieved message to all clients except the client that sent the information
                    _bufferincmessage = _encoder.GetString(_message, 0, _bytesRead);

                    switch (DetermineIsCommand(_bufferincmessage.Substring(_bufferincmessage.IndexOf(Constants.delimMsgData, 0) + Constants.delimMsgData.Length), Constants.instanceClient))
                    {
                        case true:
                            WorkChatCommand(_bufferincmessage.Substring(_bufferincmessage.IndexOf(Constants.delimMsgData, 0) + Constants.delimMsgData.Length), Constants.instanceServer);
                            break;
                        default:
                            byte[] buffer = _encoder.GetBytes(_bufferincmessage);

                            foreach (Objects.onlineListElement broadcastMember in Objects.onlineList)
                            {
                                if (broadcastMember.TcpClient != client && !chatObjects.muteList.Contains(((IPEndPoint)_tcpClient.Client.RemoteEndPoint).Address.ToString()))
                                {
                                    NetworkStream broadcastStream = broadcastMember.TcpClient.GetStream();

                                    broadcastStream.Write(buffer, 0, buffer.Length);
                                    Console.WriteLine("{0} Sent to ip=\"{1}\" string=\"{2}\"",
                                                      GetTimestampString(),
                                                      ((IPEndPoint)broadcastMember.TcpClient.Client.RemoteEndPoint).Address.ToString(),
                                                      _bufferincmessage);
                                }
                            }
                            break;
                    }
                }
            }

            // Close everything in order for the server not to crash when a user disconnects
            _clientStream.Close();
            Objects.onlineList.Remove(_newOnlineListEntry);
            _tcpClient.Close();
        }

        // Input function for console; do not use fakeinput
        private static void ServerConsoleInput(object fakeinput)
        {
            UTF8Encoding encoder = new UTF8Encoding();

            while (true)
            {
                string _userInput = String.Empty;

                _userInput = Console.ReadLine();

                switch (DetermineIsCommand(_userInput, Constants.instanceServer))
                {
                    case true:
                        WorkChatCommand(_userInput, Constants.instanceServer);
                        break;
                    default:
                        byte[] buffer = encoder.GetBytes(Constants.delimAddData + GetTimestampString() + " | " + Constants.serverUsername +
                                                         Constants.delimMsgData + _userInput + Constants.delimMsgEnd);

                        foreach (Objects.onlineListElement broadcastMember in Objects.onlineList)
                        {
                            NetworkStream broadcastStream = broadcastMember.TcpClient.GetStream();

                            broadcastStream.WriteAsync(buffer, 0, buffer.Length);
                            broadcastStream.FlushAsync();

                            Console.WriteLine("{0} Sent to ip=\"{1}\" string=\"{2}\"",
                                              GetTimestampString(),
                                              ((IPEndPoint)broadcastMember.TcpClient.Client.RemoteEndPoint).Address.ToString(),
                                              encoder.GetString(buffer));
                        }
                        break;
                }
            }
        }

        private static void ClientCreate()
        {
            string _desiredUsername = String.Empty;

            // Only accept the users desired username if it doesn't start with any forbidden string
            do
            {
                _desiredUsername = String.Empty;

                Output.PrintScreen();
                _desiredUsername = Console.ReadLine();
            } while (_desiredUsername.StartsWith(Constants.serverUsername) || _desiredUsername.StartsWith(Constants.delimAddData) ||
                     _desiredUsername.StartsWith(Constants.delimMsgData) || _desiredUsername.StartsWith(Constants.delimMsgEnd));

            chatObjects.clientName = _desiredUsername;

            // Handle input of user for server IPv4 address
            chatState = Constants.ProgramState.connecting;

            // Create a TcpClient.
            // Note, for this client to work you need to have a TcpServer 
            // connected to the same address as specified by the server, port
            // combination.
            Int32 port = 13000;
            
            // Home: 192.168.178.34
            // Work: 10.110.113.233
            string ipAddress = String.Empty;

            do
            {
                Output.PrintScreen();
                ipAddress = Console.ReadLine();
                ipAddress = ipAddress != String.Empty ? ipAddress : "127.0.0.1";
            } while (!RegexIpAddress.IsMatch(ipAddress));

            do
            {
                try
                {
                    // try / retry connection
                    chatState = Constants.ProgramState.connecting;
                    chatObjects.client = new TcpClient(ipAddress, port);
                }
                catch
                {
                    // Set chatState to connection error and retry to connect in 5 seconds
                    chatState = Constants.ProgramState.connectionerror;
                    Output.PrintScreen();
                    Thread.Sleep(5000);
                }
            } while (!chatObjects.client.Connected);

            chatState = Constants.ProgramState.connected;
            Output.PrintScreen();

            // Get a client stream for reading and writing.
            //  Stream stream = client.GetStream();
            NetworkStream stream = chatObjects.client.GetStream();

            // Translate the passed message into UTF-8 and store it as a Byte array.
            // Build logon message for the new client.
            Byte[] data = System.Text.Encoding.UTF8.GetBytes(Constants.delimOnlineData + GetTimestampString() + Constants.txtClientUsrNameEQ + chatObjects.clientName);

            // Send the logon message to the connected TcpServer. 
            stream.WriteAsync(data, 0, data.Length);

            Thread.Sleep(2000);

            ThreadPool.QueueUserWorkItem(ClientListen, chatObjects.client);

            ClientSend();
        }

        private static void ClientSend()
        {
            chatState = Constants.ProgramState.communicating;

            // Get a client stream for reading and writing.
            //  Stream stream = client.GetStream();
            NetworkStream stream = chatObjects.client.GetStream();

            // Print the screen
            Output.PrintScreen();

            try
            {
                while (true)
                {
                    if (allowInput)
                    {
                        ConsoleKeyInfo _pressedKey;
                        string _message = String.Empty;

                        // handle client input
                        do
                        {
                            bool _refreshScreen = false;
                            bool _refreshInput = false;

                            _pressedKey = Console.ReadKey(true);

                            if (_pressedKey.Key == ConsoleKey.PageUp)
                            {
                                if (chatObjects.messageData.Count - scrollOffset > Console.WindowHeight - 6 && chatObjects.messageData.Count > Console.WindowHeight - 6) // Scroll up
                                {
                                    scrollOffset++;
                                    _refreshScreen = true;
                                }
                            }
                            else if (_pressedKey.Key == ConsoleKey.PageDown) // Scroll down
                            {
                                if (scrollOffset > 0 && chatObjects.messageData.Count > Console.WindowHeight - 6)
                                {
                                    scrollOffset--;
                                    _refreshScreen = true;
                                }
                            }
                            else if (_pressedKey.Key == ConsoleKey.Backspace)
                            {
                                if (_message.Length > 0)
                                {
                                    _message = _message.Remove(_message.Length - 1);
                                    _refreshInput = true;
                                }
                            }
                            else if (_pressedKey.Key == ConsoleKey.Decimal) // Debug key
                            {
                                new Thread(new ThreadStart(Output.PrintScreen)).Start();
                                Output.PrintScreen();
                            }
                            else if (_pressedKey.Key != ConsoleKey.Enter)
                            {
                                _message += _pressedKey.KeyChar;
                                _refreshInput = true;
                            }

                            if (_refreshScreen)
                            {
                                Output.PrintScreen();
                            }
                            if (_refreshInput)
                            {
                                enteredMessage = _message;
                                Output.PrintInput();
                            }
                        } while (_pressedKey.Key != ConsoleKey.Enter);

                        enteredMessage = String.Empty;

                        switch (DetermineIsCommand(_message, Constants.instanceClient))
                        {
                            case true:
                                WorkChatCommand(_message, Constants.instanceClient);
                                break;
                            default:
                                // Translate the passed message into UTF-8 and store it as a Byte array.
                                Byte[] data = System.Text.Encoding.UTF8.GetBytes(Constants.delimAddData + GetTimestampString() + " | " + chatObjects.clientName +
                                                                                    Constants.delimMsgData + _message + Constants.delimMsgEnd);

                                // Send the message to the connected TcpServer. 
                                stream.WriteAsync(data, 0, data.Length);
                                stream.FlushAsync();

                                chatObjects.messageData.Add(new Message("You | " + GetTimestampString(), _message, Constants.alignmentRight));
                                break;

                        }

                        if (!_message.StartsWith(Constants.chatCmdMatzesMom) && !_message.StartsWith(Constants.chatCmdOnlineList))
                        {
                            // Print the screen
                            Output.PrintScreen();
                        }
                    }
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

            // Close everything.
            stream.Close();
            chatObjects.client.Close();
            Console.WriteLine("\n Press Enter to continue...");
            Console.Read();
        }

        private static void ClientListen(object client)
        {
            // Get a client stream for reading and writing.
            //  Stream stream = client.GetStream();
            UTF8Encoding encoder = new UTF8Encoding();
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();

            while (true)
            {
                byte[] receivedData = new byte[4096];
                int bytesRead = 0;

                string dataString = "";
                string addData = "";
                string message = "";
                int startPosUsername = 0;
                int startPosMessage = 0;

                // Receive the TcpServer response
                try
                {
                    // blocks until a client sends a message
                    bytesRead = clientStream.Read(receivedData, 0, 4096);

                    // decode the received data
                    dataString = encoder.GetString(receivedData, 0, bytesRead);

                    List<string> receivedMessages = new List<string>();
                    receivedMessages.AddRange(dataString.Split(new string[] { Constants.delimMsgEnd }, StringSplitOptions.RemoveEmptyEntries));

                    foreach (string receivedMessage in receivedMessages)
                    {
                        if (receivedMessage.StartsWith(Constants.delimOnlineData) && waitForOnlineData)
                        {
                            waitForOnlineData = false;
                            // Call the PrintOnlineList in a new thread so new messages can still be received + notifications still work
                            new Thread(() => Output.PrintOnlineList(Constants.instanceClient, receivedMessage)).Start();
                        }
                        else
                        {
                            startPosUsername = receivedMessage.IndexOf(Constants.delimAddData, 0) + Constants.delimAddData.Length;
                            startPosMessage = receivedMessage.IndexOf(Constants.delimMsgData, 0) + Constants.delimMsgData.Length;
                            addData = receivedMessage.Substring(startPosUsername, startPosMessage - Constants.delimMsgData.Length - startPosUsername);
                            message = receivedMessage.Substring(startPosMessage);

                            // Handle commands received from server
                            if (addData == Constants.serverUsername && (message.IndexOf(Constants.chatCmdClearAll) == 0 || message.IndexOf(Constants.chatCmdClsAll) == 0))
                            {
                                WorkChatCommand(message, Constants.instanceClient);
                            }
                            else
                            {
                                chatObjects.messageData.Add(new Message(addData, message, Constants.alignmentLeft));
                            }
                        }
                    }

                    // Let the program flash
                    if (!Program.ProgramHasFocus())
                    {
                        if (Properties.Settings.Default.doNotifyVisual)
                        {
                            Program.StartFlashTaskbarIcon();
                        }
                        if (Properties.Settings.Default.doNotifySound)
                        {
                            Console.Beep();
                        }
                    }

                    if (doPrintScreen)
                    {
                        // Print the screen
                        Output.PrintScreen();
                    }
                }
                catch
                {
                    
                }
            }
        }

        private static bool DetermineIsCommand(string message, string currInstance)
        {
            if (message.StartsWith(Constants.chatCmdHelp) || 
                message.StartsWith(Constants.chatCmdBcBlack) || message.StartsWith(Constants.chatCmdBcWhite) ||
                message.StartsWith(Constants.chatCmdBcHaXxOr) || message.StartsWith(Constants.chatCmdBcHaXxOrWhite) || 
                message.Trim() == Constants.chatCmdClear || message.Trim() == Constants.chatCmdCls || 
                message.StartsWith(Constants.chatCmdClearAll) || message.StartsWith(Constants.chatCmdClsAll) ||
                message.StartsWith(Constants.chatCmdNotificationBase) ||
                message.StartsWith(Constants.chatCmdOnlineList) || 
                message.StartsWith(Constants.chatCmdMatzesMom) || 
                (currInstance == Constants.instanceServer && message.StartsWith(Constants.chatCmdMuteBase))) 
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        private static void WorkChatCommand(string chatCommand, string currInstance)
        {
            if (chatCommand.StartsWith(Constants.chatCmdHelp))
            {
                Output.PrintHelp(currInstance);
            }
            else if (chatCommand.Contains(Constants.chatCmdCommandHelp))
            {
                Output.PrintCommandHelp(chatCommand, currInstance);
            }
            else if (chatCommand.StartsWith(Constants.chatCmdBcBlack))
            {
                bcHaXxOrMode = Constants.bcHaXxOrType.Disabled;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;
                Output.consoleColorServer = Constants.consoleColorServerBcBlack;
            }
            else if (chatCommand.StartsWith(Constants.chatCmdBcWhite))
            {
                Console.Clear();
                bcHaXxOrMode = Constants.bcHaXxOrType.Disabled;
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                Output.consoleColorServer = Constants.consoleColorServerBcWhite;
            }
            else if (chatCommand.StartsWith(Constants.chatCmdBcHaXxOrWhite))
            {
                Console.Clear();
                bcHaXxOrMode = Constants.bcHaXxOrType.HaXxOrWhite;
                Output.consoleColorServer = Constants.consoleColorServerBcHaXxOrWhite;
            }
            else if (chatCommand.StartsWith(Constants.chatCmdBcHaXxOr))
            {
                bcHaXxOrMode = Constants.bcHaXxOrType.HaXxOr;
                Output.consoleColorServer = Constants.consoleColorServerBcHaXxOr;
            }
            else if (chatCommand.StartsWith(Constants.chatCmdClearAll) ||
                     chatCommand.StartsWith(Constants.chatCmdClsAll))
            {
                 if (currInstance == Constants.instanceServer)
                 {
                     UTF8Encoding encoder = new UTF8Encoding();
                     byte[] buffer = encoder.GetBytes(Constants.delimAddData + Constants.serverUsername + Constants.delimMsgData + Constants.chatCmdClsAll);
                 
                     foreach (Objects.onlineListElement broadcastMember in Objects.onlineList)
                     {
                         NetworkStream broadcastStream = broadcastMember.TcpClient.GetStream();
                 
                         broadcastStream.WriteAsync(buffer, 0, buffer.Length);
                         broadcastStream.FlushAsync();
                     }
                 }
                 else if (currInstance == Constants.instanceClient)
                 {
                     chatObjects.messageData.Clear();
                 }
                 Console.Clear();
            }
            else if (chatCommand.StartsWith(Constants.chatCmdClear) || 
                     chatCommand.StartsWith(Constants.chatCmdCls))
            { 
                chatObjects.messageData.Clear();
                Console.Clear();
            }
            else if (chatCommand.StartsWith(Constants.chatCmdNotificationBase))
            {
                bool _showConfig = true;

                if (chatCommand.Contains(Constants.chatCmdNotificationSound))
                {
                    Properties.Settings.Default.doNotifySound = Properties.Settings.Default.doNotifySound == true ? false : true;
                    _showConfig = false;
                }
                if (chatCommand.Contains(Constants.chatCmdNotificationVisual))
                {
                    Properties.Settings.Default.doNotifyVisual = Properties.Settings.Default.doNotifyVisual == true ? false : true;
                    _showConfig = false;
                }
                Properties.Settings.Default.Save();

                if (_showConfig)
                {
                    string notificationConfig = "Notification: sound = " +
                                                (Properties.Settings.Default.doNotifySound == true ? "enabled; " : "disabled; ") +
                                                "visual = " +
                                                (Properties.Settings.Default.doNotifyVisual == true ? "enabled;" : "disabled;");

                    chatObjects.messageData.Add(new Message(chatObjects.clientName, notificationConfig, Constants.alignmentCenter));
                }
            }
            else if (chatCommand.StartsWith(Constants.chatCmdOnlineList))
            {
                if (currInstance == Constants.instanceServer)
                {
                    UTF8Encoding encoder = new UTF8Encoding();
                    string _onlineDataString = String.Empty;

                    // Build the data string
                    foreach (Objects.onlineListElement connectedClient in Objects.onlineList)
                    {
                        _onlineDataString += Constants.delimOnlineData + "IP: " + connectedClient.IP.PadRight(15) + " | Username: " + connectedClient.Username;
                    }
                    // Print server output already so it isn't necessary to remove the delimMsgEnd for the servers output 
                    Output.PrintOnlineList(Constants.instanceServer, _onlineDataString);

                    _onlineDataString += Constants.delimMsgEnd;

                    // broadcast the data string to each client
                    byte[] buffer = encoder.GetBytes(_onlineDataString);

                    foreach (Objects.onlineListElement broadcastMember in Objects.onlineList)
                    {
                        NetworkStream broadcastStream = broadcastMember.TcpClient.GetStream();

                        broadcastStream.WriteAsync(buffer, 0, buffer.Length);
                        broadcastStream.FlushAsync();
                    }
                }
                else if (currInstance == Constants.instanceClient)
                {
                    allowInput = false; // Do not allow any input while online information is displayed
                    doPrintScreen = false; // Do not print while online information is being displayed
                    waitForOnlineData = true; // Tell the invoking client that it is now listening for online data
                    // Get a client stream for reading and writing.
                    //  Stream stream = client.GetStream();
                    NetworkStream stream = chatObjects.client.GetStream();

                    // Translate the passed message into UTF-8 and store it as a Byte array.
                    Byte[] data = System.Text.Encoding.UTF8.GetBytes(Constants.delimAddData + GetTimestampString() + " | " + chatObjects.clientName +
                                                                     Constants.delimMsgData + Constants.chatCmdOnlineList + Constants.delimMsgEnd);

                    // Send the message to the connected TcpServer. 
                    stream.WriteAsync(data, 0, data.Length);
                    stream.FlushAsync();
                }
            }
            else if (chatCommand.StartsWith(Constants.chatCmdMatzesMom))
            {
                if (currInstance == Constants.instanceServer)
                {
                    Random random = new Random();
                    int cntRandom = random.Next(0, Objects.matzesMomJokes.Count);

                    UTF8Encoding encoder = new UTF8Encoding();
                    byte[] buffer = encoder.GetBytes(Constants.delimAddData + Constants.serverUsername + Constants.delimMsgData + Objects.matzesMomJokes[cntRandom]);

                    foreach (Objects.onlineListElement broadcastMember in Objects.onlineList)
                    {
                        NetworkStream broadcastStream = broadcastMember.TcpClient.GetStream();

                        broadcastStream.WriteAsync(buffer, 0, buffer.Length);
                        broadcastStream.FlushAsync();
                    }
                }
                else if (currInstance == Constants.instanceClient)
                {
                    // Get a client stream for reading and writing.
                    //  Stream stream = client.GetStream();
                    NetworkStream stream = chatObjects.client.GetStream();

                    // Translate the passed message into UTF-8 and store it as a Byte array.
                    Byte[] data = System.Text.Encoding.UTF8.GetBytes(Constants.delimAddData + GetTimestampString() + " | " + chatObjects.clientName +
                                                                      Constants.delimMsgData + Constants.chatCmdMatzesMom);

                    // Send the message to the connected TcpServer. 
                    stream.WriteAsync(data, 0, data.Length);
                    stream.FlushAsync();
                }
            }
            else if (chatCommand.StartsWith(Constants.chatCmdMuteBase))
            {
                if (chatCommand.Contains(Constants.chatCmdMuteAdd))
                {              
                    // Find all IPs in the input
                    MatchCollection MatchedIps = RegexIpAddress.Matches(chatCommand);
                    // Add all specified IPs to the muteList
                    foreach (Match IpToMute in MatchedIps)
                    {
                        if (!chatObjects.muteList.Contains(IpToMute.Value))
                        {
                            chatObjects.muteList.Add(IpToMute.Value);
                        }
                    }
                }
                else if (chatCommand.Contains(Constants.chatCmdMuteRemove))
                {
                    // Remove an ip from the muteList
                    // Regex to recognize IP addresses
                    Regex RegexIpAddress = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");

                    // Find all IPs in the input
                    MatchCollection MatchedIps = RegexIpAddress.Matches(chatCommand);
                    // Add all specified IPs to the muteList
                    foreach (Match IpToUnmute in MatchedIps)
                    {
                        if (chatObjects.muteList.Contains(IpToUnmute.Value))
                        {
                            chatObjects.muteList.Remove(IpToUnmute.Value);
                        }
                    }
                }
                else
                {
                    // Print out all ips from the muteList
                    foreach (string MutedIp in chatObjects.muteList)
                    {
                        Console.WriteLine(MutedIp);
                    }
                }
            }
        }

        // Returns a formatted timestamp string
        private static string GetTimestampString()
        {
            return DateTime.Now.ToString("T", new CultureInfo("de-DE"));
        }
    }
}
