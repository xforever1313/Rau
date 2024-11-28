//
// Rss2Pds - A bot that reads RSS feeds and posts them to a AT-Proto PDS node
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

using System.Reflection;

namespace Rss2Pds
{
    public sealed class Rss2PdsConfig
    {
        // ---------------- Fields ----------------

        private readonly List<RssFeedConfig> feeds;

        // ---------------- Constructor ----------------

        internal Rss2PdsConfig( Version? assemblyVersion )
        {
            this.feeds = new List<RssFeedConfig>();

            this.ApplicationContext = "Rss2Pds";
            this.CharacterLimit = 300;
            this.Feeds = this.feeds.AsReadOnly();
            this.SchemaVersion = new Version( 1, 0, 0 );
            this.PdsUrl = new Uri( "https://bsky.social" );
            this.UserAgentName = "rss_2_pds";
            this.UserAgentVersion = assemblyVersion;
        }

        // ---------------- Properties ----------------

        /// <summary>
        /// The context of this application.
        /// This is mainly used for logging in case the log has multiple
        /// applications writing to it.
        /// 
        /// Defaulted to "Rss2Pds".
        /// </summary>
        public string ApplicationContext { get; set; }

        /// <summary>
        /// The character limit of a post.
        /// Defaulted to Blue Sky's limit.
        /// </summary>
        public uint CharacterLimit { get; set; }

        public IReadOnlyList<RssFeedConfig> Feeds { get; }

        /// <summary>
        /// File to write log messages to.  Set to null
        /// or empty string to disable logging messages to a file.
        /// </summary>
        public FileInfo? LogFile { get; private set; }

        /// <summary>
        /// The port to expose for Prometheus metrics.
        /// Set to null to not enable metrics.
        /// </summary>
        public ushort? MetricsPort { get; set; }

        /// <summary>
        /// The schema version.
        /// </summary>
        public Version SchemaVersion { get; }

        /// <summary>
        /// The URL to the PDS that the bot will send messages to.
        /// Defaulted to the main Blue Sky PDS.
        /// </summary>
        public Uri PdsUrl { get; set; }

        public string? TelegramBotToken { get; private set; }

        public string? TelegramChatId { get; private set; }

        public string UserAgentName { get; private set; }

        public Version? UserAgentVersion { get; private set; }

        // ---------------- Methods ----------------

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
        public Rss2PdsConfig AddFeed(
            Uri feedUrl,
            string handle,
            string password,
            string cronString,
            IEnumerable<string>? hashTags = null,
            uint? alertThreshold = null
        )
        {
            return AddFeed(
                new RssFeedConfig( feedUrl, handle, password, cronString, hashTags, alertThreshold )
            );
        }

        public Rss2PdsConfig AddFeed( RssFeedConfig feedConfig )
        {
            return this;
        }

        public Rss2PdsConfig LogToFile( string filePath )
        {
            return LogToFile( new FileInfo( filePath ) );
        }

        public Rss2PdsConfig LogToFile( FileInfo filePath )
        {
            this.LogFile = filePath;
            return this;
        }

        /// <summary>
        /// Call this method to enable sending warning or higher
        /// messages to a Telegram channel.
        /// </summary>
        /// <param name="telegramBotToken">
        /// The Telegram bot token to use.  See more information
        /// on how to do that:
        /// https://docs.teleirc.com/en/latest/user/quick-start/#create-a-telegram-bot
        /// </param>
        /// <param name="telegramChatId">
        /// The chat ID of the Telegram chat.
        /// </param>
        public Rss2PdsConfig LogMessageToTelegram(
            string telegramBotToken,
            string telegramChatId
        )
        {
            this.TelegramBotToken = telegramBotToken;
            this.TelegramChatId = telegramChatId;

            return this;
        }

        /// <summary>
        /// Overrides the default user agent.
        /// </summary>
        /// <param name="userAgentName">The agent name.</param>
        /// <param name="userAgentVersion">
        /// The version of the agent.  Pass in null to not specify a version.
        /// </param>
        public Rss2PdsConfig UseUserAgent( string userAgentName, Version? userAgentVersion )
        {
            this.UserAgentName = userAgentName;
            this.UserAgentVersion = userAgentVersion;

            return this;
        }

        internal List<string> TryValidate()
        {
            var errors = new List<string>();
            

            return errors;
        }
    }
}
