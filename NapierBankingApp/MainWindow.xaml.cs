using NapierBankingApp.Models;
using NapierBankingApp.Services;
using System;
using System.Collections.Generic;
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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                preprocessor.PreprocessMessage(txtBoxHeader.Text, txtBoxBody.Text);
                updateMessages();
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
                preprocessor.PreprocessFile();
                updateMessages();
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
            lstStats.Items.Clear();
            foreach (var item in preprocessor.MentionsList)
            {
                lstStats.Items.Add(item.ToString());
            }
            foreach (var item in preprocessor.TrendingList)
            {
                lstStats.Items.Add(item.ToString());
            }
        }
        public void updateMessages()
        {
            lstViewMessages.Items.Clear();
            foreach (var item in preprocessor.MessageCollection.SMSList)
            {
                lstViewMessages.Items.Add(item.ToString());
            }
            foreach (var item in preprocessor.MessageCollection.TweetList)
            {
                lstViewMessages.Items.Add(item.ToString());
            }
        }
        public void updateUnloadedMessages()
        {
            lstViewUnloadedMessages.Items.Clear();
            foreach (var item in preprocessor.UnloadedMessages)
            {
                lstViewUnloadedMessages.Items.Add(item.ToString());
            }
        }
    }
}
