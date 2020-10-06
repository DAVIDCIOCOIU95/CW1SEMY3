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
            header = header.ToUpper();
            // Validate header
            if (header.Length != 10)
            {
                throw new Exception("The header can not be empty, must have length = 10 and must start with one of the following characters: S,E,T.");
            }

            #region Body Validation: splits body into sender and text
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

            #region SMS
            if (header[0] == 'S')
            {
                SMS message = new SMS();
                message.MessageType = "S";
                if (text.Length > 140)
                {
                    throw new Exception("The text length contains" + text.Length + " characters.\nThe max characters allowed is: 140.");
                }
                // Sobstitute abbreviations
                foreach (var entry in abbreviations)
                {
                    text = text.Replace(entry.Key, $"{entry.Key} <{entry.Value}>");
                }
                message.Text = text;

                // Clean the number
                // Eliminate any extra char
                sender = sender.Replace(" ", "").Replace("  ", "").Replace("_", "").Replace("-", "").Replace("#", "").Replace("*", "");
                sender = Regex.Match(sender, @"^\+\d{1,15}$").Value;
                if (sender.Length == 0)
                {
                    throw new Exception("Invalid sender format. Sender must start with a + and be followed by 1 to 15 numbers.");
                }
                // Load the message to messageCollection
                message.Header = header;
                message.Sender = sender;
                MessageCollection.SMSList.Add(message);
            }
            #endregion

            #region Email
            else if (header[0] == 'E')
            {

            }
            #endregion

            #region Tweet
            else if (header[0] == 'T')
            {
                Tweet message = new Tweet();
                message.MessageType = "T";

                #region Sender
                sender = Regex.Match(sender, @"^\@[a-zA-Z0-9_]{1,15}$").Value;
                if (sender.Length == 0)
                {
                    throw new Exception("Invalid sender format. Sender must start with a @ and be followed by 1 to 15 numbers and/or letters.");
                }
                #endregion

                #region Body
                if (text.Length > 140)
                {
                    throw new Exception("The text length contains" + text.Length + " characters.\nThe max characters allowed is: 140.");
                }
                // Abbreviations replacement
                foreach (var entry in abbreviations)
                {
                    text = text.Replace(entry.Key, $"{entry.Key} <{entry.Value}>");
                }
               
                // Mention and Trending lists
                #region Match Tweet Id
                foreach (Match match in Regex.Matches(text, @"\B\@\w{1,15}\b"))
                {
                    if (MentionsList.ContainsKey(match.ToString()))
                    {
                        MentionsList[match.ToString()] += 1;
                    } else
                    {
                        MentionsList.Add(match.ToString(), 1);
                    }
                    
                }
                #endregion
                #region Match Tweet hashtag
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

                #endregion

                message.Text = text;
                message.Header = header;
                message.Sender = sender;
                MessageCollection.TweetList.Add(message);
            }
            #endregion

            serializeToJSON();
        }
        public void PreprocessFile()
        {
            // Load the file 
            var filename = "csvmessages.txt";
            var path = Path.Combine(Environment.CurrentDirectory, filename);
            var lines = File.ReadAllLines(path);

            // Clear the error log so we can have fresh log
            UnloadedMessages.Clear();
            // Get message line by line and preprocess it
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
        }

        private void serializeToJSON()
        {
            string jsonString = JsonSerializer.Serialize(MessageCollection);
            File.WriteAllText("myMessages", jsonString);
        }

    }
}

