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

using System.Collections.Immutable;
using System.ServiceModel.Syndication;
using System.Text;
using Rau.Standard;
using Rau.Standard.Configuration;

namespace Rau.Plugins.Rss2Pds
{
    internal static class SyndicationItemExtensions
    {
        public static PdsPost GeneratePost(
            this SyndicationItem item,
            IFeedReader feedReader,
            RauConfig rauConfig
        )
        {
            Uri? url = null;
            string message;
            if( item.Links.Count > 0 )
            {
                url = item.Links.First().Uri;
                message = $"{feedReader.FeedTitle}: '{item.Title.Text}'{Environment.NewLine}{url}";
            }
            else
            {
                message = $"{feedReader.FeedTitle}: '{item.Title.Text}'";
            }

            var builder = new StringBuilder();
            foreach( string hashTag in feedReader.FeedConfig.HashTags ?? [] )
            {
                builder.Append( " " + hashTag );
            }

            for(
                int i = 0;
                ( i < message.Length ) && ( builder.Length < rauConfig.CharacterLimit );
                ++i
            )
            {
                builder.Insert( i, message[i] );
            }

            return new PdsPost
            {
                PostAttachmentPage = url,
                PostContents = message,
                Languages = GetLanguages( feedReader.FeedLanguage, rauConfig )
            };
        }

        private static IReadOnlyCollection<string> GetLanguages( string? feedLanguage, RauConfig config )
        {
            if( string.IsNullOrWhiteSpace( feedLanguage ) )
            {
                return config.GetDefaultLanguages();
            }
            else
            {
                return ImmutableArray.Create( feedLanguage );
            }
        }
    }
}
