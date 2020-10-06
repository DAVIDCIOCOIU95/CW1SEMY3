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
        public List<SMS> SMSList { get; set; }
        public List<Tweet> TweetList { get; set; }
        
        public List<SIR> SIRList { get; set; }
        public List<SEM> SEMList { get; set; }


        public MessageCollection()
        {
            SMSList = new List<SMS>();
            TweetList = new List<Tweet>();
            SIRList = new List<SIR>();
            SEMList = new List<SEM>();
        }

    }
}
