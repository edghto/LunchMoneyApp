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

        private DateTime? lastDate = null;

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
            String diff = null;
            TimeDiff td = new TimeDiff(current, lastDate);


            if (lastDate == null)
            {
                lastDate = current;
                LastCheckd = "0 sec";
                return true;
            }

            diff = td.diff();
            if(diff != null)
                LastCheckd = diff;

            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        class TimeDiff
        {
            private DateTime d1;
            private DateTime d2;

            public TimeDiff(DateTime d1, DateTime? d2)
            {
                this.d1 = d1;
                this.d2 = d2 ?? d1; /* if second is null just use first one */
            }

            public string diff()
            {
                TimeSpan ts = d1.Subtract(d2);
                string unit = null;
                string tmp = "";

                if (ts.Seconds != 0)
                {
                    unit = "sec";
                    tmp = ts.Seconds.ToString();
                }
                else if (ts.Minutes != 0)
                {
                    unit = "min";
                    tmp = ts.Minutes.ToString();
                }
                else if (ts.Hours != 0)
                {
                    unit = "h";
                    tmp = ts.Hours.ToString();
                }
                else if (ts.Days != 0)
                {
                    unit = ts.Days == 1 ? "day" : "days";
                    tmp = ts.Days.ToString();
                }

                return unit != null ? tmp + " " + unit: null;
            }
        }

    }
}
