using System;

namespace TCP_Testproject.Classes
{
    class Output
    {
        static public bool isPrinting = false;
        static public ConsoleColor consoleColorServer = Constants.consoleColorServerBcBlack;

        static public void PrintScreen()
        {
            if (Logic.chatState == Constants.ProgramState.initializing)
            {
                PrintHeader();
                PrintInitInterface();
            }
            else if (Logic.chatState == Constants.ProgramState.initialized)
            {
                PrintHeader();
                PrintClientCreate();
            }
            else if (Logic.chatState == Constants.ProgramState.connecting)
            {
                PrintHeader();
                PrintClientSelectServer();
            }
            else if (Logic.chatState == Constants.ProgramState.connected)
            {
                PrintHeader();
                PrintClientConnectSuccess();
            }
            else if (Logic.chatState == Constants.ProgramState.connectionerror)
            {
                PrintHeader();
                PrintClientConnectError();
            }
            else if (Logic.chatState == Constants.ProgramState.communicating)
            {
                // try to print; only print when not currently printing already
                bool _cycleIsPrinting;

                do
                {
                    _cycleIsPrinting = isPrinting;
                    if (!_cycleIsPrinting)
                    {
                        isPrinting = true;
                        PrintHeader();
                        PrintClientInterface();
                    }
                } while (_cycleIsPrinting);
            }
        }

        static public void PrintHelp(string currInstance)
        {
            if (currInstance == Constants.instanceServer)
            {
                PrintHeader();
            }

            PrintString("Help Menu:", Constants.alignmentCenter);
            PrintString(string.Concat(Constants.chatCmdBcBlack, "  = change to dark theme"), Constants.alignmentLeft);
            PrintString(string.Concat(Constants.chatCmdBcWhite, " = change to white theme"), Constants.alignmentLeft);
            PrintString(string.Concat(Constants.chatCmdBcHaXxOr, " = change to hacker theme"), Constants.alignmentLeft);
            PrintString(string.Concat(Constants.chatCmdClear, " | ", Constants.chatCmdCls, " = clear screen"), Constants.alignmentLeft);
            PrintString(string.Concat(Constants.chatCmdClearAll, " | ", Constants.chatCmdClsAll, " = clear screen of all clients + server [Server Only]"), Constants.alignmentLeft);
            PrintString(string.Concat(Constants.chatCmdNotificationBase, " = manage notification settings"), Constants.alignmentLeft);
            PrintString(string.Concat(Constants.chatCmdMatzesMom, " = broadcasts a joke to all connected clients"), Constants.alignmentLeft);
            PrintString(string.Concat(Constants.chatCmdMuteBase, " = manage muted clients [Server Only]"), Constants.alignmentLeft);
            
            if (currInstance == Constants.instanceServer)
            {
                PrintString("Press any key to return to chat!", Constants.alignmentCenter);
                Console.ReadKey();
            }
        }

