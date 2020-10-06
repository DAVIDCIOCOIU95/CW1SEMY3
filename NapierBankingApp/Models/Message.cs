using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace NapierBankingApp.Models
{
	public class Message
	{
		public string Header { get; set; }
		public string Sender { get; set; }
		public string MessageType { get; set; }
		public string Text { get; set; }
		

		public override string ToString()
		{
			return "Header: " + Header + "\nMessageType: " + MessageType  + "\nSender: " + Sender + "\nText: " + Text;
		}
	}

	
}
