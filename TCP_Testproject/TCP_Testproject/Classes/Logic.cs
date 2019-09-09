using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");

            chatObjects.server = new TcpListener(ipAddress, port);

            // Start listening for client requests.
            chatObjects.server.Start();
            
            ServerListen();
        }

        private static void ServerListen()
        {
            try
            {
                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;

                // Enter the listening loop.
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also user server.AcceptSocket() here.
                    TcpClient client = chatObjects.server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);

                        // Process the data sent by the client.
                        data = data.ToUpper();

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", data);
                    }

                    // Shutdown and end connection
                    client.Close();
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

        private static void ClientCreate()
        {
            // Create a TcpClient.
            // Note, for this client to work you need to have a TcpServer 
            // connected to the same address as specified by the server, port
            // combination.
            Int32 port = 13000;
            string ipAddress = "127.0.0.1";

            chatObjects.client = new TcpClient(ipAddress, port);

            chatState = Constants.ProgramState.connected;

            ClientListenSend();
        }

        private static void ClientListenSend()
        {
            // Get a client stream for reading and writing.
            //  Stream stream = client.GetStream();
            NetworkStream stream = chatObjects.client.GetStream();

            try
            {
                while (true)
                {
                    Output.PrintScreen();

                    string message = Console.ReadLine();

                    // Translate the passed message into ASCII and store it as a Byte array.
                    Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                    // Send the message to the connected TcpServer. 
                    stream.Write(data, 0, data.Length);

                    chatObjects.messageData.Add(new Message(message, Constants.alignmentRight));

                    // Receive the TcpServer.response.

                    // Buffer to store the response bytes.
                    data = new Byte[256];

                    // String to store the response ASCII representation.
                    String responseData = String.Empty;

                    // Read the first batch of the TcpServer response bytes.
                    Int32 bytes = stream.Read(data, 0, data.Length);
                    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

                    chatObjects.messageData.Add(new Message(responseData, Constants.alignmentLeft));
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
    }
}