        static public void PrintCommandHelp(string chatCommand, string currInstance)
        {
            if (currInstance == Constants.instanceServer)
            {
                PrintHeader();
            }

            PrintString("Help Menu for specific commands:", Constants.alignmentCenter);

            if (chatCommand.StartsWith(Constants.chatCmdBcBlack))
            {
                PrintString(string.Concat(Constants.chatCmdBcBlack, " is used to switch to the dark theme"), Constants.alignmentLeft);    
            }
            else if (chatCommand.StartsWith(Constants.chatCmdBcWhite))
            {
                PrintString(string.Concat(Constants.chatCmdBcWhite, " is used to switch to the light theme"), Constants.alignmentLeft);
            }
            else if (chatCommand.StartsWith(Constants.chatCmdBcHaXxOr))
            {
                PrintString(string.Concat(Constants.chatCmdBcHaXxOr, " is used to switch to the hacker theme"), Constants.alignmentLeft);
            }
            else if (chatCommand.StartsWith(Constants.chatCmdClear) || chatCommand.StartsWith(Constants.chatCmdCls))
            {
                PrintString(string.Concat(Constants.chatCmdClear, " | ", Constants.chatCmdCls, " deletes all messages and clears the chat for the invoking user"), Constants.alignmentLeft);
            }
            else if (chatCommand.StartsWith(Constants.chatCmdClearAll) || chatCommand.StartsWith(Constants.chatCmdClsAll))
            {
                PrintString(string.Concat(Constants.chatCmdClearAll, " | ", Constants.chatCmdClsAll, " deletes all messages and clears the chat of all clients + server"), Constants.alignmentLeft);
                PrintString("this command can only be invoked by the server and not by a client", Constants.alignmentLeft);
            }
            else if (chatCommand.StartsWith(Constants.chatCmdNotificationBase))
            {
                PrintString(string.Concat(Constants.chatCmdNotificationBase, " is used to display the current notification settings"), Constants.alignmentLeft);
                PrintString(string.Concat(Constants.chatCmdNotificationSound, " is used to toggle the sound notification 'on' or 'off'"), Constants.alignmentLeft);
                PrintString(string.Concat(Constants.chatCmdNotificationVisual, " is used to toggle the visual notification 'on' or 'off'"), Constants.alignmentLeft);
            }
            else if (chatCommand.StartsWith(Constants.chatCmdMuteBase))
            {
                PrintString(string.Concat(Constants.chatCmdMuteBase, " is used to display all currently muted ip adresses"), Constants.alignmentLeft);
                PrintString(string.Concat(Constants.chatCmdMuteAdd, " is used to add ips (multiple ips can be specified at once) to the mute list"), Constants.alignmentLeft);
                PrintString(string.Concat(Constants.chatCmdMuteRemove, " is used to remove ips (multiple ips can be specified at once) from the mute list"), Constants.alignmentLeft);
            }
            else
            {
                PrintString("No help exists for the specified command!", Constants.alignmentLeft);
            }

            if (currInstance == Constants.instanceServer)
            {
                PrintString("Press any key to return to chat!", Constants.alignmentCenter);
                Console.ReadKey();
            }
        }

        static private void PrintHeader()
        {
            Console.Clear();

            if (Logic.bcHaXxOrActive)
            {
                drawBcHaxxOr();
            }

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
            PrintInput(true);
        }

        static public void PrintInput()
        {
            PrintInput(false);
        }

