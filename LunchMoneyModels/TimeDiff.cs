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

namespace LunchMoneyModels
{
    public class TimeDiff
    {
        public class DiffException : SystemException { }

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
