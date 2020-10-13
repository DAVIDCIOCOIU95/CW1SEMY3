using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NapierBankingApp.Models
{
    public class SEM : Email
    {
        private readonly string _messageType;
        private string _header;
        private string _sender;
        private string _text;
        private string _subject;
        private string _emailType;

        public SEM()
        {
            _messageType = "E";
            _emailType = "SEM";
        }

        public SEM(string header, string sender, string subject,string text)
        {
            _messageType = "E";
            _header = header;
            _sender = sender;
            _text = text;
            _subject = subject;
            _emailType = "SEM";
        }

        public override string MessageType { get { return _messageType; } }

        public override string Header { get { return _header; } set { _header = value; } }
        public override string Sender { get { return _sender; } set { _sender = value; } }
        public override string Text { get { return _text; } set { _text = value; } }
        public override string Subject { get { return _subject; } set { _subject = value; } }
        public override string EmailType { get { return _emailType; } set { _emailType = value; } }

        public override string ToString()
        {
            return "MessageType:" + MessageType + "\nEmail Type: " + EmailType + "\nHeader: " + Header + "\nSender: " + Sender + "\nSubject: " + Subject + "\nText: " + Text;
        }
    }
}