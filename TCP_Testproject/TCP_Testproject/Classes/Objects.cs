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
        public static List<string> matzesMomJokes = new List<string>();

        public Objects()
        {
            InitMatzesMomJokes();
        }

        private static void InitMatzesMomJokes()
        {
            matzesMomJokes.Add("Was ist 200 km/h schnell und rollt über die Autobahn? Matzes Mom mit nem McDonalds Gutschein.");
            matzesMomJokes.Add("Matzes Mom ist so fett, ihr Schulfoto aus der ersten Klasse druckt immer noch.");
            matzesMomJokes.Add("Matzes Mom ist so fett, wenn sie leuchten würde, wäre die Sonne arbeitslos.");
            matzesMomJokes.Add("Matzes Mom ist sogar in Minecraft rund.");
            matzesMomJokes.Add("Matzes Mom ist so hässlich wenn sie einen Bumerang wirft weigert er sich zurück zukommen.");
            matzesMomJokes.Add("Matzes Mom ist so armselig, ihr ganzes Leben ist ein riesen Witz.");
            matzesMomJokes.Add("Matzes Mom ist so haarig, wenn sie mit ihrem Hund spazieren geht, wird sie zuerst gestreichelt.");
            matzesMomJokes.Add("Matzes Mom sitzt in einer Wanne voller Fanta, weil Sie auch mal aus ner Limo winken will.");
            matzesMomJokes.Add("Auf Matzes Mom ist mehr Verkehr als auf dem Hauptbahnhof.");
            matzesMomJokes.Add("Sitzen drei Nutten an der Bar, sagt die eine: \"In mich passen drei Schwänze auf einmal.\"\nSagt die Zweite: \"In mich passen fünf Schwänze auf einmal.\"\nSagt Matzes Mom: \"Kann mir mal jemand vom Hocker helfen?\"");
        }
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
