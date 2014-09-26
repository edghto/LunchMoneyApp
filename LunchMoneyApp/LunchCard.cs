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
using System.ComponentModel;

namespace LunchMoneyApp
{
    public class LunchCard : INotifyPropertyChanged
    {

        private System.DateTime? lastDate = null;

        private int _code;
        public int Code
        {
            get
            {
                return _code;
            }
            set
            {
                _code = value;
                RaisePropertyChanged("Code");
            }
        }

        
        private long _cardNo;
        public long CardNumber
        {
            get
            {
                return _cardNo;
            }
            set
            {
                _cardNo = value;
                RaisePropertyChanged("CardNumber");
            }
        }

        private double _balance;
        public double Balance
        {
            get
            {
                return _balance;
            }
            set
            {
                _balance = value;
                RaisePropertyChanged("Balance");
            }
        }

        private string _lastCheckd;
        public string LastCheckd
        {
            get
            {
                return _lastCheckd;
            }
            set
            {
                _lastCheckd = value;
                RaisePropertyChanged("LastCheckd");
            }
        }


        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        
        public bool update()
        {
            lastDate = new DateTime(2014, 09, 25);
            DateTime current = DateTime.Now.Date;
            string tmp = "";

            if (lastDate == null)
            {
                lastDate = current;
                LastCheckd = "0 sec";
                return true;
            }

            TimeSpan s = current.Subtract(lastDate ?? default(DateTime));
            String unit = null;

            if (s.Seconds != 0)
            {
                unit = "sec";
                tmp = s.Seconds.ToString();
            }
            else if (s.Minutes != 0)
            {
                unit = "min";
                tmp = s.Minutes.ToString();
            }
            else if (s.Hours != 0)
            {
                unit = "h";
                tmp = s.Hours.ToString();
            }
            else if (s.Days != 0)
            {
                unit = s.Days == 1 ? "day" : "days";
                tmp = s.Days.ToString();
            }

            if (unit != null)
                LastCheckd = tmp + " " + unit;

            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
