using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NapierBankingApp.Models;

namespace NapierBankingApp.Services.MessageFactory
{
    class SMSFactory: MessageFactory
    {
        private string _header;
        private string _sender;
        private string _text;

        /// <summary>
        /// Creates an SMSFactory only if the validation has been successful
        /// </summary>
        /// <param name="header"></param>
        /// <param name="sender"></param>
        /// <param name="text"></param>
        public SMSFactory(string header, string body)
        {
            // Call the validation methods
            // If all pass then assign the fields
            // else throw an error

            // assign fields

        }

        public override Message GetMessage()
        {
            return new SMS(_header, _sender, _text);
        }
    }
}
