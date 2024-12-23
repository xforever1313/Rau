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

using System.ServiceModel.Syndication;
using FakeItEasy;
using Rau.Plugins.Rss2Pds;
using Rau.Standard.Configuration;

namespace Rau.Tests.Plugins.Rss2Pds
{
    [TestClass]
    public sealed class SyndicationItemExtensionsTests
    {
        // ---------------- Fields ----------------

        private static readonly Version assemblyVersion =
            typeof( RauApi ).Assembly.GetName()?.Version ?? new Version( 0, 0, 0 );

        // ---------------- Tests ----------------

        [TestMethod]
        public void ItemWithNoLinkNoHashTagsWithLanguageTest()
        {
            // Setup
            var item = new SyndicationItem
            {
                Title = new TextSyndicationContent( "Some Title" )
            };

            var rauConfig = new RauConfig( assemblyVersion )
            {
                CharacterLimit = 300,
                // Feed uses en-US, so it should default to that.
                DefaultLanguages = ["en-ZW"]
            };

            var feedConfig = new FeedConfig(
                new Uri( "https://shendrick.net/rss.xml" ),
                "rssbot",
                "some password",
                new Uri( "https://at.shendrick.net" ),
                "0 0 * * * ?",
                [],
                5,
                null,
                false
            );

            IFeedReader feedReader = A.Fake<IFeedReader>();
            A.CallTo( () => feedReader.FeedTitle ).Returns( "Feed Title" );
            A.CallTo( () => feedReader.FeedConfig ).Returns( feedConfig );
            A.CallTo( () => feedReader.FeedLanguage ).Returns( "en-US" );

            var expectedPost = new PdsPost
            {
                PostContents = $"Feed Title: Some Title",
                Languages = ["en-US"]
            };

            // Act
            PdsPost actualPost = item.GeneratePost( feedReader, rauConfig );

            // Check
            AssertEx.ArePdsPostsEqual( expectedPost, actualPost );
        }

        [TestMethod]
        public void ItemWithLinkNoHashTagsWithLanguageTest()
        {
            // Setup
            const string url = "https://shendrick.net/Coding Tips/2023/06/18/winformsasyncawait.html";

            var item = new SyndicationItem
            {
                Title = new TextSyndicationContent( "Some Title" )
            };
            item.Links.Add( new SyndicationLink( new Uri( url ) ) );

            var rauConfig = new RauConfig( assemblyVersion )
            {
                CharacterLimit = 300,
                // Feed uses en-US, so it should default to that.
                DefaultLanguages = ["en-ZW"]
            };

            var feedConfig = new FeedConfig(
                new Uri( "https://shendrick.net/rss.xml" ),
                "rssbot",
                "some password",
                new Uri( "https://at.shendrick.net" ),
                "0 0 * * * ?",
                [],
                5,
                null,
                false
            );

            IFeedReader feedReader = A.Fake<IFeedReader>();
            A.CallTo( () => feedReader.FeedTitle ).Returns( "Feed Title" );
            A.CallTo( () => feedReader.FeedConfig ).Returns( feedConfig );
            A.CallTo( () => feedReader.FeedLanguage ).Returns( "en-US" );

            var expectedPost = new PdsPost
            {
                PostAttachmentPage = new Uri( url ),
                PostContents = $@"Feed Title: Some Title",
                Languages = ["en-US"]
            };

            // Act
            PdsPost actualPost = item.GeneratePost( feedReader, rauConfig );

            // Check
            AssertEx.ArePdsPostsEqual( expectedPost, actualPost );
        }

