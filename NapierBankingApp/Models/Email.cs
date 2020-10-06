using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NapierBankingApp.Models
{
	public class Email : Message
	{
		public string Subject { get; set; }
		public string EmailType { get; set; }

		public override string ToString()
		{
			return "Header: " + Header + "\nMessageType:" + MessageType  + "\nSubject: " + Subject + "\nEmailType: " + EmailType + "\nText: " + Text;
		}
	}
}

