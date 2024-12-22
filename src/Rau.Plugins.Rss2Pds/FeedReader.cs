//
// Rau - An AT-Proto Bot Framework
// Copyright (C) 2024 Seth Hendrick
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//

using System.ServiceModel.Syndication;
using System.Xml;

namespace Rau.Plugins.Rss2Pds
{
    internal sealed class FeedReader : IFeedReader
    {
        // ---------------- Fields ----------------

        private readonly HttpClient client;

        private readonly object feedLock;

        /// <summary>
        /// Cache of the latest updatedFeed.  Null is the updatedFeed
        /// has not been initialized yet.
        /// </summary>
        private SyndicationFeed? feedCache;

        // ---------------- Constructor ----------------

        public FeedReader( HttpClient httpClient, FeedConfig config )
        {
            this.client = httpClient;
            this.feedLock = new object();
            this.FeedTitle = "Untitled Feed";

            this.FeedConfig = config;
        }

        // ---------------- Properties ----------------

        public FeedConfig FeedConfig { get; }

        public string FeedTitle { get; private set; }

        public string? FeedLanguage { get; private set; }

        // ---------------- Methods ----------------

        /// <summary>
        /// Performs the initial filling in of the cache 
        /// </summary>
        public void Initialize()
        {
            SyndicationFeed feed = this.FetchFeedAsync().Result;
            SortFeeds( feed );
        }

        /// <summary>
        /// Updates the RSS updatedFeed and fires the OnUpdate event if there's an update.
        /// </summary>
        /// <returns>
        /// A list of items that have changed since the last update.
        /// Will NOT be null.  Will return empty list if there are none.
        /// 
        /// Items are sorted by last updated time, with the oldest being in
        /// index 0.
        /// </returns>
        public async Task<List<SyndicationItem>> UpdateAsync()
        {
            SyndicationFeed updatedFeed = await FetchFeedAsync();
            return SortFeeds( updatedFeed );
        }

        private async Task<SyndicationFeed> FetchFeedAsync()
        {
            HttpResponseMessage response = await this.client.GetAsync( this.FeedConfig.FeedUrl );
            response.EnsureSuccessStatusCode();

            using( Stream content = await response.Content.ReadAsStreamAsync() )
            {
                using( XmlReader xmlReader = XmlReader.Create( content ) )
                {
                    return SyndicationFeed.Load( xmlReader );
                }
            }
        }

        private List<SyndicationItem> SortFeeds( SyndicationFeed latestFeed )
        {
            List<SyndicationItem> newItems = new List<SyndicationItem>();

            // this.updatedFeed can be modified by multiple threads if UpdateAsync() is called multiple times...
            // lock it up.
            lock( this.feedLock )
            {
                if( this.feedCache is null )
                {
                    this.feedCache = latestFeed;
                }
                else
                {
                    // If our updatedFeed is not null, then we have at least 1 update.
                    foreach( SyndicationItem item in latestFeed.Items )
                    {
                        if( this.feedCache.Items.FirstOrDefault( i => i.Id == item.Id ) == null )
                        {
                            newItems.Add( item );
                        }
                    }
                    this.feedCache = latestFeed;
                }

                this.FeedTitle = this.feedCache.Title.Text;
                this.FeedLanguage = this.feedCache.Language;
            }

            newItems.Sort( SortByDate );
            return newItems;
        }

        private static int SortByDate( SyndicationItem item1, SyndicationItem item2 )
        {
            return item1.LastUpdatedTime.CompareTo( item2.LastUpdatedTime );
        }
    }
}
