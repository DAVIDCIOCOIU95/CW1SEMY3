using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NapierBankingApp.Models;

namespace NapierBankingApp.Services.MessageFactory
{
    abstract class MessageFactory
    {
        public abstract Message GetMessage();
    }
}
