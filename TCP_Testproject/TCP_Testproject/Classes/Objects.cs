using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCP_Testproject.Classes
{
    class Objects
    {
        public List<Message> messageData = new List<Message>();
        public TcpListener server = null;
        public TcpClient client = null;
    }

    class Message
    {
        public string message { get; set; }
        public string alignment { get; set; }

        public Message(string message, string alignment)
        {
            this.message = message;
            this.alignment = alignment;
        }
    }
}
