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

namespace Rau
{
    public record class Rss2PdsConfig
    {
        // ---------------- Fields ----------------

        private readonly List<RssFeedConfig> feeds;

        // ---------------- Constructor ----------------

        protected Rss2PdsConfig( Version? assemblyVersion )
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
        /// Defaulted to "Rau".
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
            string feedUrl,
            string handle,
            string password,
            string cronString,
            IEnumerable<string>? hashTags = null,
            uint? alertThreshold = null
        )
        {
            return AddFeed(
                new RssFeedConfig( new Uri( feedUrl ), handle, password, cronString, hashTags, alertThreshold )
            );
        }

        public Rss2PdsConfig AddFeed( RssFeedConfig feedConfig )
        {
            this.feeds.Add( feedConfig );
            return this;
        }

#if false

        /// <summary>
        /// Loads a plugin from the given filepath.
        /// 
        /// This should be a C# assembly that contains non-abstract classes that implement
        /// the <see cref="IRss2PdsFeed"/> interface and are marked with the <see cref="RauPluginAttribute"/>
        /// attribute.
        /// </summary>
        /// <param name="assembly">The assembly that contains feed types.</param>
        public Rss2PdsConfig LoadPlugin( Assembly assembly )
        {
            var errors = new List<string>();

            foreach( Type type in assembly.GetTypes() )
            {
                RauPluginAttribute? feedType = type.GetCustomAttribute<RauPluginAttribute>();
                if( feedType is null )
                {
                    continue;
                }

                if( TryLoadFeedType( feedType.FeedTypeId, type, out string errorMessage ) == false )
                {
                    errors.Add( errorMessage );
                }
            }

            if( errors.Any() )
            {
                throw new ListedValidationException( $"Error loading assembly {assembly.FullName}", errors );
            }

            return this;
        }
#endif
        


        internal List<string> TryValidate()
        {
            var errors = new List<string>();
            

            return errors;
        }

#if false
        private bool TryLoadFeedType( string feedTypeId, Type type, out string errorMessage )
        {
            if( type.IsAbstract )
            {
                errorMessage = $"Type {type.FullName} is abstract, can not use as a feed type implementation.";
                return false;
            }
            else if( type.IsAssignableTo( typeof( IRss2PdsFeed ) ) )
            {
                errorMessage = $"Type {type.FullName} can not be assigned to the {nameof( IRss2PdsFeed )} interface.  Please ensure it implements this interface.";
                return false;
            }
            else if( this.feedTypes.ContainsKey( feedTypeId ) )
            {
                errorMessage = $"Feed type ID '{feedTypeId}' already exists.  Its possible another plugin loaded a feed type and used the same ID.";
                return false;
            }

            errorMessage = "";
            return true;
        }
#endif
    }
}
