using NapierBankingApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NapierBankingApp.ViewModels
{
    class MainWindowViewModel : BaseViewModel
    {
        public ObservableCollection<Message> Messages { get; set; }
        public string HeaderTextBlock { get; private set; }
        public string BodyTextBlock { get; private set; }

        public string HeaderTextBox { get; set; }
        public string BodyTextBox { get; set; }

        public ICommand ProcessButtonCommand { get; private set; }
        public ICommand ClearButtonCommand { get; private set; }

        public string SaveButtonText { get; private set; }
        public string ClearButtonText { get; private set; }


        public MainWindowViewModel()
        {
            Messages = new ObservableCollection<Message>();
            HeaderTextBlock = "Header";


        }
    }
}
