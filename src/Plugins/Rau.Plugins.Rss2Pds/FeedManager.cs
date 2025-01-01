//
// Rau - An AT-Proto Bot Framework
// Copyright (C) 2024-2025 Seth Hendrick
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

using Rau.Standard;
using Rau.Standard.EventScheduler;

namespace Rau.Plugins.Rss2Pds
{
    public sealed class FeedManager
    {
        // ---------------- Fields ----------------

        private readonly Dictionary<int, FeedReader> feeds;

        private readonly HttpClient httpClient;

        private readonly IRauApi api;

        // ---------------- Constructor ----------------

        public FeedManager( HttpClient client, IRauApi api )
        {
            this.feeds = new Dictionary<int, FeedReader>();

            this.httpClient = client;
            this.api = api;
        }

        // ---------------- Methods ----------------

        public int AddFeed( FeedConfig feed )
        {
            var feedReader = new FeedReader(
                this.httpClient,
                feed
            );

            var e = new FeedUpdateEvent( feedReader );
            if( feed.InitializeOnStartUp )
            {
                var args = new ScheduledEventArgs
                {
                    Api = api,
                    CancellationToken = CancellationToken.None,
                    FireTimeUtc = this.api.DateTime.UtcNow
                };
                e.ExecuteEvent( args ).Wait();
            }

            int feedId = this.api.EventScheduler.ConfigureEvent( e );

            this.feeds.Add( feedId, feedReader );

            return feedId;
        }

        public void RemoveFeed( int feedId )
        {
            if( this.feeds.ContainsKey( feedId ) )
            {
                this.api.EventScheduler.RemoveEvent( feedId );
                this.feeds.Remove( feedId );
            }
        }

        internal void RemoveAllFeeds()
        {
            foreach( int feedId in this.feeds.Keys.ToArray() )
            {
                RemoveFeed( feedId );
            }
        }

        // ----------------- Helper Classes ----------------

        private record class ScheduledEventArgs : IScheduledEventParameters
        {
            public required IRauApi Api { get; init; }

            public required DateTimeOffset FireTimeUtc { get; init; }

            public required CancellationToken CancellationToken { get; init; }
        }
    }
}
