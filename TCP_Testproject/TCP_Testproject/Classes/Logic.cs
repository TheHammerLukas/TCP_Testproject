using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
            IPAddress ipAddress = IPAddress.Parse("10.110.113.233");

            chatObjects.server = new TcpListener(ipAddress, port);

            // Start listening for client requests.
            chatObjects.server.Start();
            
            ServerListen();
        }

        private static void ServerListen()
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

                    Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));

                    clientThread.Start(client);
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

        private static void HandleClientComm(object client)
        {
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
                bufferincmessage = encoder.GetString(message, 0, bytesRead);
                byte[] buffer = encoder.GetBytes(bufferincmessage);

                clientStream.WriteAsync(buffer, 0, buffer.Length);
                Console.WriteLine("Sent: {0}", bufferincmessage);
                //clientStream.Flush();
            }
        }

        private static void ClientCreate()
        {
            // Create a TcpClient.
            // Note, for this client to work you need to have a TcpServer 
            // connected to the same address as specified by the server, port
            // combination.
            Int32 port = 13000;
            string ipAddress = "10.110.113.233";

            chatObjects.client = new TcpClient(ipAddress, port);

            chatState = Constants.ProgramState.connected;

            ClientListenSend();
        }

        private static void ClientListenSend()
        {
            Thread clientlisten = new Thread(new ThreadStart(ClientListen));

            clientlisten.Start();

            // Get a client stream for reading and writing.
            //  Stream stream = client.GetStream();
            NetworkStream stream = chatObjects.client.GetStream();

            try
            {
                while (true)
                {
                    string message = Console.ReadLine();

                    // Translate the passed message into ASCII and store it as a Byte array.
                    Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                    // Send the message to the connected TcpServer. 
                    stream.Write(data, 0, data.Length);

                    chatObjects.messageData.Add(new Message(message, Constants.alignmentRight));
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

        private static void ClientListen()
        {
            while (true)
            {
                Byte[] data;

                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();
                NetworkStream stream = chatObjects.client.GetStream();

                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

                chatObjects.messageData.Add(new Message(responseData, Constants.alignmentLeft));

                Output.PrintScreen();
                Console.WriteLine("test");
            }
        }
    }
}