        static public void PrintInput(bool forcePrint)
        {
            if (!isPrinting || forcePrint)
            {
                int CursorPosY = 0; // Used to determine where the console cursor has to be repositioned to

                // Calculate the new cursor position
                if (Console.CursorLeft == 1 && Logic.enteredMessage.Length % Console.WindowWidth == 0)
                {
                    CursorPosY = Console.CursorTop - (Logic.enteredMessage.Length / Console.WindowWidth);
                }
                else if (Console.CursorLeft == 0 && Logic.enteredMessage.Length % Console.WindowWidth >= 119)
                {
                    CursorPosY = Console.CursorTop - ((Logic.enteredMessage.Length / Console.WindowWidth) + 1);
                }
                else
                {
                    CursorPosY = Console.CursorTop - (Logic.enteredMessage.Length - 1) / Console.WindowWidth;
                }

                // Set cursor position so the current displayed input is overwritten completely
                Console.SetCursorPosition(0, CursorPosY);
                // Overwrite current displayed input with whitespaces
                Console.Write(new string(' ', Logic.enteredMessage.Length + 1));

                // Calculate the new cursor position
                if (Console.CursorLeft == 1 && Logic.enteredMessage.Length / Console.WindowWidth > 0)
                {
                    CursorPosY = Console.CursorTop - (Logic.enteredMessage.Length / Console.WindowWidth);
                }
                else if (Console.CursorLeft == 0 && Logic.enteredMessage.Length % Console.WindowWidth >= 119)
                {
                    CursorPosY = Console.CursorTop - ((Logic.enteredMessage.Length / Console.WindowWidth) + 1);
                }
                else
                {
                    CursorPosY = Console.CursorTop - (Logic.enteredMessage.Length - 1) / Console.WindowWidth;
                }

                // Set cursor position so the current input is printed at the right position
                Console.SetCursorPosition(0, CursorPosY);
                // Print the current input
                Console.Write(Logic.enteredMessage);

                // Done printing => set isPrinting = false; Now the PrintClientInterface can be called again;
                isPrinting = false;
            }
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
            int _cntTotalLines = 0;
            int _maxMsgToDisplay = 0;
            int _actualMsgToDisplay = 0;
            int _lastCalculatedLines = 0;
            // Calculate the maximum count of lines that can display messages
            _maxMsgToDisplay = Console.WindowHeight - 6;

            // This for loop is so that the console does not autoscroll;
            for (int i = Logic.chatObjects.messageData.Count - 1 - Logic.scrollOffset; _cntTotalLines < _maxMsgToDisplay && i >= 0; i--)
            {
                try
                {
                    _lastCalculatedLines = ((Logic.chatObjects.messageData[i].additionalInfo.Length + 2 + Logic.chatObjects.messageData[i].message.Length) / Console.WindowWidth) + 1;
                    _cntTotalLines += _lastCalculatedLines;
                    _actualMsgToDisplay++;
                }
                catch
                {
                    // nothing happens
                }
            }

            if (_lastCalculatedLines > 1)
            {
                _actualMsgToDisplay--;
            }

            for (int i = (Logic.chatObjects.messageData.Count - _actualMsgToDisplay - Logic.scrollOffset < 0 ?
                 0 : Logic.chatObjects.messageData.Count - _actualMsgToDisplay - Logic.scrollOffset);
                 i <= Logic.chatObjects.messageData.Count - 1 - Logic.scrollOffset;
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
                    _outputString = _outputString + " :" + Logic.chatObjects.messageData[i].additionalInfo;
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
                    
                    if (outputString.StartsWith(Constants.serverUsername)) // Display of server username when /matzesmom is invoked
                    {
                        ConsoleColor currForegroundColor = Console.ForegroundColor;
                        
                        Console.ForegroundColor = consoleColorServer;
                        Console.Write(outputString.Substring(0, Constants.serverUsername.Length));
                        Console.ForegroundColor = currForegroundColor;
                        Console.WriteLine(outputString.Substring(Constants.serverUsername.Length));
                    }
                    else if (outputString.IndexOf(Constants.serverUsername, 0) == 11) // Display of server username when server sends a message
                    {
                        ConsoleColor currForegroundColor = Console.ForegroundColor;

                        Console.Write(outputString.Substring(0, 11));
                        Console.ForegroundColor = consoleColorServer;
                        Console.Write(outputString.Substring(11, Constants.serverUsername.Length));
                        Console.ForegroundColor = currForegroundColor;
                        Console.WriteLine(outputString.Substring(11 + Constants.serverUsername.Length));
                    }
                    else
                    {
                        Console.WriteLine(outputString);
                    }
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

        private static void drawBcHaxxOr()
        {
            Random random = new Random();

            // Prepare the background
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            Console.SetCursorPosition(0, 0);

            // Buffer one line of the background and then print it
            for (int y = 0; y < Console.WindowHeight; y++)
            {
                for (int x = 0; x < Console.WindowWidth - 1; x++)
                {
                    
                    if (random.Next(0, 3) < 2)
                    {
                        if (random.Next(0, 100) == 0)
                        {
                            // Add some acctens
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.BackgroundColor = ConsoleColor.DarkGreen;
                            // Print
                            Console.Write(random.Next(0, 2).ToString());
                            // Reset the color 
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.BackgroundColor = ConsoleColor.Black;
                        }
                        else
                        {
                            // Print
                            Console.Write(random.Next(0, 2).ToString());
                        }
                    }
                    else
                    {
                        // Add some accents
                        Console.Write(' ');
                    }
                    if (x == Console.WindowWidth - 2)
                    {
                        Console.WriteLine();
                    }
                }
            }

            // Prepare for normal printing
            Console.ForegroundColor = ConsoleColor.Green;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(0, 0);
        }
    }
}