        [TestMethod]
        public void ItemWithLinkNoHashTagsNoLanguageTest()
        {
            // Setup
            const string url = "https://shendrick.net/Coding Tips/2023/06/18/winformsasyncawait.html";

            var item = new SyndicationItem
            {
                Title = new TextSyndicationContent( "Some Title" )
            };
            item.Links.Add( new SyndicationLink( new Uri( url ) ) );

            var rauConfig = new RauConfig( assemblyVersion )
            {
                CharacterLimit = 300,
                // Feed uses en-US, so it should default to that.
                DefaultLanguages = ["en-ZW"]
            };

            var feedConfig = new FeedConfig(
                new Uri( "https://shendrick.net/rss.xml" ),
                "rssbot",
                "some password",
                new Uri( "https://at.shendrick.net" ),
                "0 0 * * * ?",
                [],
                5,
                null,
                false
            );

            IFeedReader feedReader = A.Fake<IFeedReader>();
            A.CallTo( () => feedReader.FeedTitle ).Returns( "Feed Title" );
            A.CallTo( () => feedReader.FeedConfig ).Returns( feedConfig );

            // A null feed language on the feed should force a fall-back
            // to whatever is defined in the default Rau config.
            A.CallTo( () => feedReader.FeedLanguage ).Returns( null );

            var expectedPost = new PdsPost
            {
                PostAttachmentPage = new Uri( url ),
                PostContents = $@"Feed Title: Some Title",
                Languages = ["en-ZW"]
            };

            // Act
            PdsPost actualPost = item.GeneratePost( feedReader, rauConfig );

            // Check
            AssertEx.ArePdsPostsEqual( expectedPost, actualPost );
        }

        [TestMethod]
        public void ItemWithLinkWithHashTagsWithLanguageTest()
        {
            // Setup
            const string url = "https://shendrick.net/Coding Tips/2023/06/18/winformsasyncawait.html";

            var item = new SyndicationItem
            {
                Title = new TextSyndicationContent( "Some Title" )
            };
            item.Links.Add( new SyndicationLink( new Uri( url ) ) );

            var rauConfig = new RauConfig( assemblyVersion )
            {
                CharacterLimit = 300,
                // Feed uses en-US, so it should default to that.
                DefaultLanguages = ["en-ZW"]
            };

            var feedConfig = new FeedConfig(
                new Uri( "https://shendrick.net/rss.xml" ),
                "rssbot",
                "some password",
                new Uri( "https://at.shendrick.net" ),
                "0 0 * * * ?",
                ["seth", "xforever1313"],
                5,
                null,
                false
            );

            IFeedReader feedReader = A.Fake<IFeedReader>();
            A.CallTo( () => feedReader.FeedTitle ).Returns( "Feed Title" );
            A.CallTo( () => feedReader.FeedConfig ).Returns( feedConfig );
            A.CallTo( () => feedReader.FeedLanguage ).Returns( "en-US" );

            var expectedPost = new PdsPost
            {
                PostAttachmentPage = new Uri( url ),
                PostContents =
$@"Feed Title: Some Title

#seth #xforever1313",
                Languages = ["en-US"]
            };

            // Act
            PdsPost actualPost = item.GeneratePost( feedReader, rauConfig );

            // Check
            AssertEx.ArePdsPostsEqual( expectedPost, actualPost );
        }

        /// <summary>
        /// Tests to ensure if we're right on the character limit,
        /// we still post everything successfully.
        /// </summary>
        [TestMethod]
        public void RightOnCharacterLimitWithHashTagsNoUrlTest()
        {
            // Setup
            var item = new SyndicationItem
            {
                Title = new TextSyndicationContent( "1234567890" )
            };

            var rauConfig = new RauConfig( assemblyVersion )
            {
                CharacterLimit = 18 + ((uint)Environment.NewLine.Length * 2),
                // Feed uses en-US, so it should default to that.
                DefaultLanguages = ["en-ZW"]
            };

            var feedConfig = new FeedConfig(
                new Uri( "https://shendrick.net/rss.xml" ),
                "rssbot",
                "some password",
                new Uri( "https://at.shendrick.net" ),
                "0 0 * * * ?",
                ["1", "2"],
                5,
                null,
                false
            );

            IFeedReader feedReader = A.Fake<IFeedReader>();
            A.CallTo( () => feedReader.FeedTitle ).Returns( "1" );
            A.CallTo( () => feedReader.FeedConfig ).Returns( feedConfig );
            A.CallTo( () => feedReader.FeedLanguage ).Returns( "en-US" );

            var expectedPost = new PdsPost
            {
                PostContents =
$@"1: 1234567890

#1 #2",
                Languages = ["en-US"]
            };

            // Act
            PdsPost actualPost = item.GeneratePost( feedReader, rauConfig );

            // Check
            AssertEx.ArePdsPostsEqual( expectedPost, actualPost );
        }

