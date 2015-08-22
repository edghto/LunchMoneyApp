using System;
using System.Text;
using System.Windows;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;
using LunchMoneyApp;
using System.ComponentModel;


namespace BalanceChangePollAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private static volatile bool _classInitialized;

        private ScheduledAgent instance;

        private LunchCardViewModel vm = new LunchCardViewModel();

        private int count;

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
            vm = new LunchCardViewModel();
            count = vm.LoadLunchCards();
            vm.RegisterForPropertyChangeEvent(PropertyChangedEventHandler);
            vm.UpdateAll();
        }

        public void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            LunchCard card = (LunchCard) sender;
            if (e.PropertyName.Equals("Balance"))
            {
                ShellToast popupMessage = new ShellToast()
                {
                    Title = "Balance for card " + card.Code + " changed: ",
                    Content = card.Balance.ToString(),
                    NavigationUri = new Uri("/MainPage.xaml", UriKind.Relative)
                };
                popupMessage.Show();
            }

            /* 
             * This property is always notified, so we can use it to count 
             * how many card was updated and how many left.
             * We are assuming that BeginInvoke preserves order of changed properties.
             * So Balance should be called before LastChecked property.
             */
            if (e.PropertyName.Equals("LastChecked"))
            {
                vm.UnregisterForPropertyChangeEvent(PropertyChangedEventHandler, card);
                NotifyCardComplete();
            }
        }

        private void NotifyCardComplete()
        {
            count -= 1;
            if( count <= 0)
            {
                count = 0;
#if DEBUG
                ScheduledActionService.LaunchForTest(Config.BALANCE_CHANGE_POLL_AGENT_NAME,
                        TimeSpan.FromMilliseconds(1500));
#endif
                instance.NotifyComplete();
            }
        }
    }
}