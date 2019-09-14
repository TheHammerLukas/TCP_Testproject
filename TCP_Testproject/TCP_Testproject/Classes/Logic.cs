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
                Output.PrintScreen();
                chatObjects.clientName = Console.ReadLine();

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
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");

            chatObjects.server = new TcpListener(ipAddress, port);

            // Start listening for client requests.
            chatObjects.server.Start();
            
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

                    Console.WriteLine("Connected!");
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
            if (!chatObjects.clientList.Contains(client))
            {
                chatObjects.clientList.Add((TcpClient)client);
            }
            TcpClient tcpClient = (TcpClient)client;
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
                    Console.WriteLine("Received: {0}", encoder.GetString(message, 0, bytesRead));
                }
                catch
                {
                    //a socket error has occured
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
                        Console.WriteLine("Sent to {0}: {1}", broadcastStream.ToString(), bufferincmessage);
                    }
                }
            }
        }

        private static void ClientCreate()
        {
            // Create a TcpClient.
            // Note, for this client to work you need to have a TcpServer 
            // connected to the same address as specified by the server, port
            // combination.
            Int32 port = 13000;
            
            // Home: 192.168.178.34
            // Work: 10.110.113.233
            string ipAddress = "127.0.0.1";

            chatObjects.client = new TcpClient(ipAddress, port);

            chatState = Constants.ProgramState.connected;

            ThreadPool.QueueUserWorkItem(ClientListen, chatObjects.client);

            ClientSend();
        }

        private static void ClientSend()
        {
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

                    // Translate the passed message into ASCII and store it as a Byte array.
                    Byte[] data = System.Text.Encoding.ASCII.GetBytes(Constants.delimUsername + chatObjects.clientName + 
                                                                      Constants.delimMsgData + message);

                    // Send the message to the connected TcpServer. 
                    stream.Write(data, 0, data.Length);

                    chatObjects.messageData.Add(new Message(chatObjects.clientName, message, Constants.alignmentRight));

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
    }
}
