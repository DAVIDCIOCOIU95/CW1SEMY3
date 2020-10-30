using NapierBankingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NapierBankingApp.Services
{
    public class MessageCollection
    {
        public Dictionary<string, SMS> SMSList { get; set; }
        public Dictionary<string, Tweet> TweetList { get; set; }
        
        public Dictionary<string, SIR> SIRList { get; set; }
        public Dictionary<string, SEM> SEMList { get; set; }


        public MessageCollection()
        {
            SMSList = new Dictionary<string,SMS>();
            TweetList = new Dictionary<string, Tweet>();
            SIRList = new Dictionary<string, SIR>();
            SEMList = new Dictionary<string, SEM>();
        }

    }
}
