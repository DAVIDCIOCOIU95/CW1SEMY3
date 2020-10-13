using Microsoft.VisualBasic.FileIO;
using NapierBankingApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NapierBankingApp.Services.Validation
{
    /// <summary>
    /// Takes in a header and a body of a message. Detects and validates the message. Returns a message type according to the business rules.
    /// </summary>
    class Validator
    {
        public List<string> UnloadedMessages { get; private set; }
        public Validator()
        {
            UnloadedMessages = new List<string>();
        }

        #region Parsers
        /// <summary>
        /// Parses a structured csv file line by line getting a list of message entities. Each message entity is in the form of string[], where index 0 is the header and index 1 is the body.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>A list of fields.</returns>
        private List<string[]> ParseCsvFile(string filename)
        {
            var path = Path.Combine(Environment.CurrentDirectory, filename);
            TextFieldParser parser = new TextFieldParser(path);
            parser.HasFieldsEnclosedInQuotes = false;
            parser.SetDelimiters("|");

            List<string[]> fields = new List<string[]>();
            while (!parser.EndOfData)
            {
                fields.Add(parser.ReadFields());
            }
            parser.Close();
            return fields;
        }

        /// <summary>
        /// Separates a block of text into fields. A delimiter can be set.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="delimiter"></param>
        /// <param name="hasQuotes"></param>
        /// <returns>A list of strings containing all the fields.</returns>
        private List<string> ParseBody(string body, string delimiter, bool hasQuotes)
        {
            StringReader sr = new StringReader(body);
            TextFieldParser parser = new TextFieldParser(sr);
            parser.HasFieldsEnclosedInQuotes = hasQuotes;
            parser.SetDelimiters(delimiter);

            List<string> fields = new List<string>();
            while (!parser.EndOfData)
            {
                var line = parser.ReadFields();
                foreach (var field in line)
                {
                    fields.Add(field);
                }
            }
            parser.Close();
            return fields;
        }
        #endregion

        #region Validators
        private void ValidateHeader(string header)
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
        private SMS ValidateSMS(string header, string body)
        {
            var fields = ParseBody(body, ",", true);
            #region Sender Validation
            if ((fields.Count == 0))
            {
                throw new Exception("The body must have at least a sender specified.");
            }
            // Allow for special chars in the sender and remove them
            fields[0] = fields[0].Replace(" ", "").Replace("  ", "").Replace("_", "").Replace("-", "").Replace("#", "").Replace("*", "");

            // Check the sender is in the correct format: + followed by 15 numbers
            var senderRegex = @"^\+\d{1,15}$";
            if (fields[0] != Regex.Match(fields[0], senderRegex).Value)
            {
                throw new Exception("Invalid sender type. Please make sure the sender starts with + and is followed by maximum 15 numbers.");
            }
            #endregion
            #region Text Validation
            var text = "";
            if (fields.Count > 1)
            {
                text = fields[1];
                if (text.Length > 140)
                {
                    throw new Exception("The text length contains" + text.Length + " characters.\nThe max characters allowed is: 140.");
                }
            }
            #endregion
            return new SMS(header, fields[0], text);
        }
        private Tweet ValidateTweet(string header, string body)
        {
            var fields = ParseBody(body, ",", true);
            #region Sender Validation
            if ((fields.Count == 0))
            {
                throw new Exception("The body must have at least a sender specified.");
            }
            // Check the sender is in the correct format: @ followed by 15 numbers
            var senderRegex = @"^\@[a-zA-Z0-9_]{1,15}$";
            if (fields[0] != Regex.Match(fields[0], senderRegex).Value)
            {
                throw new Exception("Invalid sender type. Please make sure the sender starts with + and is followed by maximum 15 numbers.");
            }
            #endregion
            #region Text Validation
            var text = "";
            if (fields.Count > 1)
            {
                text = fields[1];
                if (text.Length > 140)
                {
                    throw new Exception("The text length contains" + text.Length + " characters.\nThe max characters allowed is: 140.");
                }
            }
            #endregion
            return new Tweet(header, fields[0], text);
        }
        private Email ValidateEmail(string header, string body)
        {
            var fields = ParseBody(body, ",", true);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="header"></param>
        /// <param name="body"></param>
        /// <returns>A validated message.</returns>
        public Message ValidateMessage(string header, string body)
        {
            ValidateHeader(header);
            switch (header[0])
            {
                case 'S':
                    return ValidateSMS(header, body);
                case 'E':
                    return ValidateEmail(header, body);
                case 'T':
                    return ValidateTweet(header, body);
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
            var fields = ParseCsvFile("rawmessages.txt");
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
                throw new Exception("No valid message identified in file");
            }
            return validMessages;
        }
        #endregion
    }
}
