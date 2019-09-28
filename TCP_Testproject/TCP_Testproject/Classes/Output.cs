using System;

namespace TCP_Testproject.Classes
{
    class Output
    {
        static public void PrintScreen()
        {
            PrintHeader();

            if (Logic.chatState == Constants.ProgramState.initializing)
            {
                PrintInitInterface();
            }
            else if (Logic.chatState == Constants.ProgramState.initialized)
            {
                PrintClientCreate();
            }
            else if (Logic.chatState == Constants.ProgramState.connecting)
            {
                PrintClientSelectServer();
            }
            else if (Logic.chatState == Constants.ProgramState.connected)
            {
                PrintClientConnectSuccess();
            }
            else if (Logic.chatState == Constants.ProgramState.connectionerror)
            {
                PrintClientConnectError();
            }
            else if (Logic.chatState == Constants.ProgramState.communicating)
            {
                PrintClientInterface();
            }
        }

        static public void PrintHelp()
        {
            PrintHeader();

            PrintString("Help Menu:", Constants.alignmentCenter);
            PrintString("/bcblack = change to dark theme", Constants.alignmentLeft);
            PrintString("/bcwhite = change to white theme", Constants.alignmentLeft);
            PrintString("/clear or /cls = clear screen", Constants.alignmentLeft);
            PrintString("/notification = manage notification settings", Constants.alignmentLeft);
            PrintString("Press any key to return to chat!", Constants.alignmentCenter);

            Console.ReadKey();
        }

        static public void PrintCommandHelp(string chatCommand)
        {
            PrintHeader();

            PrintString("Help Menu for specific commands:", Constants.alignmentCenter);

            if (chatCommand.StartsWith(Constants.chatCmdBcBlack))
            {
                PrintString("/bcblack is used to switch to the dark theme", Constants.alignmentLeft);    
            }
            else if (chatCommand.StartsWith(Constants.chatCmdBcWhite))
            {
                PrintString("/bcwhite is used to switch to the light theme", Constants.alignmentLeft);
            }
            else if (chatCommand.StartsWith(Constants.chatCmdClear) || chatCommand.StartsWith(Constants.chatCmdCls))
            {
                PrintString("/clear or /cls deletes all previous messages and clears the chat for the invoking user", Constants.alignmentLeft);
            }
            else if (chatCommand.StartsWith(Constants.chatCmdClearAll) || chatCommand.StartsWith(Constants.chatCmdClsAll))
            {
                PrintString("/clear or /cls deletes all previous messages and clears the chat for all clients + the server", Constants.alignmentLeft);
                PrintString("this command can only be invoked by the server and not by a client", Constants.alignmentLeft);
            }
            else if (chatCommand.StartsWith(Constants.chatCmdNotificationBase))
            {
                PrintString("/notification is used to display the current notification settings", Constants.alignmentLeft);
                PrintString("-s is used to toggle the sound notification 'on' or 'off'", Constants.alignmentLeft);
                PrintString("-v is used to toggle the visual notification 'on' or 'off'", Constants.alignmentLeft);
            }
            else
            {
                PrintString("No help exists for the specified command!", Constants.alignmentLeft);
            }
            PrintString("Press any key to return to chat!", Constants.alignmentCenter);

            Console.ReadKey();
        }

        static private void PrintHeader()
        {
            Console.Clear();

            string textHeader = "TCP-IP Chat";

            // First Line
            Console.Write("///");
            for (int i = 0; i < (Console.WindowWidth - (textHeader.Length + 12)) / 2; i++)
            {
                Console.Write("=");
            }
            Console.Write("///");
            for (int i = 0; i < textHeader.Length; i++)
            {
                Console.Write("-");
            }
            Console.Write("\\\\\\");
            for (int i = 0; i < (Console.WindowWidth - (textHeader.Length + 12)) / 2; i++)
            {
                Console.Write("=");
            }
            Console.WriteLine("\\\\\\");

            // Second Line
            Console.Write("|||");
            for (int i = 0; i < (Console.WindowWidth - (textHeader.Length + 12)) / 2; i++)
            {
                Console.Write("=");
            }
            Console.Write("|||");
            Console.Write(textHeader);
            Console.Write("|||");
            for (int i = 0; i < (Console.WindowWidth - (textHeader.Length + 12)) / 2; i++)
            {
                Console.Write("=");
            }
            Console.WriteLine("|||");

            // Third Line
            Console.Write("\\\\\\");
            for (int i = 0; i < (Console.WindowWidth - (textHeader.Length + 12)) / 2; i++)
            {
                Console.Write("=");
            }
            Console.Write("\\\\\\");
            for (int i = 0; i < textHeader.Length; i++)
            {
                Console.Write("-");
            }
            Console.Write("///");
            for (int i = 0; i < (Console.WindowWidth - (textHeader.Length + 12)) / 2; i++)
            {
                Console.Write("=");
            }
            Console.WriteLine("///");
        }

