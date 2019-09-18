using System;
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
                chatState = Constants.ProgramState.connecting;
                ClientCreate();
            }

            Console.ReadKey();
        }

        private static void ServerCreate()
        {
            Int32 port = 13000;

            // Home: 192.168.178.34
            // Work: 10.110.113.233

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

            ASCIIEncoding encoder = new ASCIIEncoding();

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
            ASCIIEncoding encoder = new ASCIIEncoding();

            while (true)
            {
                string _userInput = String.Empty;

                _userInput = Console.ReadLine();

                switch (_userInput)
                {
                    case Constants.chatCmdHelp:
                    case Constants.chatCmdBcBlack:
                    case Constants.chatCmdBcWhite:
                    case Constants.chatCmdClear:
                    case Constants.chatCmdCls:
                        WorkChatCommand(_userInput);
                        break;
                    default:
                        byte[] buffer = encoder.GetBytes(Constants.delimUsername + "<Server>" + Constants.delimMsgData + _userInput);

                        foreach (TcpClient broadcastMember in chatObjects.clientList)
                        {
                            NetworkStream broadcastStream = broadcastMember.GetStream();

                            broadcastStream.Write(buffer, 0, buffer.Length);
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
            Output.PrintScreen();
            chatObjects.clientName = Console.ReadLine();

            // Create a TcpClient.
            // Note, for this client to work you need to have a TcpServer 
            // connected to the same address as specified by the server, port
            // combination.
            Int32 port = 13000;
            
            // Home: 192.168.178.34
            // Work: 10.110.113.233
            string ipAddress = "192.168.178.34";

            do
            {
                try
                {
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
                    string message = Console.ReadLine();

                    switch (message)
                    {
                        case Constants.chatCmdHelp:
                        case Constants.chatCmdBcBlack:
                        case Constants.chatCmdBcWhite:
                        case Constants.chatCmdClear:
                        case Constants.chatCmdCls:
                            WorkChatCommand(message);
                            break;
                        default:
                            // Translate the passed message into ASCII and store it as a Byte array.
                            Byte[] data = System.Text.Encoding.ASCII.GetBytes(Constants.delimUsername + chatObjects.clientName +
                                                                              Constants.delimMsgData + message);

                            // Send the message to the connected TcpServer. 
                            stream.Write(data, 0, data.Length);

                            chatObjects.messageData.Add(new Message(chatObjects.clientName, message, Constants.alignmentRight));
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
            ASCIIEncoding encoder = new ASCIIEncoding();
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
                    startPosUsername = dataString.IndexOf(Constants.delimUsername, 0) + Constants.delimUsername.Length;
                    startPosMessage = dataString.IndexOf(Constants.delimMsgData, 0) + Constants.delimMsgData.Length;
                    username = dataString.Substring(startPosUsername, startPosMessage - Constants.delimMsgData.Length - startPosUsername);
                    message = dataString.Substring(startPosMessage);

                    chatObjects.messageData.Add(new Message(username, message, Constants.alignmentLeft));

                    // Print the screen
                    Output.PrintScreen();
                }
                catch
                {
                    
                }
            }
        }

        private static void WorkChatCommand(string chatCommand)
        {
            switch (chatCommand)
            {
                case Constants.chatCmdHelp:
                    Output.PrintHelp();
                    break;
                case Constants.chatCmdBcBlack:
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case Constants.chatCmdBcWhite:
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    break;
                case Constants.chatCmdClear:
                case Constants.chatCmdCls:
                    chatObjects.messageData.Clear();
                    Console.Clear();
                    break;
            }
        }
    }
}
