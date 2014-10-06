using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace LunchMoneyApp
{

    public class LastCheckedStackPanel : StackPanel
    {
        public static readonly DependencyProperty LastCheckedProperty =
            DependencyProperty.RegisterAttached("LastChecked",
             typeof(string),
             typeof(LastCheckedStackPanel),
             new PropertyMetadata("never", OnFooChanged));


        public static void SetLastChecked(DependencyObject obj, string value)
        {
            obj.SetValue(LastCheckedProperty, value);
        }

        public static string GetLastChecked(DependencyObject obj)
        {
            return (string)obj.GetValue(LastCheckedProperty);
        } 

        static void OnFooChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) 
        {
            TextBlock lastCheckedTextBlock = obj as TextBlock;
            TextBlock helperStatusTextBlock = (TextBlock)
                ((VisualTreeHelper.GetParent(lastCheckedTextBlock) as LastCheckedStackPanel).FindName("HelperStatus"));
            lastCheckedTextBlock.Text = (string) args.NewValue;
            if (lastCheckedTextBlock.Text == "Error" ||
                lastCheckedTextBlock.Text == "Never")
                helperStatusTextBlock.Text = "";
            else
                helperStatusTextBlock.Text = "ago";
         } 
    }

    public partial class MainPage : PhoneApplicationPage
    {
        private LunchCardViewModel vm;

        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            StateUtility.IsLaunching = true;
        }

        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            StateUtility.IsLaunching = false;
        }

        public MainPage()
        {
            InitializeComponent();
            vm = new LunchCardViewModel();
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

            if (App.lunchCard != null)
            {
                vm.Add(App.lunchCard);
                App.lunchCard = null;
            }

            LunchCardList.DataContext = vm.LunchCards;
            vm.RefreshLastChecked();
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
                LunchCard card = (LunchCardList.SelectedItem as LunchCard);
                card.update();
                (sender as ListBox).SelectedIndex = -1;

            }
        }

        private void ContextMenuButtonDelete_Click(object sender, EventArgs e)
        {
            MenuItem menuItem = (sender as MenuItem);
            LunchCard card = ((VisualTreeHelper.GetParent(menuItem) as FrameworkElement).DataContext as LunchCard);
            vm.Del(card);
        }

        private void ConextMenuButtonUpdate_Click(object sender, EventArgs e)
        {
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
            vm.UpdateAll();
        }
    }
}