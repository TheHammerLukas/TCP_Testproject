using System.Collections.Generic;
using System.Net.Sockets;

namespace TCP_Testproject.Classes
{
    class Objects
    {
        public List<Message> messageData = new List<Message>();
        public TcpListener server = null;
        public List<TcpClient> clientList = new List<TcpClient>();
        public TcpClient client = new TcpClient();
        public string clientName = "<NoNameSpecified>";
    }

    class Message
    {
        public string additionalInfo { get; set; }
        public string message { get; set; }
        public string alignment { get; set; }

        public Message(string additionalInfo, string message, string alignment)
        {
            this.additionalInfo = additionalInfo;
            this.message = message;
            this.alignment = alignment;
        }
    }
}
