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
        public string ServerUrl { get;  set; }
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

        private CheckStatus _lastCheckd;
        public CheckStatus LastChecked
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
            CheckStatus checkStatus = new CheckStatus();
            double balance = 0;

            checkStatus.isNew = false;
            try
            {
                JObject jsonObject = JObject.Parse(response);
                JObject balanceJsonObj = jsonObject.Value<JObject>("balance");
                JObject cardJsonObj = balanceJsonObj.Value<JObject>(CardNumber + "");
                balance = cardJsonObj.Value<double>("amount");

                checkStatus.status = true;
                checkStatus.time = 0;
                checkStatus.timeUnit = CheckStatus.TimeUnit.SECONDS;
                
                isNew = false;
                LastDate = DateTime.Now;

                Deployment.Current.Dispatcher.BeginInvoke(() => { Balance = balance; LastChecked = checkStatus; });
            }
            catch
            {
                checkStatus.status = false;
                Deployment.Current.Dispatcher.BeginInvoke(() => { LastChecked = checkStatus; });
            }
        }

        public void refreshLastCheckedProperty()
        {
            DateTime current = DateTime.Now;
            TimeDiff td = new TimeDiff();
            CheckStatus checkStatus = new CheckStatus();

            if (isNew)
            {
                checkStatus.isNew = true;
            }
            else
            {
                checkStatus.isNew = false;
                try
                {
                    td.diff(current, LastDate, checkStatus);
                    checkStatus.status = true;
                }
                catch
                {
                    checkStatus.status = false;
                }
            }

            LastChecked = checkStatus;
        }

        public bool update()
        {
            HttpPOSTWorker httpPostWorker = new HttpPOSTWorker();
            string postData = string.Format(
                "action=mobileweb&code={0}&cardNumber={1}&lang=pl",
                Code, CardNumber);
            try
            {
                httpPostWorker.connect(ServerUrl,
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

            public void diff(DateTime d1, DateTime d2, CheckStatus checkStatus)
            {
                TimeSpan ts = d1.Subtract(d2);
                
                if (ts.Seconds != 0)
                {
                    checkStatus.timeUnit = CheckStatus.TimeUnit.SECONDS;
                    checkStatus.time = ts.Seconds;
                }
                if (ts.Minutes != 0)
                {
                    checkStatus.timeUnit = CheckStatus.TimeUnit.MINUTES;
                    checkStatus.time = ts.Minutes;
                }
                if (ts.Hours != 0)
                {
                    checkStatus.timeUnit = CheckStatus.TimeUnit.HOURS;
                    checkStatus.time = ts.Hours;
                }
                if (ts.Days != 0)
                {
                    checkStatus.timeUnit = CheckStatus.TimeUnit.DAYS;
                    checkStatus.time = ts.Days;
                }

                if (checkStatus.timeUnit == CheckStatus.TimeUnit.NONE)
                    throw new DiffException();
            }
        }        
    }

    public class CheckStatus
    {
        public enum TimeUnit { DAYS, HOURS, MINUTES, SECONDS, NONE };

        public int time { get; set; }
        public TimeUnit timeUnit { get; set; }
        public bool status { get; set; }
        public bool isNew { get; set; }

        public CheckStatus()
        {
            time = -1;
            timeUnit = CheckStatus.TimeUnit.NONE;
            status = false;
            isNew = true;
        }
    }
}
