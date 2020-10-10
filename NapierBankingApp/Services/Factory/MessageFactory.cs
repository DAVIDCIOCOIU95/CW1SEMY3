using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NapierBankingApp.Models;

namespace NapierBankingApp.Services.Factory
{
    abstract class MessageFactory
    {
        public abstract Message GetMessage();
    }
}