        /// <summary>
        /// If the feed title makes the message too big, get rid of it.
        /// </summary>
        [TestMethod]
        public void OneLessthanCharacterLimitWithHashTagsNoUrlTest()
        {
            // Setup
            var item = new SyndicationItem
            {
                Title = new TextSyndicationContent( "1234567890" )
            };

            var rauConfig = new RauConfig( assemblyVersion )
            {
                CharacterLimit = 17 + ( (uint)Environment.NewLine.Length * 2 ),
                // Feed uses en-US, so it should default to that.
                DefaultLanguages = ["en-ZW"]
            };

            var feedConfig = new FeedConfig(
                new Uri( "https://shendrick.net/rss.xml" ),
                "rssbot",
                "some password",
                new Uri( "https://at.shendrick.net" ),
                "0 0 * * * ?",
                ["1", "2"],
                5,
                null,
                false
            );

            IFeedReader feedReader = A.Fake<IFeedReader>();
            A.CallTo( () => feedReader.FeedTitle ).Returns( "1" );
            A.CallTo( () => feedReader.FeedConfig ).Returns( feedConfig );
            A.CallTo( () => feedReader.FeedLanguage ).Returns( "en-US" );

            var expectedPost = new PdsPost
            {
                // Feed title would go, item title more important.
                PostContents =
$@"1234567890

#1 #2",
                Languages = ["en-US"]
            };

            // Act
            PdsPost actualPost = item.GeneratePost( feedReader, rauConfig );

            // Check
            AssertEx.ArePdsPostsEqual( expectedPost, actualPost );
        }

        /// <summary>
        /// If even if removing the feed title results in
        /// too many characters, start stripping the item title.
        /// </summary>
        [TestMethod]
        public void ThreeLessthanCharacterLimitWithHashTagsNoUrlTest()
        {
            // Setup
            var item = new SyndicationItem
            {
                Title = new TextSyndicationContent( "1234567890" )
            };

            var rauConfig = new RauConfig( assemblyVersion )
            {
                CharacterLimit = 14 + ( (uint)Environment.NewLine.Length * 2 ),
                // Feed uses en-US, so it should default to that.
                DefaultLanguages = ["en-ZW"]
            };

            var feedConfig = new FeedConfig(
                new Uri( "https://shendrick.net/rss.xml" ),
                "rssbot",
                "some password",
                new Uri( "https://at.shendrick.net" ),
                "0 0 * * * ?",
                ["1", "2"],
                5,
                null,
                false
            );

            IFeedReader feedReader = A.Fake<IFeedReader>();
            A.CallTo( () => feedReader.FeedTitle ).Returns( "1" );
            A.CallTo( () => feedReader.FeedConfig ).Returns( feedConfig );
            A.CallTo( () => feedReader.FeedLanguage ).Returns( "en-US" );

            var expectedPost = new PdsPost
            {
                // Feed title would go, item title more important.
                PostContents =
$@"123456789

#1 #2",
                Languages = ["en-US"]
            };

            // Act
            PdsPost actualPost = item.GeneratePost( feedReader, rauConfig );

            // Check
            AssertEx.ArePdsPostsEqual( expectedPost, actualPost );
        }

