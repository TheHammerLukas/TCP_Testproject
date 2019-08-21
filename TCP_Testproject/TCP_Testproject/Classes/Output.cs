using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCP_Testproject.Classes
{
    class Output
    {
        static public void PrintScreen()
        {
            string _outputString = "";
            string _textAlignment = "";

            PrintHeader();

            if (Logic.chatState == Constants.ProgramState.connected)
            {
                for (int i = 0; i < Logic.chatObjects.messageData.Count; i++)
                {
                    _outputString = Logic.chatObjects.messageData[i].message;
                    _textAlignment = Logic.chatObjects.messageData[i].alignment;

                    PrintString(_outputString, _textAlignment);
                }
            }
            else if (Logic.chatState == Constants.ProgramState.initializing)
            {
                PrintString("Host server | Connect as Client [H|C]", Constants.alignmentCenter);
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

        static private void PrintString(string outputString, string textAlignment)
        {
            switch (textAlignment)
            {
                case Constants.alignmentLeft:

                    break;
                case Constants.alignmentCenter:
                    Console.SetCursorPosition((Console.WindowWidth - outputString.Length) / 2, Console.CursorTop);
                    Console.WriteLine(outputString);
                    break;
                case Constants.alignmentRight:

                    break;
            }
        }
    }
}
