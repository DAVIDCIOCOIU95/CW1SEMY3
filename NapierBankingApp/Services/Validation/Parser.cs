using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NapierBankingApp.Services.Validation
{
    class Parser
    {
        /// <summary>
        /// Parses a structured csv file line by line getting a list of message entities. Each message entity is in the form of string[], where index 0 is the header and index 1 is the body.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>A list of message entities, each containing header and body.</returns>
        public static List<string[]> ParseCsvFile(string filename, string delimiter)
        {
            var path = Path.Combine(filename);
            TextFieldParser parser = new TextFieldParser(path);
            parser.HasFieldsEnclosedInQuotes = false;
            parser.SetDelimiters(delimiter);

            List<string[]> fields = new List<string[]>();
            while (!parser.EndOfData)
            {
                fields.Add(parser.ReadFields());
            }
            parser.Close();
            return fields;
        }

        /// <summary>
        /// Separates a block of text into fields.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="delimiter"></param>
        /// <param name="hasQuotes"></param>
        /// <returns>A list of strings containing all the fields in the body.</returns>
        public static List<string> ParseBody(string body, string delimiter, bool hasQuotes)
        {
            body = body.Replace("\n", " ").Replace("\r", " ");
            StringReader sr = new StringReader(body);
            TextFieldParser parser = new TextFieldParser(sr);
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
    }
}
