using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace NapierBankingApp.Models
{
	public abstract class Message
	{
		public abstract string MessageType { get; }
		public abstract string Header { get; set; }
		public abstract string Sender { get; set; }
		public abstract string Text { get; set; }
	}

	
}
