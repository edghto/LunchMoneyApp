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
using Microsoft.Phone.Shell;

namespace LunchMoneyApp
{
    public class ProgressIndicatorController
    {
        private static object _obj = null;

        public static void On(DependencyObject obj, string progressString = null)
        {
            _obj = obj;
            SystemTray.SetProgressIndicator(obj, new ProgressIndicator
            {
                IsVisible = true,
                IsIndeterminate = true,
                Text = progressString
            });
        }

        public static void Off()
        {
            if (_obj != null)
                SystemTray.SetProgressIndicator(_obj as DependencyObject, null);
        }
    }
}
