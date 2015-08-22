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

namespace LunchMoneyApp
{
    public static class Config
    {
#if DEBUG
        public const string SERVER_URL = "http://192.168.0.101";
#else
        public const string SERVER_URL = "http://www.edenred.pl/mobileapp/";
#endif
        public const string BALANCE_CHANGE_POLL_AGENT_NAME = "LunchCard_BalanceChangePollAgent";
    }
}
