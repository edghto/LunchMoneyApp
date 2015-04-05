using System;
using System.Text;
using System.Windows;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;
using System.IO.IsolatedStorage;
using Newtonsoft.Json.Linq;
using LunchMoneyApp;
using System.ComponentModel;


namespace BalanceChangePollAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private static volatile bool _classInitialized;

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        public ScheduledAgent()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }
        }

        /// Code to execute on Unhandled Exceptions
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
#if FOO 
            string url = "http://www.edenred.pl/mobileapp/";
            string postData = string.Format(
                "action=mobileweb&code={0}&cardNumber={1}&lang=pl",
                Code, CardNumber);
            HttpPOSTWorker.invoke(url, postData, processResponse);
#else
            LunchCardViewModel vm = new LunchCardViewModel();
            vm.LoadLunchCards();
            vm.UpdateAll(PropertyChangedEventHandler);
#endif
        }

        public void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Balance"))
            {
                ShellToast popupMessage = new ShellToast()
                {
                    Title = "Balance changed: ",
                    Content = Balance.ToString(),
                    NavigationUri = new Uri("/MainPage.xaml", UriKind.Relative)
                };
                popupMessage.Show();
            }
#if DEBUG
            ScheduledActionService.LaunchForTest(Config.BALANCE_CHANGE_POLL_AGENT_NAME,
                    TimeSpan.FromMilliseconds(1500));
#endif
            NotifyComplete();
        }
#if FOO
        void processResponse(String response)
        {
            double balance = 0;

            try
            {
                JObject jsonObject = JObject.Parse(response);
                JObject balanceJsonObj = jsonObject.Value<JObject>("balance");
                JObject cardJsonObj = balanceJsonObj.Value<JObject>(CardNumber + "");
                balance = cardJsonObj.Value<double>("amount");


                if (Balance != balance)
                {
                    Balance = balance;
                    ShellToast popupMessage = new ShellToast()
                    {
                        Title = "Lunch Card status changed",
                        Content = "Current balance: " + Balance,
                        NavigationUri = new Uri("/App.xaml", UriKind.Relative)
                    };
                    popupMessage.Show();
                }
            }
            catch
            {
            }

            NotifyComplete();


        }
#endif

        public Double Balance; 
        public String CardNumber;
        public String Code; 
    }
}