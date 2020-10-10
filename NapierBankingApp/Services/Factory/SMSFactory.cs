using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NapierBankingApp.Models;

namespace NapierBankingApp.Services.Factory
{
    class SMSFactory : MessageFactory
    {
        private string _header;
        private string _sender;
        private string _text;

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        /// <summary>
        /// Creates an SMSFactory only if the validation has been successful
        /// </summary>
        /// <param name="header"></param>
        /// <param name="sender"></param>
        /// <param name="text"></param>
        public SMSFactory(string header, string sender, string text)
        {
            _header = header;
            _sender = sender;
            _text = text;
        }

        public override Message GetMessage()
        {
            return new SMS(_header, _sender, _text);
        }
    }
}
