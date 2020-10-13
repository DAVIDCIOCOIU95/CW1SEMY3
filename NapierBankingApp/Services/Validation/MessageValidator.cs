using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NapierBankingApp.Services.Validation
{
    abstract class MessageValidator
    {
        protected static void ValidateHeader(string header)
        {
            if (header.Length != 10)
            {
                header = header.ToUpper();
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
        }
        protected static string ValidateSender(List<string> fields, string senderRegex, Dictionary<string, string> specialCharacters = null)
        {
            if (specialCharacters == null)
                specialCharacters = new Dictionary<string, string>();

            if ((fields.Count == 0))
            {
                throw new Exception("The body must have at least a sender specified.");
            }
            foreach (var spChar in specialCharacters)
            {
                fields[0] = fields[0].Replace(spChar.Key, spChar.Value);
            }
            if (fields[0] != Regex.Match(fields[0], senderRegex).Value)
            {
                throw new Exception("Invalid sender type.");
            }
            return fields[0];
        }
        protected static string ValidateText(List<string> fields, int textPosition, int maxChars) 
        {
            var text = "";
            if (fields.Count > textPosition)
            {
                text = fields[textPosition];
                if (text.Length > maxChars)
                {
                    throw new Exception("The text length contains" + text.Length + " characters.\nThe max characters allowed is: 140.");
                }
            }
            return text;
        }
    }
}
