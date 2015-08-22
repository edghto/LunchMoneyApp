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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Collections.Specialized;
using System.ComponentModel;

namespace LunchMoneyApp
{
    public class LunchCardViewModel
    {
        public ObservableCollection<LunchCard> LunchCards { get; set; }

        public int LoadLunchCards()
        {
            ObservableCollection<LunchCard> l = new ObservableCollection<LunchCard>();

            foreach (Object o in IsolatedStorageSettings.ApplicationSettings.Values)
            {
                ((LunchCard)o).ServerUrl = Config.SERVER_URL;
                l.Add((LunchCard)o);
            }

            LunchCards = l;

            RegisterForPropertyChangeEvent(new PropertyChangedEventHandler(OnCardPropertyChange));

            return l.Count;
        }




        #region Event stuff

        public void RegisterForPropertyChangeEvent(PropertyChangedEventHandler eventHandler)
        {
            IEnumerator<LunchCard> iter = LunchCards.GetEnumerator();
            while (iter.MoveNext())
            {
                ((LunchCard)iter.Current).PropertyChanged += eventHandler;
            }
        }

        public void UnregisterForPropertyChangeEvent(PropertyChangedEventHandler eventHandler)
        {
            IEnumerator<LunchCard> iter = LunchCards.GetEnumerator();
            while (iter.MoveNext())
            {
                ((LunchCard)iter.Current).PropertyChanged -= eventHandler;
            }
        }

        public void UnregisterForPropertyChangeEvent(PropertyChangedEventHandler eventHandler, LunchCard card)
        {
            card.PropertyChanged -= eventHandler;
        }


        private void OnCardPropertyChange(object sender, PropertyChangedEventArgs args)
        {
            Update((LunchCard)sender);
        }

        #endregion




        #region Called directly from UI

        public void UpdateAll()
        {
            IEnumerator<LunchCard> iter = LunchCards.GetEnumerator();
            while (iter.MoveNext())
            {
                ((LunchCard)iter.Current).update();
            }
        }

        public void RefreshLastChecked()
        {
            foreach (LunchCard card in LunchCards)
            {
                card.refreshLastCheckedProperty();
            }
        }

        #endregion




        #region IsolatedStorage methods

        public void Update(LunchCard card)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            settings[card.getCardHash()] = card.getCopy();
            settings.Save();
        }

        public void Add(LunchCard card)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            card.ServerUrl = Config.SERVER_URL;
            LunchCards.Add(card);
            settings.Add(card.getCardHash(), card.getCopy());
            card.PropertyChanged += new PropertyChangedEventHandler(OnCardPropertyChange);
        }

        public void Del(LunchCard card)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            LunchCards.Remove(card);
            settings.Remove(card.getCardHash());
        }

        #endregion
    }
}
