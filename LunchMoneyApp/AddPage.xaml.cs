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
using LunchMoneyModels;

namespace LunchMoneyApp
{
    public partial class AddPage : PhoneApplicationPage
    {
        public AddPage()
        {
            InitializeComponent();
        }

        private void ApplicationBarIconButtonAdd_Click(object sender, EventArgs e)
        {
            string codeStr = codeTextBox.Text.ToString();
            string cardNumberStr = cardNumberTextBox.Text.ToString();
            Int64 cardNumber;
            Int16 code;

            if (codeStr.Length == 4 &&
                cardNumberStr.Length == 10 &&
                Int16.TryParse(codeStr, out code) &&
                Int64.TryParse(cardNumberStr, out cardNumber) &&
                code > 0 && cardNumber > 0)
            {
                App.lunchCard = new LunchCard()
                {
                    Code = code,
                    CardNumber = cardNumber,
                    Balance = 0,
                    LastChecked = new CheckStatus(),
                    isNew = true
                };
                this.NavigationService.GoBack();
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