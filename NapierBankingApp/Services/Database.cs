using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        public void serializeToJSON(object artefact)
        {
            string jsonString = JsonSerializer.Serialize(artefact);

            File.AppendAllText(ConnectionPath, jsonString);
        }

        


        // Add deserializer
    }
}
