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
            PrintInput();
        }

        static public void PrintInput()
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
