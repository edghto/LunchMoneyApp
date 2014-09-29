using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace LunchMoneyApp
{
    public partial class AddPage : PhoneApplicationPage
    {
        private LunchCardViewModel vm;

        public AddPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e); 
            if (App.vm != null)
            {
                vm = App.vm ?? new LunchCardViewModel();
            }
            else
            {
                MessageBox.Show("Unknown error!");
                this.NavigationService.GoBack();
            }
        }

        private void ApplicationBarIconButtonAdd_Click(object sender, EventArgs e)
        {
            if (codeTextBox.Text.ToString().Length == 4 &&
                cardNumberTextBox.Text.ToString().Length == 10)
            {
                if (vm.Add(Int16.Parse(codeTextBox.Text.ToString()),
                    Int32.Parse(cardNumberTextBox.Text.ToString())))
                {
                    this.NavigationService.GoBack();
                }
                else
                {
                    MessageBox.Show("Internal error!");
                }
            }
            else
            {
                MessageBox.Show("Input invalid!");
            }

        }

        private void ApplicationBarIconButtonCancel_Click(object sender, EventArgs e)
        {
            this.NavigationService.GoBack();
        }
        
    }
}