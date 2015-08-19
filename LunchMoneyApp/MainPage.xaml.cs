using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LunchMoneyModels;


namespace LunchMoneyApp
{
    public class LastCheckedStackPanel : StackPanel
    {
        public static readonly DependencyProperty LastCheckedProperty =
            DependencyProperty.RegisterAttached("LastChecked",
             typeof(CheckStatus),
             typeof(LastCheckedStackPanel),
             new PropertyMetadata(new CheckStatus(), OnFooChanged));

        public static void SetLastChecked(DependencyObject obj, CheckStatus value)
        {
            obj.SetValue(LastCheckedProperty, value);
        }

        public static CheckStatus GetLastChecked(DependencyObject obj)
        {
            return (CheckStatus)obj.GetValue(LastCheckedProperty);
        } 

        //TODO rename this handler, foo seems to be inappropriate 
        static void OnFooChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) 
        {
            TextBlock lastCheckedTextBlock = obj as TextBlock;
            TextBlock helperStatusTextBlock = (TextBlock)
                ((VisualTreeHelper.GetParent(lastCheckedTextBlock) as LastCheckedStackPanel).FindName("HelperStatus"));
            CheckStatus checkStatus = (CheckStatus)args.NewValue;

            if (checkStatus.status)
            {
                lastCheckedTextBlock.Text = checkStatus.time + " ";
                switch (checkStatus.timeUnit)
                {
                case CheckStatus.TimeUnit.DAYS:
                    lastCheckedTextBlock.Text += checkStatus.time > 0 ? "days" : "day"; break;
                case CheckStatus.TimeUnit.HOURS:
                    lastCheckedTextBlock.Text += "h"; break;
                case CheckStatus.TimeUnit.MINUTES:
                    lastCheckedTextBlock.Text += "min"; break;
                case CheckStatus.TimeUnit.SECONDS:
                    lastCheckedTextBlock.Text += "sec"; break;
                }
            }
            else if (checkStatus.isNew)
            {
                lastCheckedTextBlock.Text = "Never";
                helperStatusTextBlock.Text = "";
            }
            else
            {
                lastCheckedTextBlock.Text = "Error";
                helperStatusTextBlock.Text = "";
            }

            ProgressIndicatorController.Off();
         } 
    }

    public partial class MainPage : PhoneApplicationPage
    {
        private LunchCardViewModel vm;
        //private LiveTileViewModel liveTileVm;

        private BgTaskManager taskManager;

        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            /* first launch of app */
            StateUtility.IsLaunching = true;
        }

        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            /* we have just get back to already running app */
            StateUtility.IsLaunching = false;
        }

        public MainPage()
        {
            InitializeComponent();
            vm = new LunchCardViewModel();
            //liveTileVm = new LiveTileViewModel();
            taskManager = new BgTaskManager(){
                TaskDescription = "Retreives balance of your lunch card",
                TaskName = Config.BALANCE_CHANGE_POLL_AGENT_NAME
            };
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (!StateUtility.IsLaunching && this.State.ContainsKey("LunchCards"))
            {
                vm = (LunchCardViewModel)this.State["LunchCards"];
            }
            else
            {
                vm.LoadLunchCards();
            }

            /*
             * The hacky way to pass data between AddPage and Main Page.
             * Probably it would be better to pass it as url params.
             * TODO fix it!
             */
            if (App.lunchCard != null)
            {
                vm.Add(App.lunchCard);
                App.lunchCard = null;
            }

            LunchCardList.DataContext = vm.LunchCards;
            vm.RefreshLastChecked();

            //TODO - fix this ugly hardcode index of menu item
            TaskMenuItem_Text((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[0], taskManager.isEnabled());
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (this.State.ContainsKey("LunchCards"))
            {
                this.State["LunchCards"] = vm;
            }
            else
            {
                this.State.Add("LunchCards", vm);
            }

            StateUtility.IsLaunching = false;
        }

        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ProgressIndicatorController.On(this);
                LunchCard card = (LunchCardList.SelectedItem as LunchCard);
                card.update();
                (sender as ListBox).SelectedIndex = -1;

            }
        }

        //private void ContextMenuButtonPinToStart_Click(object sender, EventArgs e)
        //{
        //    MenuItem menuItem = (sender as MenuItem);
        //    LunchCard card = ((VisualTreeHelper.GetParent(menuItem) as FrameworkElement).DataContext as LunchCard);

        //    try
        //    {
        //        liveTileVm.create(card);
        //    }
        //    catch (LiveTileExists)
        //    {
        //        MessageBox.Show("Tile already exists!"); //TODO probably not needed
        //    }
        //    catch
        //    {
        //        MessageBox.Show("Creating live tile failed!");
        //    }
        //}

        private void ContextMenuButtonDelete_Click(object sender, EventArgs e)
        {
            MenuItem menuItem = (sender as MenuItem);
            LunchCard card = ((VisualTreeHelper.GetParent(menuItem) as FrameworkElement).DataContext as LunchCard);
            vm.Del(card);
        }

        private void ConextMenuButtonUpdate_Click(object sender, EventArgs e)
        {
            ProgressIndicatorController.On(this);
            MenuItem menuItem = (sender as MenuItem);
            LunchCard card = ((VisualTreeHelper.GetParent(menuItem) as FrameworkElement).DataContext as LunchCard);
            card.update();
        }

        private void Menu_Loaded(object sender, RoutedEventArgs e)
        {
            ListBox listBox = (sender as ListBox);
            listBox.SelectedIndex = -1;
        }

        private void ApplicationBarIconButtonAdd_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/AddPage.xaml", UriKind.Relative));
        }

        private void ApplicationBarIconButtonUpdate_Click(object sender, EventArgs e)
        {
            ProgressIndicatorController.On(this);
            vm.UpdateAll();
        }

        private void ApplicationBarItemRunInBg_Click(object sender, EventArgs e)
        {
            taskManager.toggleState();
            TaskMenuItem_Text((ApplicationBarMenuItem)sender, taskManager.isEnabled());
        }

        private void TaskMenuItem_Text(ApplicationBarMenuItem item, bool isEnabled)
        {
            item.Text = isEnabled ? "Disable task" : "Run in background";
        }

        private void ApplicationBarItemLicense_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Provided under MIT license\n" +
                "github.com/edghto/LunchMoneyApp\n\n" +
                "Powered by Json.NET under MIT License\n" +
                "json.codeplex.com"
            );
        }
    }
}