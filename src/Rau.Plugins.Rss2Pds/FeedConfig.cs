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

namespace Rau.Plugins.Rss2Pds
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="FeedUrl">The feed URL.</param>
    /// <param name="Handle">The handle to post RSS messages to.</param>
    /// <param name="Password">The app password to use.</param>
    /// <param name="CronString">How often to check for RSS updates.</param>
    /// <param name="HashTags">HashTags to add to the end of the message.</param>
    /// <param name="AlertThreshold">
    /// How many failed scrapes in a row must happen
    /// before an error message is logged.
    /// </param>
    public sealed record class FeedConfig(
        Uri FeedUrl,
        string Handle,
        string Password,
        string CronString,
        IEnumerable<string>? HashTags,
        uint? AlertThreshold
    );
}