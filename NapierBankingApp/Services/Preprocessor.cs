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
                #region AbbreviationSanitizer
                // Sanitize key value on any extra comma
                if (abbreviation.Length > 2)
                {
                    abbreviation[0] = abbreviation[0];
                    for (int counter = 1; counter < abbreviation.Length; counter++)
                    {
                        abbreviation[1] = abbreviation[1] + "," + abbreviation[counter];
                    }
                }
                #endregion
                abbreviations.Add(abbreviation[0], abbreviation[1]);
            }
        }

        public void PreprocessMessage(string header, string body)
        {
            header = header.ToUpper();
            if (header.Length == 10)
            {
                // Preprocess sms
                if (header[0] == 'S')
                {
                    SMS message = new SMS();
                    message.MessageType = "S";

                    // Split the text and make sure it contains only sender and text. If text is > 140 then throw error.
                    var bodyArray = body.Split('|');
                    if (bodyArray.Length > 2)
                    {
                        throw new Exception("The body can have only 2 fields, please make sure to prepend sender and body with |.");
                    }
                    var sender = bodyArray[0];
                    var text = bodyArray[1];
                    if (text.Length > 140)
                    {
                        throw new Exception("The text length contains" + text.Length + " characters.\nThe max characters allowed is: 140.");
                    }
                    // Clean the number
                    if (header[0] == '+')
                    {
                        sender = "+" + Regex.Match(sender, @"\d+").Value;
                    }
                    else
                    {
                        sender = Regex.Match(sender, @"\d+").Value;
                    }
                    // Sobstitute abbreviations
                    foreach (var entry in abbreviations)
                    {
                        text = Regex.Replace(text, entry.Key, entry.Value);
                    }
                    // Load the message to messageCollection
                    MessageCollection.SMSList.Add(message);
                }
                // Preprocess email
                else if (header[0] == 'E')
                {

                }
                // Preprocess tweet
                else if (header[0] == 'T')
                {
                }
                serializeToJSON();
            }
            else
            {
                throw new Exception("The header can not be empty and must start with one of the following characters: S,E,T.");
            }
        }
        public void PreprocessFile()
        {
            // Load the file 
            var filename = "myCsvMessages";
            var path = Path.Combine(Environment.CurrentDirectory, filename);
            var lines = File.ReadAllLines(path);

            // Get message line by line and preprocess it
            foreach (var line in lines)
            {
                var messageArray = line.Split(',');
                if (messageArray.Length > 2)
                {
                    for (int counter = 1; counter < messageArray.Length; counter++)
                    {
                        messageArray[1] = messageArray[1] + "," + messageArray[counter];
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

