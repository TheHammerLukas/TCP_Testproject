using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TCP_Testproject.Classes
{
    class Logic
    {
        public static Objects chatObjects = new Objects();
        public static Constants.ProgramState chatState = Constants.ProgramState.initializing;
        public static int scrollOffset = 0;
        public static string enteredMessage = String.Empty;

        public static void InitClientServer()
        {
            string _input = "";

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

                    Console.WriteLine("Connection accepted ip=\"{0}\"", ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
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
            TcpClient tcpClient = (TcpClient)client;

            // Add new TcpClients to the list of connected clients
            if (!chatObjects.clientList.Contains(tcpClient))
            {
                chatObjects.clientList.Add(tcpClient);
            }

            NetworkStream clientStream = tcpClient.GetStream();

            UTF8Encoding encoder = new UTF8Encoding();

            byte[] message = new byte[4096];
            int bytesRead;

            string bufferincmessage;

            while (true)
            {
                bytesRead = 0;

                try
                {
                    //blocks until a client sends a message
                    bytesRead = clientStream.Read(message, 0, 4096);
                    Console.WriteLine("Received from ip=\"{0}\" string=\"{1}\"", 
                                      ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString(),
                                      encoder.GetString(message, 0, bytesRead));
                }
                catch
                {
                    //a socket error has occured
                    Console.WriteLine("Connection disconnect ip=\"{0}\"", ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString());
                    break;
                }

                if (bytesRead == 0)
                {
                    //the client has disconnected from the server
                    break;
                }

                //message has successfully been received
                // Broadcast recieved message to all clients except the client that sent the information
                bufferincmessage = encoder.GetString(message, 0, bytesRead);
                byte[] buffer = encoder.GetBytes(bufferincmessage);

                foreach (TcpClient broadcastMember in chatObjects.clientList)
                {
                    if (broadcastMember != client)
                    {
                        NetworkStream broadcastStream = broadcastMember.GetStream();

                        broadcastStream.Write(buffer, 0, buffer.Length);
                        Console.WriteLine("Sent to ip=\"{0}\" string=\"{1}\"", 
                                          ((IPEndPoint)broadcastMember.Client.RemoteEndPoint).Address.ToString(), 
                                          bufferincmessage);
                    }
                }
            }

            // Close everything in order for the server not to crash when a user disconnects
            clientStream.Close();
            chatObjects.clientList.Remove(tcpClient);
            tcpClient.Close();
        }

        // Input function for console; do not use fakeinput
        private static void ServerConsoleInput(object fakeinput)
        {
            UTF8Encoding encoder = new UTF8Encoding();

            while (true)
            {
                string _userInput = String.Empty;

                _userInput = Console.ReadLine();

                switch (DetermineIsCommand(_userInput, Constants.InstanceServer))
                {
                    case true:
                        WorkChatCommand(_userInput);
                        break;
                    default:
                        byte[] buffer = encoder.GetBytes(Constants.delimAddData + GetTimestampString() + " | " + Constants.serverUsername + Constants.delimMsgData + _userInput);

                        foreach (TcpClient broadcastMember in chatObjects.clientList)
                        {
                            NetworkStream broadcastStream = broadcastMember.GetStream();

                            broadcastStream.WriteAsync(buffer, 0, buffer.Length);
                            broadcastStream.FlushAsync();

                            Console.WriteLine("Sent to ip=\"{0}\" string=\"{1}\"",
                                              ((IPEndPoint)broadcastMember.Client.RemoteEndPoint).Address.ToString(),
                                              encoder.GetString(buffer));
                        }
                        break;
                }
            }
        }

        private static void ClientCreate()
        {
            string _desiredUsername = String.Empty;

            // Only accept the users desired username if it doesn't immitate the server username
            do
            {
                _desiredUsername = String.Empty;

                Output.PrintScreen();
                _desiredUsername = Console.ReadLine();
            } while (_desiredUsername == Constants.serverUsername || _desiredUsername == Constants.delimAddData ||
                     _desiredUsername == Constants.delimMsgData);

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

            Output.PrintScreen();
            ipAddress = Console.ReadLine();
            ipAddress = ipAddress != String.Empty ? ipAddress : "127.0.0.1";

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
                    ConsoleKeyInfo _pressedKey;
                    string _message = String.Empty;

                    do
                    {
                        bool _refreshScreen = false;
                        bool _refreshInput = false;

                        _pressedKey = Console.ReadKey(true);
                        
                        if (_pressedKey.Key == ConsoleKey.PageUp)
                        {
                            if (scrollOffset < chatObjects.messageData.Count - 1)
                            {
                                scrollOffset++;
                            }
                            _refreshScreen = true;
                        }
                        else if (_pressedKey.Key == ConsoleKey.PageDown)
                        {
                            if (scrollOffset > 0)
                            {
                                scrollOffset--;
                            }
                            _refreshScreen = true;
                        }
                        else if (_pressedKey.Key == ConsoleKey.Backspace && _message.Length > 0)
                        {
                            _message = _message.Remove(_message.Length - 1);
                            _refreshInput = true;
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

                    switch (DetermineIsCommand(_message, Constants.InstanceClient))
                    {
                        case true:
                            WorkChatCommand(_message);
                            break;
                        default:
                            // Translate the passed message into UTF-8 and store it as a Byte array.
                            Byte[] data = System.Text.Encoding.UTF8.GetBytes(Constants.delimAddData + GetTimestampString() + " | " + chatObjects.clientName +
                                                                              Constants.delimMsgData + _message);

                            // Send the message to the connected TcpServer. 
                            stream.WriteAsync(data, 0, data.Length);
                            stream.FlushAsync();

                            chatObjects.messageData.Add(new Message("You | " + GetTimestampString(), _message, Constants.alignmentRight));
                            break;
                    }
                    // Print the screen
                    Output.PrintScreen();
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
                string username = "";
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
                    startPosUsername = dataString.IndexOf(Constants.delimAddData, 0) + Constants.delimAddData.Length;
                    startPosMessage = dataString.IndexOf(Constants.delimMsgData, 0) + Constants.delimMsgData.Length;
                    username = dataString.Substring(startPosUsername, startPosMessage - Constants.delimMsgData.Length - startPosUsername);
                    message = dataString.Substring(startPosMessage);

                    // Handle commands received from server
                    if (username == Constants.serverUsername && (message.IndexOf(Constants.chatCmdClearAll) == 0 || message.IndexOf(Constants.chatCmdClsAll) == 0))
                    {
                        WorkChatCommand(message);
                    }
                    else
                    {
                        chatObjects.messageData.Add(new Message(username, message, Constants.alignmentLeft));
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

                    // Print the screen
                    Output.PrintScreen();
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
                message.StartsWith(Constants.chatCmdClear) || message.StartsWith(Constants.chatCmdCls) ||
                currInstance == Constants.InstanceServer && 
                (message.StartsWith(Constants.chatCmdClearAll) || message.StartsWith(Constants.chatCmdClsAll)) ||
                message.StartsWith(Constants.chatCmdNotificationBase)) 
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        private static void WorkChatCommand(string chatCommand)
        {
            if (chatCommand.StartsWith(Constants.chatCmdHelp))
            {
                Output.PrintHelp();
            }
            else if (chatCommand.Contains(Constants.chatCmdCommandHelp))
            {
                Output.PrintCommandHelp(chatCommand);
            }
            else if (chatCommand.StartsWith(Constants.chatCmdBcBlack))
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else if (chatCommand.StartsWith(Constants.chatCmdBcWhite))
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
            }
            else if (chatCommand.StartsWith(Constants.chatCmdClear) || 
                     chatCommand.StartsWith(Constants.chatCmdCls) ||
                     chatCommand.StartsWith(Constants.chatCmdClearAll) ||
                     chatCommand.StartsWith(Constants.chatCmdClsAll))
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
        }

        private static string GetTimestampString()
        {
            return DateTime.Now.ToString("T", new CultureInfo("de-DE"));
        }
    }
}
