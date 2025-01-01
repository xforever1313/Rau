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
    /// Represents an image that is attached to a post.
    /// </summary>
    public record class PostImage
    {
        // ---------------- Fields ----------------

        private static readonly Dictionary<string, string> supportedMimeTypes = new Dictionary<string, string>
        {
            ["apng"] = "image/apng",
            ["avif"] = "image/avif",
            ["bmp"] = "image/bmp",
            ["gif"] = "image/gif",
            ["ico"] = "image/vnd.microsoft.icon",
            ["jpg"] = "image/jpeg",
            ["jpeg"] = "image/jpeg",
            ["png"] = "image/png",
            ["svg"] = "image/svg+xml",
            ["tif"] = "image/tiff",
            ["tiff"] = "image/tiff",
            ["webp"] = "image/webp",
        };

        // ---------------- Constructor ----------------

        /// <summary>
        /// Tries to read in the image from the given file.
        /// It will then try to determine the mime type based
        /// on the file extension and return an <see cref="PostImage"> object.
        /// </summary>
        public static PostImage FromFile( string fileName, string altText )
        {
            return FromFile( new FileInfo( fileName ), altText );
        }

        /// <summary>
        /// Tries to read in the image from the given file.
        /// It will then try to determine the mime type based
        /// on the file extension and return an <see cref="PostImage"> object.
        /// </summary>
        public static PostImage FromFile( FileInfo fileName, string altText )
        {
            string extension = fileName.Extension.ToLower().TrimStart( '.' );

            if( supportedMimeTypes.TryGetValue( extension, out string? mimeType ) )
            {
                return new PostImage(
                    File.ReadAllBytes( fileName.FullName ),
                    mimeType,
                    altText
                );
            }
            else
            {
                throw new ArgumentException(
                    $"Given filename's extension of {extension} is not a known mime type.",
                    nameof( fileName )
                );
            }
        }

        /// <summary>
        /// Tries to read in the image from the given file.
        /// It will then try to determine the mime type based
        /// on the file extension and return an <see cref="PostImage"> object.
        /// </summary>
        public static async Task<PostImage> FromFileAsync( string fileName, string altText )
        {
            return await FromFileAsync( new FileInfo( fileName ), altText );
        }

        /// <summary>
        /// Tries to read in the image from the given file.
        /// It will then try to determine the mime type based
        /// on the file extension and return an <see cref="PostImage"> object.
        /// </summary>
        public static async Task<PostImage> FromFileAsync( FileInfo fileName, string altText )
        {
            string extension = fileName.Extension.ToLower().TrimStart( '.' );

            if( supportedMimeTypes.TryGetValue( extension, out string? mimeType ) )
            {
                return new PostImage(
                    await File.ReadAllBytesAsync( fileName.FullName ),
                    mimeType,
                    altText
                );
            }
            else
            {
                throw new ArgumentException(
                    $"Given filename's extension of {extension} is not a known mime type.",
                    nameof( fileName )
                );
            }
        }

        public PostImage( byte[] imageContents, string mimeType, string altText )
        {
            this.ImageContents = imageContents;
            this.MimeType = mimeType;
            this.AltText = altText;
        }

        // ---------------- Properties ----------------

        public byte[] ImageContents { get; }

        public string MimeType { get; }

        public string AltText { get; }
    }
}
