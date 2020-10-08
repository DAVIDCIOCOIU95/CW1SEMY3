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
            if (header[0] != 'S' || header[0] != 'E' || header[0] != 'T')
            {
                throw new Exception("Incorrect header type. Make sure you start your header with: S, E or T.");
            }
            #endregion

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

            if (header[0] == 'S')
            { PreprocessSMS(header, sender, text); }
            else if (header[0] == 'E')
            { PreprocessEmail(header, sender, text); }
            else if (header[0] == 'T')
            { PreprocessTweet(header, sender, text); }

            serializeToJSON();
        }

        private void PreprocessEmail(string header, string sender, string text)
        {
            throw new NotImplementedException();
        }

        private Message PreprocessTweet(string header, string sender, string text)
        {
            Tweet message = new Tweet();
           
            // Validate sender
            sender = Regex.Match(sender, @"^\@[a-zA-Z0-9_]{1,15}$").Value;
            if (sender.Length == 0)
            {
                throw new Exception("Invalid sender format. Sender must start with a @ and be followed by 1 to 15 numbers and/or letters.");
            }
            
            // Validate Text
            if (text.Length > 140)
            {
                throw new Exception("The text length contains" + text.Length + " characters.\nThe max characters allowed is: 140.");
            }

            // Abbreviations replacement
            foreach (var entry in abbreviations)
            {
                text = text.Replace(entry.Key, $"{entry.Key} <{entry.Value}>");
            }

            // Add to Mention list
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
          
            // Add to Trending list
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
            
            // Set up message fields and load to the list of preprocessed messages
            message.Header = header;
            message.MessageType = "T";
            message.Sender = sender;
            message.Text = text;
            MessageCollection.TweetList.Add(message);
            return message;
        }

        private Message PreprocessSMS(string header, string sender, string text)
        {
            SMS message = new SMS();

            // Check text length is max 140 chars
            if (text.Length > 140)
            {
                throw new Exception("The text length contains" + text.Length + " characters.\nThe max characters allowed is: 140.");
            }

            // Sobstitute abbreviations
            foreach (var entry in abbreviations)
            {
                text = text.Replace(entry.Key, $"{entry.Key} <{entry.Value}>");
            }

            // Clean the number from any extra char
            sender = sender.Replace(" ", "").Replace("  ", "").Replace("_", "").Replace("-", "").Replace("#", "").Replace("*", "");

            // Check the sender is in the correct format: @ followed by 15 numbers
            sender = Regex.Match(sender, @"^\+\d{1,15}$").Value;
            if (sender.Length == 0)
            {
                throw new Exception("Invalid sender format. Sender must start with a + and be followed by 1 to 15 numbers.");
            }

            // Assign the fields in the message and load it to message collection
            message.Header = header;
            message.MessageType = "S";
            message.Sender = sender;
            message.Text = text;

            MessageCollection.SMSList.Add(message);
            return message;
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

