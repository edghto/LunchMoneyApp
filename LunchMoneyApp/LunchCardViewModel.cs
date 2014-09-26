using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace LunchMoneyApp
{
    public class LunchCardViewModel
    {
        public ObservableCollection<LunchCard> LunchCards { get; set; }


        public void LoadLunchCards()
        {
            ObservableCollection<LunchCard> l = new ObservableCollection<LunchCard>();

            l.Add(new LunchCard() { Code = 1234, CardNumber = 1234567890, Balance = 0, LastCheckd = "never" });

            LunchCards = l;
        }
    }
}
