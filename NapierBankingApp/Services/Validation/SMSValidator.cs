using NapierBankingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NapierBankingApp.Services.Validation
{
    class SMSValidator : MessageValidator
    {
        public static SMS ValidateSMS(string header, string body)
        {
            var fields = Parser.ParseBody(body, ",", true);
            return new SMS(header, ValidateSender(fields, @"^\+\d{1,15}$", new Dictionary<string, string>() { [" "] = "", ["    "] = "", ["_"] = "", ["-"] = "", ["#"] = "", ["*"] = "" }), ValidateText(fields, 1, 140));
        }
    }
}
