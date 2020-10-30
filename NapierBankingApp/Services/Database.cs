using Microsoft.VisualBasic;
using NapierBankingApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace NapierBankingApp.Services
{
    class Database
    {
        private string _connectionPath;

        public Database(string connectionPath)
        {
            _connectionPath = connectionPath;
        }
        public string ConnectionPath
        { get { return _connectionPath; } }

        public bool serializeToJSON(Message message)
        {
            // Look into the message collection for duplicates
            MessageCollection collection = loadFile(_connectionPath);

            // Throw error if the database already contains message
            if (collection.SMSList.ContainsKey(message.Header) || collection.TweetList.ContainsKey(message.Header) || collection.SIRList.ContainsKey(message.Header) || collection.SEMList.ContainsKey(message.Header))
            {
                return false;
            }

            // add message to collection
            switch (message.MessageType)
            {
                case "S":
                    SMS sms = (SMS)message;
                    collection.SMSList.Add(message.Header, sms);
                    break;
                case "E":
                    Email email = (Email)message;
                    if (email.EmailType == "SEM")
                    {
                        SEM sem = (SEM)email;
                        collection.SEMList.Add(message.Header, sem);

                    }
                    else if (email.EmailType == "SIR")
                    {
                        SIR sir = (SIR)email;
                        collection.SIRList.Add(message.Header, sir);
                    }
                    break;
                case "T":
                    Tweet tweet = (Tweet)message;
                    collection.TweetList.Add(message.Header, tweet);
                    break;
                default:
                    break;
            }

            string jsonString = JsonSerializer.Serialize(collection, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            File.WriteAllText(ConnectionPath, jsonString);
            return true;
        }



        private MessageCollection loadFile(string fileName)
        {
            var jsonString = File.ReadAllText(fileName);
            MessageCollection collection = null;
            try
            {
                collection = JsonSerializer.Deserialize<MessageCollection>(jsonString);
            }
            catch (Exception ex)
            {
                collection = new MessageCollection();
            }
            
            return collection;
        }

        


        // Add deserializer
    }
}
