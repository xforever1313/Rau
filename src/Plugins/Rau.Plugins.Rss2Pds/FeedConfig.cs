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
    /// <param name="FeedUrl">The feed URL.</param>
    /// <param name="Handle">The PDS handle to post RSS messages to.</param>
    /// <param name="Password">The app password to use.</param>
    /// <param name="pdsInstanceUrl">
    /// The instance of the PDS to login to.
    /// </param>
    /// <param name="CronString">How often to check for RSS updates.</param>
    /// <param name="HashTags">
    /// HashTags to add to the end of the message.
    /// Note, hashtags will be counted against the post's character limit.
    /// Adding too many will make posts be cut-off to make room.
    /// 
    /// Do *NOT* include the '#' in front, or the hashtag will end up starting with "##".
    /// In otherwords, the '#' is added automatically.
    /// </param>
    /// <param name="Languages">
    /// The languages that we should post in.  If set to null,
    /// the default behavior is to used, which is to use the feed's language first,
    /// falling back to the default languages specified in
    /// <see cref="Standard.Configuration.RauConfig.DefaultLanguages"/> if the feed
    /// does not specify a language.
    /// </param>
    /// <param name="AlertThreshold">
    /// How many failed scrapes in a row must happen
    /// before an error message is logged.
    /// </param>
    /// <param name="InitializeOnStartUp">
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
    public sealed record class FeedConfig(
        Uri FeedUrl,
        string Handle,
        string Password,
        Uri PdsInstanceUrl,
        string CronString,
        IEnumerable<string>? HashTags,
        uint? AlertThreshold,
        IEnumerable<string>? Languages = null,
        bool InitializeOnStartUp = true
    );

    internal static class FeedConfigExtensions
    {
        public static PdsAccount ToPdsAccount( this FeedConfig FeedConfig )
        {
            return new PdsAccount
            {
                Instance = FeedConfig.PdsInstanceUrl,
                Password = FeedConfig.Password,
                UserName = FeedConfig.Handle
            };
        }
    }
}