//
// Rau - A bot that reads RSS feeds and posts them to a AT-Proto PDS node
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

namespace Rau.Standard
{
    /// <summary>
    /// A post that will be posted to the PDS and eventually Blue Sky.
    /// </summary>
    public record class PdsPost
    {
        // ---------------- Constructor ----------------

        /// <summary>
        /// Creates a post with no attachment page.
        /// </summary>
        public PdsPost( string postContents ) :
            this( postContents, (Uri?)null )
        {
        }

        /// <summary>
        /// Creates a post with post contents and an attachment page URL
        /// in the form of a string.
        /// </summary>
        public PdsPost( string postContents, string attachmentUrl ) :
            this( postContents, new Uri( attachmentUrl ) )
        {
        }

        /// <summary>
        /// Creates a post with post contents and an attachment URL.
        /// </summary>
        /// <param name="url">
        /// Url of attachment page.  Set to null to have none.
        /// </param>
        public PdsPost( string postContents, Uri? url )
        {
            this.PostContents = postContents;
            this.PostAttachmentPage = url;
        }

        // ---------------- Properties ----------------

        public string PostContents { get; }

        public Uri? PostAttachmentPage { get; }
    }
}