        static private void PrintFooter()
        {
            for (int i = 0; i < Console.WindowWidth - 1; i++)
            {
                Console.Write("-");
            }
            Console.WriteLine();
            PrintString("Type your message and press enter to send:", Constants.alignmentLeft);
        }

        static private void PrintInitInterface()
        {
            PrintString("Host server | Connect as client [H|C]", Constants.alignmentCenter);
        }

        static private void PrintClientCreate()
        {
            PrintString("Enter your desired username:", Constants.alignmentCenter);
        }

        static private void PrintClientSelectServer()
        {
            PrintString("Enter the IPv4 address of the server:", Constants.alignmentCenter);
        }


        static private void PrintClientConnectSuccess()
        {
            PrintString("Successfully connected to the server!", Constants.alignmentCenter);
        }

        static private void PrintClientConnectError()
        {
            PrintString("Error while connecting to the server! Retrying in 5 seconds...", Constants.alignmentCenter);
        }

        static private void PrintClientInterface()
        {
            string _outputString = "";
            string _textAlignment = "";
            int _cntMessagesToHide = 0;

            // This for loop is so that the console does not autoscroll;
            for (int i = Logic.chatObjects.messageData.Count - Logic.scrollOffset; i >= Logic.chatObjects.messageData.Count - Console.WindowHeight + 6 - Logic.scrollOffset; i--)
            {
                try
                {
                    if (Logic.chatObjects.messageData[i].additionalInfo.Length + 2 + Logic.chatObjects.messageData[i].message.Length <= Console.WindowWidth - 1)
                    {
                        // nothing happens => check next message;
                    }
                    else if (Logic.chatObjects.messageData[i].additionalInfo.Length + 2 + Logic.chatObjects.messageData[i].message.Length <= (Console.WindowWidth - 1) * 2)
                    {
                        _cntMessagesToHide += 1;
                    }
                    else if (Logic.chatObjects.messageData[i].additionalInfo.Length + 2 + Logic.chatObjects.messageData[i].message.Length > Console.WindowWidth - 1)
                    {
                        _cntMessagesToHide += 2;
                    }
                    else
                    {
                        // nothing happens => check next message;
                    }

                    if (Logic.chatObjects.messageData.Count - Console.WindowHeight + (6 + _cntMessagesToHide + Logic.scrollOffset) > Logic.chatObjects.messageData.Count)
                    {
                        _cntMessagesToHide = 0;
                    }
                }
                catch
                {
                    // nothing happens
                }
            }

            for (int i = Logic.chatObjects.messageData.Count - Console.WindowHeight + (6 + _cntMessagesToHide) - Logic.scrollOffset < 0 ?
                 0 : Logic.chatObjects.messageData.Count - Console.WindowHeight + (6 + _cntMessagesToHide) - Logic.scrollOffset;
                 i < Logic.chatObjects.messageData.Count - Logic.scrollOffset;
                 i++)
            {
                _outputString = Logic.chatObjects.messageData[i].message;
                _textAlignment = Logic.chatObjects.messageData[i].alignment;

                // Only display the username if it is a message that has been received from the server
                if (_textAlignment == Constants.alignmentLeft)
                {
                    _outputString = Logic.chatObjects.messageData[i].additionalInfo + ": " + _outputString;
                }
                else if (_textAlignment == Constants.alignmentRight)
                {
                    for (int y = 0;
                         y <= Console.WindowWidth - (2 + Logic.chatObjects.messageData[i].additionalInfo.Length) -
                                                   (Logic.chatObjects.messageData[i].message.Length % Console.WindowWidth) - 2
                         && Logic.chatObjects.messageData[i].additionalInfo.Length + 2 + Logic.chatObjects.messageData[i].message.Length > Console.WindowWidth - 1;
                         y++)
                    {
                        _outputString = _outputString + " ";
                    }
                    _outputString = _outputString + ": " + Logic.chatObjects.messageData[i].additionalInfo;
                }
                PrintString(_outputString, _textAlignment);
            }
            PrintFooter();
        }

        static private void PrintString(string outputString, string textAlignment)
        {
            switch (textAlignment)
            {
                case Constants.alignmentLeft:
                    Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);
                    Console.WriteLine(outputString);
                    break;
                case Constants.alignmentCenter:
                    Console.SetCursorPosition(((Console.WindowWidth - outputString.Length) / 2), Console.CursorTop);
                    Console.WriteLine(outputString);
                    break;
                case Constants.alignmentRight:
                    // Cursorleft is the minimun value for the console bounds; PrintString with alignment left => Client will not crash
                    if ((Console.WindowWidth - outputString.Length) -1 < 0)
                    {
                        PrintString(outputString, Constants.alignmentLeft);
                    }
                    else
                    {
                        Console.SetCursorPosition((Console.WindowWidth - outputString.Length) - 1, Console.CursorTop);
                        Console.WriteLine(outputString);
                    }
                    break;
            }
        }
    }
}
