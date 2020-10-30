using NapierBankingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NapierBankingApp.Services.Validation
{
    class TweetValidator: MessageValidator
    {
        public static Tweet ValidateTweet(string header, string body)
        {
            var fields = Parser.ParseBody(body, ",", true);
            return new Tweet(header, ValidateSender(fields, @"^\@[a-zA-Z0-9_]{1,15}$"), ValidateText(fields, 1, 140));
        }
    }
}

