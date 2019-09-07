using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCP_Testproject.Classes
{
    class Constants
    {
        // Text alignment constants
        public const string alignmentLeft = "alignLeft";
        public const string alignmentCenter = "alignCenter";
        public const string alignmentRight = "alignRight";

        // Program state constants
        public enum ProgramState
        {
            initializing,
            initialized,
            connecting,
            connected,
            exit
        };
    }
}
