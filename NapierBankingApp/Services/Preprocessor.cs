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

namespace NapierBankingApp.Services
{
    /// <summary>
    /// A preprocessor class. Allows to preprocess messages and files of messages, serializing them to a JSON file.
    /// All the preprocessed messages are in the message collection.
    /// Retains 3 type of lists: trending, mentions, SIR.
    /// An unloaded message list is retained for the massages which failed to be serialized.
    /// </summary>
    public class Preprocessor
    {
        public Dictionary<string, int> TrendingList { get; private set; }
        public Dictionary<string, int> MentionsList { get; private set; }
        public Dictionary<string, Dictionary<string, int>> SirList { get; private set; }
        public MessageCollection MessageCollection { get; private set; }
        public List<string> UnloadedMessages { get; private set; }
        private Dictionary<string, string> abbreviations;

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

        public void PreprocessMessage(string header, string body)
        {
            #region Header validation
            header = header.ToUpper();
            if (header.Length != 10)
            {
                throw new Exception("The header must have a length of 10 and start with S, E or T");
            }
            #endregion

            if (header[0] == 'S')
            { PreprocessSMS(header, body); }
            else if (header[0] == 'E')
            { PreprocessEmail(header, body); }
            else if (header[0] == 'T')
            { PreprocessTweet(header, body); }
            else
            { throw new Exception("Incorrect header type. Make sure you start your header with: S, E or T."); }

            serializeToJSON();
        }
        private void PreprocessEmail(string header, string body)
        {
            #region Body Validation, Split body into: sender, subject, text
            var bodyArray = body.Split('|');
            if ((bodyArray.Length == 0))
            {
                throw new Exception("The body must have at least a sender specified.");
            }
            if(bodyArray.Length < 2)
            {
                throw new Exception("The body must have a subject specified.");
            }
            #endregion
            Email message = new Email();

            #region Validate sender
            var sender = bodyArray[0];
            if (sender.Length == 0)
            {
                throw new Exception("You must have a sender.");
            }
            sender = Regex.Match(sender, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z").Value;
            if (sender.Length == 0)
            {
                throw new Exception("Invalid sender format. Sender must start with a @ and be followed by 1 to 15 numbers and/or letters.");
            }
            #endregion

            #region Validate Subject
            if(String.IsNullOrWhiteSpace(bodyArray[1]))
            {
                throw new Exception("The subject can not be empty.");
            }
            var subject = bodyArray[1];
            if(Regex.IsMatch(bodyArray[1], @"^SIR\d{1,2}/\d{1,2}/\d{4}$") || Regex.IsMatch(bodyArray[1], @"^SIR \d{1,2}/\d{1,2}/\d{4}$"))
            {
                subject = Regex.Match(bodyArray[1], @"^SIR\d{1,2}/\d{1,2}/\d{4}$").Value;
            }
            #endregion

            // Validate Text
            var text = "";
            if (bodyArray.Length >= 3)
            {
                text = bodyArray[2];
            }
            if (text.Length > 1028)
            {
                throw new Exception("The text length contains" + text.Length + " characters.\nThe max characters allowed is: 140.");
            }
        }
        private void PreprocessTweet(string header, string body)
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
        }
        private void PreprocessSMS(string header, string body)
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
            SMS message = new SMS();

            #region Check text length is max 140 chars
            if (text.Length > 140)
            {
                throw new Exception("The text length contains" + text.Length + " characters.\nThe max characters allowed is: 140.");
            }
            #endregion

            #region Sobstitute abbreviations
            foreach (var entry in abbreviations)
            {
                text = text.Replace(entry.Key, $"{entry.Key} <{entry.Value}>");
            }
            #endregion

            #region Sender Validation
            sender = sender.Replace(" ", "").Replace("  ", "").Replace("_", "").Replace("-", "").Replace("#", "").Replace("*", "");

            // Check the sender is in the correct format: @ followed by 15 numbers
            sender = Regex.Match(sender, @"^\+\d{1,15}$").Value;
            if (sender.Length == 0)
            {
                throw new Exception("Invalid sender format. Sender must start with a + and be followed by 1 to 15 numbers.");
            }
            #endregion

            message.Header = header;
            message.MessageType = "S";
            message.Sender = sender;
            message.Text = text;
            MessageCollection.SMSList.Add(message);
        }
        public void PreprocessFile()
        {
            #region Load File
            var filename = "csvmessages.txt";
            var path = Path.Combine(Environment.CurrentDirectory, filename);
            var lines = File.ReadAllLines(path);
            #endregion

            #region Split Lines and Preprocess
            foreach (var line in lines)
            {
                var messageArray = line.Split(',');
                if (messageArray.Length > 2)
                {
                    for (int counter = 2; counter < messageArray.Length; counter++)
                    {
                        if (!string.IsNullOrWhiteSpace(messageArray[counter]))
                        {
                            messageArray[1] = messageArray[1] + "," + messageArray[counter];
                        }
                    }
                }
                try
                {
                    PreprocessMessage(messageArray[0], messageArray[1]);
                }
                catch (Exception ex)
                {
                    UnloadedMessages.Add(messageArray[0] + " Error: " + ex.Message);
                }

            }
            #endregion
        }
        private void serializeToJSON()
        {
            string jsonString = JsonSerializer.Serialize(MessageCollection);
            File.WriteAllText("myMessages", jsonString);
        }
    }
}

