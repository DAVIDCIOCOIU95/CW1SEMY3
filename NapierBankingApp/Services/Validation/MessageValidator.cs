using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NapierBankingApp.Services.Validation
{
    public abstract class MessageValidator
    {
        public static string ValidateHeader(string header)
        {
            header = header.ToUpper();
            if (header.Length != 10)
            {
                throw new Exception("The header must have a length of 10.");
            }
            if (header[0].ToString() != Regex.Match(header, @"[SET]").Value)
            {
                throw new Exception("The header must start with S, E or T");
            }
            if (header != Regex.Match(header, @"[SET]\d+").Value)
            {
                throw new Exception("The header type must be followed by only numeric characters.");
            }
            return header;
        }
        public static string ValidateSender(List<string> fields, string senderRegex, Dictionary<string, string> specialCharacters = null)
        {
            if (specialCharacters == null)
                specialCharacters = new Dictionary<string, string>();

            // Replace special chars to clean the sender
            foreach (var spChar in specialCharacters)
            {
                fields[0] = fields[0].Replace(spChar.Key, spChar.Value);
            }
            // Match regex
            if (string.IsNullOrEmpty(fields[0]) || fields[0] != Regex.Match(fields[0], senderRegex).Value)
            {
                
                throw new Exception("The sender format is incorrect, please enter a valid sender.");
            }
            return fields[0];
        }
        public static string ValidateText(List<string> fields, int textPosition, int maxChars) 
        {
            var text = "";
            if (fields.Count > textPosition)
            {
                text = fields[textPosition];
                if (text.Length > maxChars)
                {
                    throw new Exception("The text length contains " + text.Length + " characters.\nThe max characters allowed is: " + maxChars);
                }
            }
            return text;
        }
    }
}
