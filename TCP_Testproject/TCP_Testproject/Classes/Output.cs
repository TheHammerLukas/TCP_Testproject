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

            }
            else if (Logic.chatState == Constants.ProgramState.connecting)
            {
                PrintClientCreate();
            }
            else if (Logic.chatState == Constants.ProgramState.connected)
            {
                PrintClientInterface();
            }
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

        static private void PrintInitInterface()
        {
            PrintString("Host server | Connect as client [H|C]", Constants.alignmentCenter);
        }

        static private void PrintClientCreate()
        {
            PrintString("Enter your desired username:", Constants.alignmentCenter);
        }

        static private void PrintClientInterface()
        {
            string _outputString = "";
            string _textAlignment = "";

            for (int i = 0; i < Logic.chatObjects.messageData.Count; i++)
            {
                _outputString = Logic.chatObjects.messageData[i].message;
                _textAlignment = Logic.chatObjects.messageData[i].alignment;

                // Only display the username if it is a message that has been received from the server
                if (_textAlignment == Constants.alignmentLeft)
                {
                    _outputString = Logic.chatObjects.messageData[i].username + ": " + _outputString;
                }
                <
                PrintString(_outputString, _textAlignment);
            }
            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.Write("-");
            }
            PrintString("Type your message and press enter to send:", Constants.alignmentLeft);
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
                    Console.SetCursorPosition(((Console.WindowWidth - outputString.Length) / 2) - 1, Console.CursorTop);
                    Console.WriteLine(outputString);
                    break;
                case Constants.alignmentRight:
                    // Do not go below 0 or an error will occur; Cursorleft is the minimun value for the console bounds; Otherwise clients will crash
                    if ((Console.WindowWidth - outputString.Length) -1 < 0)
                    {
                        Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);
                    }
                    else
                    {
                        Console.SetCursorPosition((Console.WindowWidth - outputString.Length) - 1, Console.CursorTop);
                    }
                    Console.WriteLine(outputString);
                    break;
            }
        }
    }
}
