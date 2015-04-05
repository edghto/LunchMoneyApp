﻿using System;
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
using Microsoft.Phone.Scheduler;

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

        //TODO rename this handler foo seems to be inappropriate 
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
            string taskName = Config.BALANCE_CHANGE_POLL_AGENT_NAME;

            // Remove old task
            if (null != (ScheduledActionService.Find(taskName) as PeriodicTask))
            {
                ScheduledActionService.Remove(taskName);
            }

            PeriodicTask task = new PeriodicTask(taskName);
            task.Description = "Retreives balance of your lunch card";
            ScheduledActionService.Add(task);
#if DEBUG
            ScheduledActionService.LaunchForTest(taskName,
                    TimeSpan.FromMilliseconds(1500));
#endif
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