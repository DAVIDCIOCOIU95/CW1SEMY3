using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NapierBankingApp.Models
{
	public abstract class Email : Message
	{
		public abstract string Subject { get; set; }
		public abstract string EmailType { get; set; }
	}
}

