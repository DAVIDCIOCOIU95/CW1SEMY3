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

        public void serializeToJSON(Message message)
        {
            // Look into the message collection for duplicates
            MessageCollection collection = loadFile(_connectionPath);

            // add message to collection or throw error
            switch (message.MessageType)
            {
                case "S":
                    if (collection.SMSList.ContainsKey(message.Header))
                    {
                        throw new Exception($"{message.Header} message not saved: the database already contains the message.");
                    }
                    SMS sms = (SMS)message;
                    collection.SMSList.Add(message.Header, sms);
                    break;
                case "E":
                    Email email = (Email)message;
                    if (email.EmailType == "SEM")
                    {
                        if (collection.SEMList.ContainsKey(message.Header))
                        {
                            throw new Exception($"{message.Header} message not saved: the database already contains the message.");
                        }
                        SEM sem = (SEM)email;
                        collection.SEMList.Add(message.Header, sem);

                    }
                    else if (email.EmailType == "SIR")
                    {
                        if (collection.SIRList.ContainsKey(message.Header))
                        {
                            throw new Exception($"{message.Header} message not saved: the database already contains the message.");
                        }
                        SIR sir = (SIR)email;
                        collection.SIRList.Add(message.Header, sir);
                    }
                    break;
                case "T":
                    if (collection.TweetList.ContainsKey(message.Header))
                    {
                        throw new Exception($"{message.Header} message not saved: the database already contains the message.");
                    }
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
        }



        private MessageCollection loadFile(string fileName)
        {
            var jsonString = File.ReadAllText(fileName);
            MessageCollection collection = null;
            try
            {
                collection = JsonSerializer.Deserialize<MessageCollection>(jsonString);
            }
            // Create a new collection if no file exists in order to avoid throwing an extra error
            catch (Exception ex)
            {
                collection = new MessageCollection();
            }
            
            return collection;
        }
    }
}
