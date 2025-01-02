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
        /// <param name="userName">
        /// The AT-proto userName to post the RSS feeds to.
        /// </param>
        /// <param name="password">
        /// The password to the userName to login.
        /// App passwords are the recommended approach instead of
        /// using the main account password.
        /// </param>
        /// <param name="pdsUrl">
        /// The PDS instance to send messages to.
        /// </param>
        /// <param name="cronString">
        /// How often to scrape the RSS feeds in the form of a cron string
        /// as described here:
        /// https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontriggers.html#cron-expressions
        /// </param>
        /// <param name="languages">
        /// The languages that we should post in.  If set to null,
        /// the default behavior is to used, which is to use the feed's language first,
        /// falling back to the default languages specified in
        /// <see cref="Standard.Configuration.RauConfig.DefaultLanguages"/> if the feed
        /// does not specify a language.
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
        /// <param name="includeFeedTitleInPost">
        /// If set to true, the title of the feed will be prefixed before the
        /// item title in the post.  If false, it is not included.
        /// 
        /// A use case for setting to true is if an account has multiple RSS
        /// feeds it mirrors, it can be used to determine which feed it came from.
        /// </param>
        /// <param name="initializeOnStartUp">
        /// If set to true, the initial cache of the feed is downloaded
        /// when the plugin is initialized.
        /// 
        /// If set to false, it means the first time the cron string elapses,
        /// no posts will be sent to the PDS 
        /// as the feed cache must be initialzied first.
        /// 
        /// A good rule of thumb is to set to true if a startup penality is 
        /// not a big deal, and the feed isn't updated that often.  Set to false
        /// if the feed is updated fairly often, or you do not want a startup penalty.
        /// </param>
        /// <returns>
        /// An ID that is associated with this feed.
        /// Use this to remove the feed.
        /// </returns>
        public static int MirrorRssFeed(
            this IRauApi api,
            string feedUrl,
            string userName,
            string password,
            string pdsUrl,
            string cronString,
            IEnumerable<string>? hashTags = null,
            uint? alertThreshold = null,
            bool includeFeedTitleInPost = false,
            IEnumerable<string>? languages = null,
            bool initializeOnStartUp = true
        )
        {
            return api.MirrorRssFeed(
                new Uri( feedUrl ),
                userName,
                password,
                new Uri( pdsUrl ),
                cronString,
                hashTags,
                alertThreshold,
                includeFeedTitleInPost,
                languages,
                initializeOnStartUp
            );
        }

        /// <summary>
        /// Adds a feed to watch.
        /// </summary>
        /// <param name="feedUrl">
        /// The RSS feed to scrape.
        /// </param>
        /// <param name="userName">
        /// The AT-proto userName to post the RSS feeds to.
        /// </param>
        /// <param name="password">
        /// The password to the userName to login.
        /// App passwords are the recommended approach instead of
        /// using the main account password.
        /// </param>
        /// <param name="pdsUrl">
        /// The PDS instance to send messages to.
        /// </param>
        /// <param name="cronString">
        /// How often to scrape the RSS feeds in the form of a cron string
        /// as described here:
        /// https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontriggers.html#cron-expressions
        /// </param>
        /// <param name="languages">
        /// The languages that we should post in.  If set to null,
        /// the default behavior is to used, which is to use the feed's language first,
        /// falling back to the default languages specified in
        /// <see cref="Standard.Configuration.RauConfig.DefaultLanguages"/> if the feed
        /// does not specify a language.
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
        /// <param name="includeFeedTitleInPost">
        /// If set to true, the title of the feed will be prefixed before the
        /// item title in the post.  If false, it is not included.
        /// 
        /// A use case for setting to true is if an account has multiple RSS
        /// feeds it mirrors, it can be used to determine which feed it came from.
        /// </param>
        /// <param name="initializeOnStartUp">
        /// If set to true, the initial cache of the feed is downloaded
        /// when the plugin is initialized.
        /// 
        /// If set to false, it means the first time the cron string elapses,
        /// no posts will be sent to the PDS 
        /// as the feed cache must be initialzied first.
        /// 
        /// A good rule of thumb is to set to true if a startup penality is 
        /// not a big deal, and the feed isn't updated that often.  Set to false
        /// if the feed is updated fairly often, or you do not want a startup penalty.
        /// </param>
        /// <returns>
        /// An ID that is associated with this feed.
        /// Use this to remove the feed.
        /// </returns>
        public static int MirrorRssFeed(
            this IRauApi api,
            Uri feedUrl,
            string userName,
            string password,
            Uri pdsUrl,
            string cronString,
            IEnumerable<string>? hashTags = null,
            uint? alertThreshold = null,
            bool includeFeedTitleInPost = false,
            IEnumerable<string>? languages = null,
            bool initializeOnStartUp = true
        )
        {
            return api.MirrorRssFeed(
                new FeedConfig
                {
                    AlertThreshold = alertThreshold,
                    CronString = cronString,
                    FeedUrl = feedUrl,
                    HashTags = hashTags,
                    IncludeFeedTitleInPost = includeFeedTitleInPost,
                    InitializeOnStartUp = initializeOnStartUp,
                    Languages = languages,
                    Password = password,
                    PdsInstanceUrl = pdsUrl,
                    UserName = userName
                }
            );
        }

        public static int MirrorRssFeed( this IRauApi api, FeedConfig feedConfig )
        {
            return api.GetRss2PdsPlugin().FeedManager.AddFeed( feedConfig );
        }
    }
}
