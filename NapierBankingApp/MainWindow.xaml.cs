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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var message = preprocessor.PreprocessMessage(validator.ValidateMessage(txtBoxHeader.Text, txtBoxBody.Text));
                if (database.serializeToJSON(message))
                {
                    MessageBox.Show("Message Saved.");
                }
                else
                {
                    MessageBox.Show("Couldn't save message.");
                }
                updateMessages(message);
                updateUnloadedMessages();
                updateStats();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }




        }

        private void btnLoadFromFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<Message> messages = new List<Message>();
                foreach (var message in validator.ValidateFile(browseFile()))
                {
                    try
                    {
                        messages.Add(preprocessor.PreprocessMessage(message));
                        if (database.serializeToJSON(message))
                        {
                            MessageBox.Show("Message Saved.");
                        }
                        else
                        {
                            MessageBox.Show("Couldn't save message.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                updateMessages(messages);
                updateUnloadedMessages();
                updateStats();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

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
        public void updateMessages(Message message)
        {
            lstViewMessages.Items.Clear();
            lstViewMessages.Items.Add(message.ToString());
        }
        public void updateMessages(List<Message> messages)
        {
            lstViewMessages.Items.Clear();
            foreach(var message in messages)
            {
                lstViewMessages.Items.Add(message.ToString());
            }
            
        }
        public void updateUnloadedMessages()
        {
            lstViewUnloadedMessages.Items.Clear();
            foreach (var item in validator.UnloadedMessages)
            {
                lstViewUnloadedMessages.Items.Add(item.ToString());
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
    }
}
