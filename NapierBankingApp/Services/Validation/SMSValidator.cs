using NapierBankingApp.Models;
using System.Collections.Generic;

namespace NapierBankingApp.Services.Validation
{
    class SMSValidator : MessageValidator
    {
        public static SMS ValidateSMS(string header, string body)
        {
            var fields = Parser.ParseBody(body, ",", true);
            return new SMS(header, ValidateSender(fields, @"^[\+]\d{7,15}$", new Dictionary<string, string>() { [" "] = "", ["    "] = "", ["_"] = "", ["-"] = "", ["#"] = "", ["*"] = "" }), ValidateText(fields, 1, 140));
        }
    }
}
