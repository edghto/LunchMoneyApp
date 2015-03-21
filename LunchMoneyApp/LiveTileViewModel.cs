using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Phone.Shell;

namespace LunchMoneyApp
{
    class LiveTileExists : Exception { }

    class LiveTileViewModel
    {
        private string tileUriPattern = "/MainPage.xaml?code={0}&cardNumber={1}&lang=pl";

        public ShellTile getTile(LunchCard card)
        {
            String uriString = string.Format(tileUriPattern, card.Code, card.CardNumber);
            return ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Equals(uriString));
        }

        public void create(LunchCard card)
        {
            ShellTile tile = getTile(card);
            if (null != tile)
            {
                throw new LiveTileExists();
            }
            else
            {
                String uri = string.Format(tileUriPattern, card.Code, card.CardNumber);
                ShellTileData data = prepareTileData(card);
                ShellTile.Create(new Uri(uri, UriKind.Relative), data);
            }
        }

        public void update(LunchCard card)
        {
            ShellTile tile = getTile(card);
            if (null != tile)
            {
                ShellTileData data = prepareTileData(card);
                tile.Update(data);
            }
        }

        private ShellTileData prepareTileData(LunchCard card)
        {
            StandardTileData data = new StandardTileData();
            data.Title = card.Balance.ToString();
            data.BackgroundImage = new Uri("/icons/tile.png", UriKind.Relative);

            return data;
        }
    }
}
