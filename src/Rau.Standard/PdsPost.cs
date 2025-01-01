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

namespace Rau.Standard
{
    /// <summary>
    /// A post that will be posted to the PDS and eventually Blue Sky.
    /// </summary>
    public record class PdsPost
    {
        // ---------------- Constructor ----------------

        /// <summary>
        /// Default constructor.
        /// 
        /// Use the initialization syntax to set the properties
        /// if using this constructor.
        /// </summary>
        public PdsPost()
        {
        }

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
        public PdsPost( string postContents, Uri? url ) :
            this( postContents, url, (IEnumerable<PostImage>?)null )
        {
        }

        /// <summary>
        /// Creates a post with post contents and an attachment URL.
        /// </summary>
        /// <param name="image">
        /// The image to inlcude on the post.
        /// </param>
        public PdsPost( string postContents, PostImage image ) :
            this( postContents, null, [image] )
        {
        }

        /// <summary>
        /// Creates a post with post contents and an attachment URL.
        /// <param name="images">
        /// The image(s) to inlcude on the post.  Set to null to not include any.
        /// </param>
        public PdsPost( string postContents, IEnumerable<PostImage>? images ) :
            this( postContents, null, images )
        {
        }

        /// <summary>
        /// Creates a post with post contents and an attachment URL.
        /// </summary>
        /// <param name="url">
        /// Url of attachment page.  Set to null to have none.
        /// </param>
        /// <param name="image">
        /// The image to inlcude on the post.
        /// </param>
        public PdsPost( string postContents, Uri? url, PostImage image ) :
            this( postContents, url, [image] )
        {
        }

        /// <summary>
        /// Creates a post with post contents and an attachment URL.
        /// </summary>
        /// <param name="url">
        /// Url of attachment page.  Set to null to have none.
        /// </param>
        /// <param name="images">
        /// The image(s) to inlcude on the post.  Set to null to not include any.
        /// </param>
        public PdsPost( string postContents, Uri? url, IEnumerable<PostImage>? images )
        {
            this.PostContents = postContents;
            this.PostAttachmentPage = url;
            this.PostImages = images;
        }

        // ---------------- Properties ----------------

        /// <summary>
        /// The text contents of the post.
        /// </summary>
        public string PostContents { get; init; } = "";

        /// <param name="url">
        /// Url of attachment page.  Set to null to have none.
        /// </param>
        public Uri? PostAttachmentPage { get; init; } = null;

        /// <summary>
        /// List of images to include on the post.
        /// Set to null if no imagees are desired.
        /// </summary>
        public IEnumerable<PostImage>? PostImages { get; init; } = null;

        /// <summary>
        /// The language(s) this post is posted in.
        /// Set to the default value if null,
        /// which is defaulted to en-US.
        /// </summary>
        public IEnumerable<string>? Languages { get; init; } = null;
    }
}
