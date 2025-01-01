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

namespace Rau.Plugins.Rss2Pds
{
    internal interface IFeedReader
    {
        public FeedConfig FeedConfig { get; }

        /// <summary>
        /// The cached title of the feed as of the last update.
        /// </summary>
        public string FeedTitle { get; }

        /// <summary>
        /// The language of the feed as of the last update.
        /// </summary>
        public string? FeedLanguage { get; }
    }
}
