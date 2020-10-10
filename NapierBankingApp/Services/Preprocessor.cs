using NapierBankingApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Text.Json;
using System.Text.Json.Serialization;
using NapierBankingApp.Services.Factory;
using Microsoft.VisualBasic.FileIO;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Navigation;

namespace NapierBankingApp.Services
{
    public class Preprocessor
    {
        public Dictionary<string, int> TrendingList { get; private set; }
        public Dictionary<string, int> MentionsList { get; private set; }
        public Dictionary<string, Dictionary<string, int>> SirList { get; private set; }
        public MessageCollection MessageCollection { get; private set; }
        public List<string> UnloadedMessages { get; private set; }

        private Dictionary<string, string> abbreviations;

        Database database = new Database("myMessage");

        public Preprocessor()
        {
            TrendingList = new Dictionary<string, int>();
            MentionsList = new Dictionary<string, int>();
            SirList = new Dictionary<string, Dictionary<string, int>>();
            MessageCollection = new MessageCollection();
            abbreviations = new Dictionary<string, string>();
            UnloadedMessages = new List<string>();
            LoadAbbreviations();
        }

        #region Parsers
        /// <summary>
        /// Parses a csv file line by line to get a list of field. Each field is in the form of a string[].
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>A list of fields.</returns>
        public List<string[]> ParseCsvFile(string filename)
        {
            var path = Path.Combine(Environment.CurrentDirectory, filename);
            TextFieldParser parser = new TextFieldParser(path);
            parser.HasFieldsEnclosedInQuotes = true;
            parser.SetDelimiters(",");

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
        public List<string> ParseBody(string body, string delimiter, bool hasQuotes)
        {
            TextFieldParser parser = new TextFieldParser(body);
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
        public void validateHeader(string header)
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
        private Message ValidateSMS(string header, string body)
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
                #region Check text length is max 140 chars
                if (text.Length > 140)
                {
                    throw new Exception("The text length contains" + text.Length + " characters.\nThe max characters allowed is: 140.");
                }
                #endregion
                text = fields[1];
            }
            #endregion

            return new SMS(header, fields[0], text);
        }

        private void ValidateTweet();
        private void ValidateSIR();
        #endregion

        #region Preprocessors
        
