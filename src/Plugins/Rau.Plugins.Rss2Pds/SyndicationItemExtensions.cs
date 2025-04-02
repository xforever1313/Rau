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
            if( item.Links.Count > 0 )
            {
                url = item.Links.First().Uri;
            }

            string titlePortion;
            if( feedReader.FeedConfig.IncludeFeedTitleInPost )
            {
                titlePortion = $"{feedReader.FeedTitle}: ";
            }
            else
            {
                titlePortion = string.Empty;
            }

            var postContents = new StringBuilder();

            bool firstHastag = true;
            foreach( string hashTag in feedReader.FeedConfig.HashTags ?? [] )
            {
                if( firstHastag )
                {
                    postContents.AppendLine();
                    postContents.AppendLine();
                    postContents.Append( "#" + hashTag );

                    firstHastag = false;
                }
                else
                {
                    postContents.Append( " #" + hashTag );
                }
            }

            long charactersRemaining = rauConfig.CharacterLimit - postContents.Length;
            if( charactersRemaining <= 0 )
            {
                // If there are too many hashtags, clear them out.
                // The contents of the feed are more important.
                postContents.Clear();
                charactersRemaining = rauConfig.CharacterLimit;
            }
            
            if( ( titlePortion.Length + item.Title.Text.Length ) > charactersRemaining )
            {
                // If there isn't enough characters for the title, clear them out.
                // Item title is most important.
                titlePortion = "";
            }

            string message = titlePortion + item.Title.Text;

            for(
                int i = 0;
                ( i < message.Length ) && ( postContents.Length < rauConfig.CharacterLimit );
                ++i
            )
            {
                postContents.Insert( i, message[i] );
            }

            return new PdsPost
            {
                PostAttachmentPage = url,
                PostContents = postContents.ToString(),
                Languages = GetLanguages( feedReader, rauConfig ),
                GenerateCardForUrl = feedReader.FeedConfig.GenerateCardForUrl
            };
        }

        private static IEnumerable<string> GetLanguages( IFeedReader feedReader, RauConfig config )
        {
            if( feedReader.FeedConfig.Languages is null )
            {
                if( string.IsNullOrWhiteSpace( feedReader.FeedLanguage ) )
                {
                    return config.GetDefaultLanguages();
                }
                else
                {
                    return [feedReader.FeedLanguage];
                }
            }
            else
            {
                return feedReader.FeedConfig.Languages;
            }
        }
    }
}
