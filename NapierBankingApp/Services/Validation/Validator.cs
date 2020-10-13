using NapierBankingApp.Models;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NapierBankingApp.Services.Validation
{
    /// <summary>
    /// Takes in a header and a body of a message. Detects and validates the message. Returns a message type according to the business rules.
    /// </summary>
    class Validator : MessageValidator
    {
        public List<string> UnloadedMessages { get; private set; }
        public Validator()
        {
            UnloadedMessages = new List<string>();
        }

        public Message ValidateMessage(string header, string body)
        {
            ValidateHeader(header);
            switch (header[0])
            {
                case 'S':
                    return SMSValidator.ValidateSMS(header, body);
                case 'E':
                    return EmailValidator.ValidateEmail(header, body);
                case 'T':
                    return TweetValidator.ValidateTweet(header, body);
                default:
                    throw new Exception("Invalid Message Type.");
            }

        }

        /// <summary>
        /// Validates a file containing messages.
        /// </summary>
        /// <param name="fields"></param>
        /// <returns>A list of valid messages.</returns>
        public List<Message> ValidateFile()
        {
            var fields = Parser.ParseCsvFile("rawmessages.txt", "|");
            List<Message> validMessages = new List<Message>();
            UnloadedMessages.Clear();
            foreach (var field in fields)
            {
                if (field.Length != 2)
                {
                    UnloadedMessages.Add("Error for: " + field.ToString() + "\nError type: please make sure your input is of the type <header>|<body>");
                }
                else
                {
                    try
                    {
                        validMessages.Add(ValidateMessage(field[0], field[1]));
                    }
                    catch (Exception ex)
                    {
                        UnloadedMessages.Add("Error for: " + field[0].ToString() + "\nError type: " + ex.Message);
                    }
                }
            }
            if (validMessages.Count == 0)
            {
                throw new Exception("No valid messages identified in the file");
            }
            return validMessages;
        }
    }
}
