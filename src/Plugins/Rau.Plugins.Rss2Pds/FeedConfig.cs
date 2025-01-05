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
    /// <summary>
    /// A feed that the bot will scrape and post updates to the given
    /// handle.
    /// </summary>
    public sealed record class FeedConfig
    {
        /// <summary>
        /// The URL of the feed to mirror.
        /// </summary>
        public required Uri FeedUrl { get; init; }

        /// <summary>
        /// The username or handle of the PDS account that will
        /// perform the mirroring.
        /// </summary>
        public required string UserName { get; init; }

        /// <summary>
        /// The password (prefer to be an app password) of the PDS
        /// account that will perform the mirroring.
        /// </summary>
        public required string Password { get; init; }

        /// <summary>
        /// The PDS instance to post to.
        /// </summary>
        public required Uri PdsInstanceUrl { get; init; }

        /// <summary>
        /// Cron string of how often to check the RSS feeds for updates.
        /// 
        /// See https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontriggers.html#cron-expressions
        /// for the cron string format.
        /// </summary>
        public required string CronString { get; init; }

        /// <summary>
        /// HashTags to add to the end of the message.
        /// Note, hashtags will be counted against the post's character limit.
        /// Adding too many will make posts be cut-off to make room.
        /// 
        /// Do *NOT* include the '#' in front, or the hashtag will end up starting with "##".
        /// In otherwords, the '#' is added automatically.
        /// </summary>
        public IEnumerable<string>? HashTags { get; init; } = null;

        /// <summary>
        /// How many failed scrapes in a row must happen
        /// before an error message is logged.
        /// 
        /// Set to null to never send an alert on a failed scrape.
        /// </summary>
        public uint? AlertThreshold { get; init; } = null;

        /// The languages that we should post in.  If set to null,
        /// the default behavior is to used, which is to use the feed's language first,
        /// falling back to the default languages specified in
        /// <see cref="Standard.Configuration.RauConfig.DefaultLanguages"/> if the feed
        /// does not specify a language.
        public IEnumerable<string>? Languages { get; init; } = null;

        /// <summary>
        /// If set to true, the title of the feed will be prefixed before the
        /// item title in the post.  If false, it is not included.
        /// 
        /// A use case for setting to true is if an account has multiple RSS
        /// feeds it mirrors, it can be used to determine which feed it came from.
        /// </summary>
        public bool IncludeFeedTitleInPost { get; init; } = false;

        /// <summary>
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
        /// </summary>
        public bool InitializeOnStartUp { get; init; } = true;
    }

    internal static class FeedConfigExtensions
    {
        public static PdsAccount ToPdsAccount( this FeedConfig FeedConfig )
        {
            return new PdsAccount
            {
                Instance = FeedConfig.PdsInstanceUrl,
                Password = FeedConfig.Password,
                UserName = FeedConfig.UserName
            };
        }
    }
}