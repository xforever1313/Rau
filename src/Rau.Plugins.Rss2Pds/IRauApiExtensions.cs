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
    public static class IRauApiExtensions
    {
        public static Rss2PdsPlugin GetRss2PdsPlugin( this IRauApi api )
        {
            return api.GetPlugin<Rss2PdsPlugin>( Rss2PdsPlugin.PluginGuid );
        }

        /// <summary>
        /// Adds a feed to watch.
        /// </summary>
        /// <param name="feedUrl">
        /// The RSS feed to scrape.
        /// </param>
        /// <param name="handle">
        /// The AT-proto handle to post the RSS feeds to.
        /// </param>
        /// <param name="password">
        /// The password to the handle to login.
        /// App passwords are the recommended approach instead of
        /// using the main account password.
        /// </param>
        /// <param name="cronString">
        /// How often to scrape the RSS feeds in the form of a cron string
        /// as described here:
        /// https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontriggers.html#cron-expressions
        /// </param>
        /// <param name="hashTags">
        /// Any hash tags you wish to include after the summary of the feed and link
        /// are posted.  Note, the more hash tags, the less of the summary is included
        /// to stay under the character limit.
        /// 
        /// Do not include any '#' in front of the strings,
        /// they will be included automatically.
        /// 
        /// Set to null or empty to not include any hash tags.
        /// </param>
        /// <param name="alertThreshold">
        /// How many failed scrapes in a row must occur before
        /// alerting an admin.  Set to null to not alert (default).
        /// </param>
        /// <returns>
        /// An ID that is associated with this feed.
        /// Use this to remove the feed.
        /// </returns>
        public static int AddFeed(
            this IRauApi api,
            string feedUrl,
            string handle,
            string password,
            string cronString,
            IEnumerable<string>? hashTags = null,
            uint? alertThreshold = null
        )
        {
            return api.AddFeed(
                new FeedConfig( new Uri( feedUrl ), handle, password, cronString, hashTags, alertThreshold )
            );
        }

        public static int AddFeed( this IRauApi api, FeedConfig feedConfig )
        {
            return api.GetRss2PdsPlugin().FeedManager.AddFeed( feedConfig );
        }
    }
}
