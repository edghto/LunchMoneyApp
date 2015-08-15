using System;
using System.Text;
using System.Windows;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;
using System.IO.IsolatedStorage;
using LunchMoneyApp;
using System.ComponentModel;


namespace BalanceChangePollAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private static volatile bool _classInitialized;

        /*
         * Are we sure that there will be only one instance of this class
         * at a time??
         */
        private static ScheduledAgent instance;

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        public ScheduledAgent()
        {
            if (!_classInitialized)
            {
                instance = this;
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
            LunchCardViewModel vm = new LunchCardViewModel();
            vm.LoadLunchCards();
            vm.RegisterForPropertyChangeEvent(PropertyChangedEventHandler);
            vm.UpdateAll();
        }

        public static void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Balance"))
            {
                LunchCard card = (LunchCard)sender;
                ShellToast popupMessage = new ShellToast()
                {
                    Title = "Balance for card " + card.Code + " changed: ",
                    Content = card.Balance.ToString(),
                    NavigationUri = new Uri("/MainPage.xaml", UriKind.Relative)
                };
                popupMessage.Show();
            }
#if DEBUG
            ScheduledActionService.LaunchForTest(Config.BALANCE_CHANGE_POLL_AGENT_NAME,
                    TimeSpan.FromMilliseconds(1500));
#endif
            instance.NotifyComplete();
        }
    }
}