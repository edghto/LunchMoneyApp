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
            LunchCardList.DataContext = vm.LunchCards;
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

        private void Menu_Loaded(object sender, RoutedEventArgs e)
        {
            ListBox listBox = (sender as ListBox);
            listBox.SelectedIndex = -1;
        }

        private void ApplicationBarIconButtonAdd_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/AddView.xaml"));
        }
    }
}