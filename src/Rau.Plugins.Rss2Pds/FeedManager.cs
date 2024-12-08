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

using Rau.Standard;

namespace Rau.Plugins.Rss2Pds
{
    public sealed class FeedManager
    {
        // ---------------- Fields ----------------

        private readonly Dictionary<int, FeedConfig> feeds;

        private readonly IRauApi api;

        // ---------------- Constructor ----------------

        public FeedManager( IRauApi api )
        {
            this.feeds = new Dictionary<int, FeedConfig>();

            this.api = api;
        }

        // ---------------- Methods ----------------

        public int AddFeed( FeedConfig feed )
        {
            return 0;
        }

        public void RemoveFeed( int feedId )
        {
            if( this.feeds.ContainsKey( feedId ) )
            {
                this.feeds.Remove( feedId );
            }
        }
    }
}
