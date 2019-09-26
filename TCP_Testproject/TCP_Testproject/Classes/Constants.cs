using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCP_Testproject.Classes
{
    class Constants
    {
        // Delimiter constants
        public const string delimUsername = ":#USERNAME:";
        public const string delimMsgData = ":#MSGDATA:";

        // Name constants
        public const string serverUsername = "<Server>";

        // Program state constants
        public enum ProgramState
        {
            initializing,
            initialized,
            connecting,
            connected,
            connectionerror,
            communicating,
            exit
        };

        // Text alignment constants
        public const string alignmentLeft = "alignLeft";
        public const string alignmentCenter = "alignCenter";
        public const string alignmentRight = "alignRight";

        // Chat command constants
        public const string chatCmdHelp = "/help";
        public const string chatCmdCommandHelp = "?";
        public const string chatCmdBcBlack = "/bcblack";
        public const string chatCmdBcWhite = "/bcwhite";
        public const string chatCmdClear = "/clear";
        public const string chatCmdCls = "/cls";
        public const string chatCmdClearAll = "/clearall";
        public const string chatCmdClsAll = "/clsall";
        public const string chatCmdNotificationBase = "/notification";
        public const string chatCmdNotificationSound = "-s";
        public const string chatCmdNotificationVisual = "-v";
        
    }
}
