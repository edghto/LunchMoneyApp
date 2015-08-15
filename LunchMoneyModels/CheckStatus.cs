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