        private Message PreprocessTweet(string header, string body)
        {
            #region Body Validation, Split body into: sender, text
            var bodyArray = body.Split('|');
            if ((bodyArray.Length == 0))
            {
                throw new Exception("The body must have at least a sender specified.");
            }

            var sender = bodyArray[0];
            if (sender.Length == 0)
            {
                throw new Exception("You must have a sender.");
            }
            var text = "";
            if (bodyArray.Length >= 2)
            {
                text = bodyArray[1];
            }
            #endregion
            Tweet message = new Tweet();

            #region Validate Sender
            sender = Regex.Match(sender, @"^\@[a-zA-Z0-9_]{1,15}$").Value;
            if (sender.Length == 0)
            {
                throw new Exception("Invalid sender format. Sender must start with a @ and be followed by 1 to 15 numbers and/or letters.");
            }
            #endregion

            #region Validate Text
            if (text.Length > 140)
            {
                throw new Exception("The text length contains" + text.Length + " characters.\nThe max characters allowed is: 140.");
            }
            #endregion

            #region Abbreviations replacement
            foreach (var entry in abbreviations)
            {
                text = text.Replace(entry.Key, $"{entry.Key} <{entry.Value}>");
            }
            #endregion

            #region Add to Mention list
            foreach (Match match in Regex.Matches(text, @"\B\@\w{1,15}\b"))
            {
                if (MentionsList.ContainsKey(match.ToString()))
                {
                    MentionsList[match.ToString()] += 1;
                }
                else
                {
                    MentionsList.Add(match.ToString(), 1);
                }
            }
            #endregion

            #region Add to Trending list
            foreach (Match match in Regex.Matches(text, @"\B\#\w{1,15}\b"))
            {
                if (TrendingList.ContainsKey(match.ToString()))
                {
                    TrendingList[match.ToString()] += 1;
                }
                else
                {
                    TrendingList.Add(match.ToString(), 1);
                }
            }
            #endregion

            message.Header = header;
            message.MessageType = "T";
            message.Sender = sender;
            message.Text = text;
            MessageCollection.TweetList.Add(message);

            return new Tweet();
        }
        private Message PreprocessEmail(string header, string body)
        {
            #region Body Validation, Split body into: sender, subject, text
            var bodyArray = body.Split('|');
            if ((bodyArray.Length == 0))
            {
                throw new Exception("The body must have at least a sender specified.");
            }
            if (bodyArray.Length < 2)
            {
                throw new Exception("The body must have a subject specified.");
            }
            #endregion
            Email message = new Email();

            #region Validate sender
            if (bodyArray[0].Length == 0) { throw new Exception("You must have a sender."); }
            var emailRegex = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
            if (Regex.IsMatch(bodyArray[0], emailRegex))
            {
                message.Sender = Regex.Match(bodyArray[0], emailRegex).Value;
            }
            else { throw new Exception("Invalid sender format"); }
            if (bodyArray[0].Length == 0)
            {

            }
            #endregion

            #region Validate Subject
            if (string.IsNullOrWhiteSpace(bodyArray[1])) { throw new Exception("The subject can not be empty."); }
            if (Regex.IsMatch(bodyArray[1], @"^SIR\d{1,2}/\d{1,2}/\d{4}$") || Regex.IsMatch(bodyArray[1], @"^SIR \d{1,2}/\d{1,2}/\d{4}$"))
            {
                message.Subject = Regex.Match(bodyArray[1], @"^SIR\d{1,2}/\d{1,2}/\d{4}$").Value;
            }
            #endregion

            #region  Validate Text
            var text = "";
            if (bodyArray.Length >= 3)
            {
                if (bodyArray[2].Length > 1028) { throw new Exception("The text length contains" + bodyArray[2].Length + " characters.\nThe max characters allowed is: 140."); }
                // sobstitue trings
                text = bodyArray[2];
            }


            #endregion
            return new Email();
        }
        private Message PreprocessSMS(string header, string body)
        {
            Message message = ValidateSMS(header, body);
            message.Text = SobstituteAbbreviations(message.Text);
            return message;
        }
        public void PreprocessMessage(string header, string body)
        {
            Message message;
            validateHeader(header);
            switch (header[0])
            {
                case 'S':
                    message = PreprocessSMS(header, body);
                    break;
                case 'E':
                    break;
                case 'T':
                    break;
                default:
                    throw new Exception("Incorrect message type");
            }

            { ); }


            database.serializeToJSON(MessageCollection);
        }
        public void PreprocessFile()
        {
            List<string[]> lines = ParseCsvFile("rawmessages.txt");
            foreach (var line in lines)
            {
                validateHeader(line[0]);
                switch (line[0][0])
                {
                    case 'S':
                        // Validate sms
                        // Preprocess sms
                        // Serialize sms
                        break;
                    case 'E':
                        // Validate 
                        break;
                    case 'T':
                        break;
                    default:
                        throw new Exception("Message type not recognised.");
                }


                if (line[0][0] == 'S')
                {

                }

                foreach (var field in line)
                {

                    try
                    {
                        PreprocessMessage(messageArray[0], messageArray[1]);
                    }
                    catch (Exception ex)
                    {
                        UnloadedMessages.Add(messageArray[0] + " Error: " + ex.Message);
                    }
                }
            }

        }
        #endregion


        private void LoadAbbreviations()
        {
            // Loading abbreviations file
            var filename = "textwords.csv";
            var path = Path.Combine(Environment.CurrentDirectory, filename);
            var lines = File.ReadAllLines(path);

            foreach (string line in lines)
            {
                var abbreviation = line.Split(',');
                abbreviations.Add(abbreviation[0], abbreviation[1]);
            }
        }
        public string SobstituteAbbreviations(string text)
        {
            foreach (var entry in abbreviations)
            {
                text = text.Replace(entry.Key, $"{entry.Key} <{entry.Value}>");
            }
            return text;
        }

       
       

    }
}