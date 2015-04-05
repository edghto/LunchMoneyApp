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

        public void LoadLunchCards()
        {
            ObservableCollection<LunchCard> l = new ObservableCollection<LunchCard>();

            foreach (Object o in IsolatedStorageSettings.ApplicationSettings.Values)
            {
                ((LunchCard)o).ServerUrl = Config.SERVER_URL;
                l.Add((LunchCard) o);
            }

            LunchCards = l;
        }

        public void UpdateAll()
        {
        }

        public void UpdateAll(PropertyChangedEventHandler eventHandler = null)
        {
            IEnumerator<LunchCard> iter = LunchCards.GetEnumerator();
            while (iter.MoveNext())
            {
                if (null != eventHandler)
                    (iter.Current as LunchCard).PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(eventHandler);
                (iter.Current as LunchCard).update();
            }
        }

        public void Add(LunchCard card)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            card.ServerUrl = Config.SERVER_URL;
            LunchCards.Add(card);
            settings.Add(GetCardHash(card), card.getCopy());
        }

        public void Del(LunchCard card)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            LunchCards.Remove(card);
            settings.Remove(GetCardHash(card));
        }

        private string GetCardHash(LunchCard card)
        {
            return card.Code + " " + card.CardNumber; ;
        }

        public void RefreshLastChecked()
        {
            foreach (LunchCard card in LunchCards)
            {
                card.refreshLastCheckedProperty();
            }
        }
    }
}
