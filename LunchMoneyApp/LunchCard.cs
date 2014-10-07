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
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Windows.Threading;

namespace LunchMoneyApp
{
    public class LunchCard : INotifyPropertyChanged
    {
        public bool isNew { get; set; } //This shouldn't be public
        private DateTime _lastDate;
        public DateTime LastDate
        {
            get
            {
                return _lastDate;
            }
            set
            {
                _lastDate = value;
                RaisePropertyChanged("LastDate");
            }
        }

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
        public string LastChecked
        {
            get
            {
                return _lastCheckd;
            }
            set
            {
                _lastCheckd = value;
                RaisePropertyChanged("LastChecked");
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        void processResponse(String response)
        {
            DateTime current = DateTime.Now;
            TimeDiff td = new TimeDiff();
            string diff = null;
            double balance = 0;

            try
            {
                JObject jsonObject = JObject.Parse(response);
                JObject balanceJsonObj = jsonObject.Value<JObject>("balance");
                JObject cardJsonObj = balanceJsonObj.Value<JObject>(CardNumber + "");
                balance = cardJsonObj.Value<double>("amount");

                if (isNew)
                {
                    diff = "0 sec";
                }
                else
                {
                    diff = td.diff(current, LastDate);
                }
                
                isNew = false;
                LastDate = current;

                Deployment.Current.Dispatcher.BeginInvoke(() => { Balance = balance; LastChecked = diff; });
            }
            catch
            {
                diff = "Error";
                Deployment.Current.Dispatcher.BeginInvoke(() => { LastChecked = diff; });
            }
        }

        public void refreshLastCheckedProperty()
        {
            DateTime current = DateTime.Now;
            TimeDiff td = new TimeDiff();
            string diff;

            if (isNew)
            {
                diff = "Never";
            }
            else
            {
                try
                {
                    diff = td.diff(current, LastDate);
                }
                catch
                {
                    diff = "Error";
                }
            }

            LastChecked = diff;
        }

        public bool update()
        {
            HttpPOSTWorker httpPostWorker = new HttpPOSTWorker();
            string postData = string.Format(
                "action=mobileweb&code={0}&cardNumber={1}&lang=pl",
                Code, CardNumber);
            try
            {
                httpPostWorker.connect("http://www.edenred.pl/mobileapp/",
                    Encoding.UTF8.GetBytes(postData), new ProcessHttpPostResponse(processResponse));
            }
            catch
            {
                return false;
            }

            return true;
        }

        public object getCopy()
        {
            LunchCard copy = (LunchCard)this.MemberwiseClone();
            return copy;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        class TimeDiff
        {
            public class DiffException : SystemException {}

            public string diff(DateTime d1, DateTime d2)
            {
                TimeSpan ts = d1.Subtract(d2);
                string unit = null;
                string tmp = "";
                
                if (ts.Seconds != 0)
                {
                    unit = "sec";
                    tmp = ts.Seconds.ToString();
                }
                if (ts.Minutes != 0)
                {
                    unit = "min";
                    tmp = ts.Minutes.ToString();
                }
                if (ts.Hours != 0)
                {
                    unit = "h";
                    tmp = ts.Hours.ToString();
                }
                if (ts.Days != 0)
                {
                    unit = ts.Days == 1 ? "day" : "days";
                    tmp = ts.Days.ToString();
                }

                if (unit == null)
                    throw new DiffException();

                return tmp + " " + unit;
            }
        }
    }
}
