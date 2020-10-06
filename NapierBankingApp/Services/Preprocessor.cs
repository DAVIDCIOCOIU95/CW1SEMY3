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

            // Work on the body
            var bodyArray = body.Split('|');
            MessageBox.Show(bodyArray[0]);
            if ((bodyArray.Length == 0))
            {
                throw new Exception("The body must have at least a sender specified.");
            }
            var sender = bodyArray[0];
            var text = "";
            if (bodyArray.Length >= 2)
            {
                text = bodyArray[1];
            }

            // Preprocess sms 
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
                sender = sender.Replace(" ", "").Replace("  ","").Replace("_", "").Replace("-", "").Replace("#", "").Replace("*", "");
                sender =  Regex.Match(sender, @"^\+\d{1,15}$").Value;
                if(sender.Length == 0)
                {
                    throw new Exception("Your number must start with a + and must be followed by 1 or less than 15 numbers. The numbers must be consecutive.");
                }
                // Load the message to messageCollection
                message.Header = header;
                message.Sender = sender;
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
                    MessageBox.Show(messageArray[0]);
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

