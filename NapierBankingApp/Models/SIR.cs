using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NapierBankingApp.Models
{
    public class SIR : Email
    {
        public string SortCode { get; set; }
        public string IncidentType { get; set; }

        public override string ToString()
        {
            return "Header: " + Header + "\nMessageType:" + MessageType + "\nSender: " + Sender + EmailType + "\nSorCode: " + SortCode + "\nIncidentTypet: " + IncidentType + "\nText: " + Text;
        }
    }
}