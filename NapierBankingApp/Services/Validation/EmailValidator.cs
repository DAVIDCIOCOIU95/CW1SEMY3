using NapierBankingApp.Models;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace NapierBankingApp.Services.Validation
{
    public class EmailValidator : MessageValidator
    {
        public static Email ValidateEmail(string header, string body)
        {
            var fields = Parser.ParseBody(body, ",", true);
            var sender = ValidateSender(fields, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z");
            var type = ValidateSubject(fields, 1, @"^SIR \d{1,2}/\d{1,2}/\d{4}$", @"^[a-zA-Z0-9_]{0,20}");
            // @"^\@[a-zA-Z0-9_]{1,15}$"

            if (type == "SIR")
            {
                return new SIR(header, fields[0], fields[1], ValidateSortCode(fields, 2, @"\b[0-9]{2}-?[0-9]{2}-?[0-9]{2}\b"), ValidateIncidentType(fields, 3), ValidateText(fields, 4, 1028));
            }
            else if (type == "SEM")
            {
                return new SEM(header, fields[0], fields[1], ValidateText(fields, 2, 1028));
            }
            else
            {
                throw new Exception("Can't validate the email, the sender has an invalid type.");
            }
        }

        /// <summary>
        /// Validates the subject of an email.
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="subjectRegex"></param>
        /// <returns>The type of the email.</returns>
        protected static string ValidateSubject(List<string> fields, int subjectIndex,string subjectRegex1, string subjectRegex2)
        {
            if ((fields.Count < (subjectIndex + 1)))
            {
                throw new Exception("The body must have a subject specified.");
            }
            if (string.IsNullOrWhiteSpace(fields[subjectIndex])) { throw new Exception("The subject can not be empty."); }
            if (fields[subjectIndex].Length > 20) { throw new Exception("The subject length must be less or equal to 20 characters."); }
            if (Regex.IsMatch(fields[subjectIndex], subjectRegex1))
            {
                return "SIR";
            }
            else if (Regex.IsMatch(fields[subjectIndex], subjectRegex2))
            {
                return "SEM";
            }
            else
            {
                throw new Exception("Invalid subject.");
            }
        }

        protected static string ValidateSortCode(List<string> fields, int sortCodeIndex, string sortCodeRegex)
        {
            if (fields.Count < sortCodeIndex + 1 || string.IsNullOrWhiteSpace(fields[sortCodeIndex]))
            {
                throw new Exception("The body must have a sort code specified.");
            }
            if (string.IsNullOrEmpty(fields[sortCodeIndex]) || fields[sortCodeIndex] != Regex.Match(fields[sortCodeIndex], sortCodeRegex).Value)
            {
                throw new Exception("Incorrect sort code format.");
            }
            return fields[sortCodeIndex];
        }

        protected static string ValidateIncidentType(List<string> fields, int incidentIndex)
        {
            if ((fields.Count < (incidentIndex + 1)))
            {
                throw new Exception("The body must have an incident type specified.");
            }
            List<string> incidentTypes = new List<string>() { "Theft", "Staff Attack", "ATM Theft", "Raid", "Customer Attack", "Staff Abuse", "Bomb Threat", "Terrorism", "Suspicious Incident", "Intelligence", "Cash Loss" };
            if (!incidentTypes.Contains(fields[incidentIndex]))
            {
                throw new Exception("Invalid incident type.");
            }
            
            return fields[incidentIndex];
        }
    }
}
