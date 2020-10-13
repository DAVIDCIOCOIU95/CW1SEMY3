using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace NapierBankingApp.Models
{
    public class SMS : Message
    {
        private readonly string _messageType;
        private string _header;
        private string _sender;
        private string _text;

        public SMS()
        {
            _messageType = "S";
        }

        public SMS(string header, string sender, string text)
        {
            _messageType = "S";
            _header = header;
            _sender = sender;
            _text = text;
        }

        public override string MessageType { get { return _messageType; } }

        public override string Header { get { return _header; } set { _header = value; } }
        public override string Sender { get { return _sender; } set { _sender = value; } }
        public override string Text { get { return _text; } set { _text = value; } }

        public override string ToString()
        {
            return "MessageType:" + MessageType + "\nHeader: " + Header + "\nSender: " + Sender + "\nText: " + Text;
        }
    }
}