        /// <summary>
        /// If we have so many hash tags we go above the character
        /// limit, ensure they are cleared out.
        /// </summary>
        [TestMethod]
        public void TooManyHashTagsWithinCharacterLimitTest()
        {
            // Setup
            var item = new SyndicationItem
            {
                Title = new TextSyndicationContent( "12345678901234567" )
            };

            var rauConfig = new RauConfig( assemblyVersion )
            {
                CharacterLimit = 20 + ((uint)Environment.NewLine.Length * 2),
                // Feed uses en-US, so it should default to that.
                DefaultLanguages = ["en-ZW"]
            };

            var feedConfig = new FeedConfig(
                new Uri( "https://shendrick.net/rss.xml" ),
                "rssbot",
                "some password",
                new Uri( "https://at.shendrick.net" ),
                "0 0 * * * ?",
                ["234567890", "34567890"], // Should be 20 characters with # and space.
                5,
                null,
                false
            );

            IFeedReader feedReader = A.Fake<IFeedReader>();
            A.CallTo( () => feedReader.FeedTitle ).Returns( "1" );
            A.CallTo( () => feedReader.FeedConfig ).Returns( feedConfig );
            A.CallTo( () => feedReader.FeedLanguage ).Returns( "en-US" );

            var expectedPost = new PdsPost
            {
                PostContents = $@"1: 12345678901234567",
                Languages = ["en-US"]
            };

            // Act
            PdsPost actualPost = item.GeneratePost( feedReader, rauConfig );

            // Check
            AssertEx.ArePdsPostsEqual( expectedPost, actualPost );
        }

        /// <summary>
        /// If we have so many hash tags we go above the character
        /// limit, and the title as well, we only keep the item title.
        /// </summary>
        [TestMethod]
        public void TooManyHashTagsFeedTitleTooBigCharacterLimitTest()
        {
            // Setup
            var item = new SyndicationItem
            {
                Title = new TextSyndicationContent( "12345678901234567" )
            };

            var rauConfig = new RauConfig( assemblyVersion )
            {
                CharacterLimit = 17,
                // Feed uses en-US, so it should default to that.
                DefaultLanguages = ["en-ZW"]
            };

            var feedConfig = new FeedConfig(
                new Uri( "https://shendrick.net/rss.xml" ),
                "rssbot",
                "some password",
                new Uri( "https://at.shendrick.net" ),
                "0 0 * * * ?",
                ["234567890", "34567890"], // Should be 20 characters with # and space.
                5,
                null,
                false
            );

            IFeedReader feedReader = A.Fake<IFeedReader>();
            A.CallTo( () => feedReader.FeedTitle ).Returns( "1" );
            A.CallTo( () => feedReader.FeedConfig ).Returns( feedConfig );
            A.CallTo( () => feedReader.FeedLanguage ).Returns( "en-US" );

            var expectedPost = new PdsPost
            {
                PostContents = $@"12345678901234567",
                Languages = ["en-US"]
            };

            // Act
            PdsPost actualPost = item.GeneratePost( feedReader, rauConfig );

            // Check
            AssertEx.ArePdsPostsEqual( expectedPost, actualPost );
        }

        /// <summary>
        /// If we have so many hash tags we go above the character
        /// limit, and the feed and item title as well, we only keep the
        /// trimmed-down item title.
        /// </summary>
        [TestMethod]
        public void TooManyHashTagsFeedAndItemTitlesTooBigCharacterLimitTest()
        {
            // Setup
            var item = new SyndicationItem
            {
                Title = new TextSyndicationContent( "12345678901234567" )
            };

            var rauConfig = new RauConfig( assemblyVersion )
            {
                CharacterLimit = 16,
                // Feed uses en-US, so it should default to that.
                DefaultLanguages = ["en-ZW"]
            };

            var feedConfig = new FeedConfig(
                new Uri( "https://shendrick.net/rss.xml" ),
                "rssbot",
                "some password",
                new Uri( "https://at.shendrick.net" ),
                "0 0 * * * ?",
                ["234567890", "34567890"], // Should be 20 characters with # and space.
                5,
                null,
                false
            );

            IFeedReader feedReader = A.Fake<IFeedReader>();
            A.CallTo( () => feedReader.FeedTitle ).Returns( "1" );
            A.CallTo( () => feedReader.FeedConfig ).Returns( feedConfig );
            A.CallTo( () => feedReader.FeedLanguage ).Returns( "en-US" );

            var expectedPost = new PdsPost
            {
                PostContents = $@"1234567890123456",
                Languages = ["en-US"]
            };

            // Act
            PdsPost actualPost = item.GeneratePost( feedReader, rauConfig );

            // Check
            AssertEx.ArePdsPostsEqual( expectedPost, actualPost );
        }
    }
}
