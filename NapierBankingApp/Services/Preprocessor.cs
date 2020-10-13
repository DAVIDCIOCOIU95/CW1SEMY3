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
            LoadAbbreviations("textwords.csv");
        }

        

        #region Preprocessors
        private Message PreprocessTweet(Message message)
        {
            message.Text = SobstituteAbbreviations(message.Text);
            AddToMentionList(message.Text);
            AddToTrendingList(message.Text);
            return message;
        }
        private Message PreprocessEmail(Message message)
        {
            return message;
        }
        private Message PreprocessSMS(Message message)
        {
            message.Text = SobstituteAbbreviations(message.Text);
            return message;
        }
        public void PreprocessMessage(Message message)
        {
            switch (message.MessageType)
            {
                case "S":
                    message = PreprocessSMS(message);
                    break;
                case "E":
                    message = PreprocessEmail(message);
                    break;
                case "T":
                    message = PreprocessTweet(message);
                    break;
                default:
                    throw new Exception("Incorrect message type");
            }
            database.serializeToJSON(MessageCollection);
        }
       
        #endregion

        #region Preprocessor private methods
        private void LoadAbbreviations(string filename)
        {
            var path = Path.Combine(Environment.CurrentDirectory, filename);
            var lines = File.ReadAllLines(path);

            foreach (string line in lines)
            {
                var abbreviation = line.Split(',');
                abbreviations.Add(abbreviation[0], abbreviation[1]);
            }
        }
        private string SobstituteAbbreviations(string text)
        {
            foreach (var entry in abbreviations)
            {
                text = text.Replace(entry.Key, $"{entry.Key} <{entry.Value}>");
            }
            return text;
        }

        /// <summary>
        /// Checks if the text has any occurencies of twitter Ids, is so updates the instance, otherwise it adds it to the mention list.
        /// </summary>
        /// <param name="text"></param>
        private void AddToMentionList(string text)
        {
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
        }

        /// <summary>
        /// Checks if the list has any mentions of the trending list, if so updates the list, otherwise it adds the new instance to the trending list.
        /// </summary>
        /// <param name="text"></param>
        private void AddToTrendingList(string text)
        {
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
        }
        #endregion
    }
}