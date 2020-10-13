using NapierBankingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NapierBankingApp.Services.Validation
{
    class EmailValidator: MessageValidator
    {
        public static Email ValidateEmail(string header, string body)
        {
            var fields = Parser.ParseBody(body, ",", true);
            var sender = ValidateSender(fields, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", new Dictionary<string, string>());
            #region Sender Validation
            if ((fields.Count == 0))
            {
                throw new Exception("The body must have at least a sender specified.");
            }

            // Check the sender is in the correct format: + followed by 15 numbers
            var senderRegex = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
            if (fields[0] != Regex.Match(fields[0], senderRegex).Value)
            {
                throw new Exception("Invalid sender type. Please make sure the sender has an email format.");
            }
            #endregion

            #region Validate Subject
            if ((fields.Count < 1))
            {
                throw new Exception("The body must have at least a subject specified.");
            }
            if (string.IsNullOrWhiteSpace(fields[1])) { throw new Exception("The subject can not be empty."); }
            if (fields[1].Length > 20) { throw new Exception("The subject length must be less or equal to 20 characters."); }
            var subjectRegex = @"^SIR \d{1,2}/\d{1,2}/\d{4}$";

            if (Regex.IsMatch(fields[1], subjectRegex))
            {
                if ((fields.Count < 3))
                {
                    throw new Exception("The body must contain sort code and nature of incident.");
                }

                // Sort code validation
                var sortCodeRegex = @"\b[0-9]{2}-?[0-9]{2}-?[0-9]{2}\b";
                if (fields[2] != Regex.Match(fields[2], sortCodeRegex).Value)
                {
                    throw new Exception("Incorrect sort code format.");
                }

                // Incident type validation
                List<string> incidentTypes = new List<string>() { "Theft", "Staff Attack", "ATM Theft", "Raid", "Customer Attack", "Staff Abuse", "Bomb Threat", "Terrorism", "Suspicious Incident", "Intelligence", "Cash Loss" };
                if (!incidentTypes.Contains(fields[3]))
                {
                    throw new Exception("Invalid incident type.");
                }

                #region  Validate Text
                var text = "";
                if (fields.Count >= 4)
                {
                    if (fields[4].Length > 1028) { throw new Exception("The text length contains" + fields[4].Length + " characters.\nThe max characters allowed is: 140."); }
                    text = fields[4];
                }
                #endregion

                return new SIR(header, fields[0], fields[1], fields[2], fields[3], text);

            }

            // Else check for SEM validation

            #region  Validate Text
            var textSEM = "";
            if (fields.Count >= 3)
            {
                if (fields[2].Length > 1028) { throw new Exception("The text length contains" + fields[2].Length + " characters.\nThe max characters allowed is: 140."); }
                textSEM = fields[2];
            }
            #endregion
            #endregion
            return new SEM(header, fields[0], fields[1], textSEM);
        }
    }
}
