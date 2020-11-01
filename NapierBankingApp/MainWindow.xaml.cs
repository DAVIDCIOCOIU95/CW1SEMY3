using NapierBankingApp.Models;
using NapierBankingApp.Services;
using NapierBankingApp.Services.Validation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NapierBankingApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Preprocessor preprocessor = new Preprocessor();
        Validator validator = new Validator();
        Database database = new Database("myMessage");
        List<Message> currentMessages = new List<Message>();
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Process_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                messageErrors.Items.Clear();
                var message = preprocessor.PreprocessMessage(validator.ValidateMessage(txtBoxHeader.Text, txtBoxBody.Text));
                currentMessages.Add(message);
                updateProcessedMessages();
                updateStats();
            }
            catch (Exception ex)
            {
                messageErrors.Items.Add("Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Saves the processed messages to a database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            messageErrors.Items.Clear();
            savedMessages.Items.Clear();
            foreach (var message in currentMessages)
            {
                try
                {
                    database.serializeToJSON(message);
                    savedMessages.Items.Add(message);
                }
                catch (Exception ex)
                {
                    messageErrors.Items.Add("Error for message: " + message.Header + " " + ex.Message);
                }
            }
        }

        private void LoadFromFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                messageErrors.Items.Clear();
                foreach (var message in validator.ValidateFile(browseFile()))
                {
                    try
                    {
                        currentMessages.Add(preprocessor.PreprocessMessage(message));
                    }
                    catch (Exception ex)
                    {
                        messageErrors.Items.Add(ex.Message);
                    }
                }
                updateProcessedMessages();
                updateStats();
                foreach(var unloaded in validator.UnloadedMessages)
                {
                    messageErrors.Items.Add(unloaded);
                }
              

            }
            catch (Exception ex)
            {
                messageErrors.Items.Add(ex.Message);
            }
        }

        /// <summary>
        /// Updates the statistics lists in the UI getting them from the preprocessor.
        /// </summary>
        public void updateStats()
        {
            lstTrends.Items.Clear();
            lstMentions.Items.Clear();
            lstSIR.Items.Clear();
            foreach (var item in preprocessor.MentionsList)
            {
                lstMentions.Items.Add(item.ToString());
            }
            foreach (var item in preprocessor.TrendingList)
            {
                lstTrends.Items.Add(item.ToString());
            }
            foreach (var item in preprocessor.SirList)
            {
                lstSIR.Items.Add(item.ToString());
              
            }
        }

        /// <summary>
        /// Updates the processed messages list.
        /// </summary>
        /// <param name="messages"></param>
        public void updateProcessedMessages()
        {
            processedMessages.Items.Clear();
            foreach (var message in currentMessages)
            {
                processedMessages.Items.Add(message.ToString());
            }

        }
        public void MessageErrors()
        {
            messageErrors.Items.Clear();
            foreach (var item in validator.UnloadedMessages)
            {
                messageErrors.Items.Add(item.ToString());
            }
        }

        private string browseFile()
        {
            var FD = new System.Windows.Forms.OpenFileDialog();
            if (FD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileToOpen = FD.FileName;
                //FileInfo File = new FileInfo(FD.FileName);
                //StreamReader reader = new StreamReader(fileToOpen);
                return fileToOpen;
                //etc
            }
            return "";
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            processedMessages.Items.Clear();
            currentMessages.Clear();
            savedMessages.Items.Clear();
        }
    }
}
