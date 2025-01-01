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

using System;
using FakeItEasy;
using Rau.Plugins.Rss2Pds;
using Rau.Standard.Configuration;
using Rau.Standard.Logging;
using Rau.Tests.Mocks.Rau.Standard.EventScheduler;
using SethCS.Extensions;

namespace Rau.Tests.Plugins.Rss2Pds
{
    [TestClass]
    // Since we write to a file system, do not parallelize so nothing
    // weird happens.
    [DoNotParallelize]
    public sealed class FeedUpdateEventTests
    {
        // ---------------- Fields ----------------

        private static readonly string assemblyPath =
            Path.GetDirectoryName( typeof( FeedUpdateEventTests ).Assembly.Location ) ??
            throw new InvalidOperationException( "Unable to get folder location of the assembly" );

        private static readonly DirectoryInfo testDir =
            new DirectoryInfo( Path.Combine( assemblyPath, "FeedUpdateEventTests" ) );

        private static HttpClient? client;

        private RauConfig? rauConfig;
        private IRauLogger? mockLogger;
        private IPdsPoster? mockPoster;
        private IRauApi? mockApi;

        // ---------------- Setup / Teardown ----------------

        [ClassInitialize]
        public static void FixtureSetup( TestContext testContext )
        {
            if( testDir.Exists )
            {
                testDir.Delete( true );
            }

            testDir.Create();

            client = new HttpClient( new FileProtocolHandler() );
        }

        [ClassCleanup]
        public static void FixtureTeardown()
        {
            client?.Dispose();
            client = null;

            if( testDir.Exists )
            {
                testDir.Delete( true );
            }
        }

        [TestInitialize]
        public void TestSetup()
        {
            this.rauConfig = new RauConfig( null )
            {
                DefaultLanguages = new List<string>() { "en-US" }
            };

            this.mockLogger = A.Fake<IRauLogger>();
            this.mockPoster = A.Fake<IPdsPoster>( x => x.Strict() );
            this.mockApi = A.Fake<IRauApi>( x => x.Strict() );

            A.CallTo( () => this.mockApi.Config ).Returns( this.RauConfig );
            A.CallTo( () => this.mockApi.Logger ).Returns( this.MockLogger );
            A.CallTo( () => this.mockApi.PdsPoster ).Returns( this.MockPoster );
        }

        public void TestTeardown()
        {
            this.mockApi = null;
            this.mockLogger = null;
            this.mockPoster = null;
            this.rauConfig = null;
        }

        // ---------------- Properties ----------------

        public HttpClient Client
        {
            get
            {
                Assert.IsNotNull( client );
                return client;
            }
        }

        public RauConfig RauConfig
        {
            get
            {
                Assert.IsNotNull( this.rauConfig );
                return this.rauConfig;
            }
        }

        public IRauLogger MockLogger
        {
            get
            {
                Assert.IsNotNull( this.mockLogger );
                return this.mockLogger;
            }
        }

        public IPdsPoster MockPoster
        {
            get
            {
                Assert.IsNotNull( this.mockPoster );
                return this.mockPoster;
            }
        }

        public IRauApi MockApi
        {
            get
            {
                Assert.IsNotNull( this.mockApi );
                return this.mockApi;
            }
        }

        // ---------------- Tests ----------------

        [TestMethod]
        public void CronStringSetterTest()
        {
            // Setup
            const string cronString = "0 0 * * * ?";

            var config = new FeedConfig(
                new Uri( "https://roclongboarding.info/rss.xml" ),
                "roclongboarding.info",
                "SomePassword",
                new Uri( "https://at.shendrick.net" ),
                cronString,
                null,
                null,
                null,
                true
            );

            var feedReader = new FeedReader( Client, config );
            var uut = new FeedUpdateEvent( feedReader );

            // Act
            string actualCrontString = uut.CronString;

            // Check
            Assert.AreEqual( cronString, actualCrontString );
        }

        /// <summary>
        /// Ensures if there is one update that needs to go out, it does.
        /// </summary>
        [TestMethod]
        public void SingleUpdateTest()
        {
            // Setup
            const string initialFeed =
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<rss version=""2.0"" xmlns:atom=""http://www.w3.org/2005/Atom"">
<channel>
    <title>Rochester Longboarding Info | Seth Hendrick</title>
    <link>https://www.roclongboarding.info</link>
    <atom:link href=""https://www.roclongboarding.info/rss.xml"" rel=""self"" type=""application/rss+xml"" />
    <description>Places to Longboard in Rochester, NY</description>
    <language>en-us</language>
    <pubDate>Mon, 06 May 2024 11:46:25 +00:00</pubDate>
    <lastBuildDate>Mon, 06 May 2024 11:46:25 +00:00</lastBuildDate>
    <item>
        <title>Lehigh Valley Trail: John Street</title>
        <link>https://www.roclongboarding.info/Meh Places/2020/09/27/JohnStreet.html</link>
        <pubDate>Sun, 27 Sep 2020 0:00:00 </pubDate>
        <author>contact@roclongboarding.info (Seth Hendrick)</author>
        <guid isPermaLink=""true"">https://www.roclongboarding.info/Meh Places/2020/09/27/JohnStreet.html</guid>
        <description>Post 1</description>
    </item>
    <item>
        <title>Erie Canal Trail: Between Clinton and Lock 33</title>
        <link>https://www.roclongboarding.info/Cool Places/2020/09/07/ErieCanalClinton.html</link>
        <pubDate>Mon, 07 Sep 2020 0:00:00 </pubDate>
        <author>contact@roclongboarding.info (Seth Hendrick)</author>
        <guid isPermaLink=""true"">https://www.roclongboarding.info/Cool Places/2020/09/07/ErieCanalClinton.html</guid>
        <description>Post 2</description>
    </item>
  </channel> 
</rss>
";

            const string updatedFeed =
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<rss version=""2.0"" xmlns:atom=""http://www.w3.org/2005/Atom"">
<channel>
    <title>Rochester Longboarding Info | Seth Hendrick</title>
    <link>https://www.roclongboarding.info</link>
    <atom:link href=""https://www.roclongboarding.info/rss.xml"" rel=""self"" type=""application/rss+xml"" />
    <description>Places to Longboard in Rochester, NY</description>
    <language>en-us</language>
    <pubDate>Mon, 06 May 2024 11:46:25 +00:00</pubDate>
    <lastBuildDate>Mon, 06 May 2024 11:46:25 +00:00</lastBuildDate>
    <item>
        <title>Lake Ontario State Parkway Trail: Beach Segment</title>
        <link>https://www.roclongboarding.info/Meh Places/2020/11/14/LakeOntarioStateParkwayTrail.html</link>
        <pubDate>Sat, 14 Nov 2020 0:00:00 </pubDate>
        <author>contact@roclongboarding.info (Seth Hendrick)</author>
        <guid isPermaLink=""true"">https://www.roclongboarding.info/Meh Places/2020/11/14/LakeOntarioStateParkwayTrail.html</guid>
        <description>Post 0</description>
    </item>
    <item>
        <title>Lehigh Valley Trail: John Street</title>
        <link>https://www.roclongboarding.info/Meh Places/2020/09/27/JohnStreet.html</link>
        <pubDate>Sun, 27 Sep 2020 0:00:00 </pubDate>
        <author>contact@roclongboarding.info (Seth Hendrick)</author>
        <guid isPermaLink=""true"">https://www.roclongboarding.info/Meh Places/2020/09/27/JohnStreet.html</guid>
        <description>Post 1</description>
    </item>
    <item>
        <title>Erie Canal Trail: Between Clinton and Lock 33</title>
        <link>https://www.roclongboarding.info/Cool Places/2020/09/07/ErieCanalClinton.html</link>
        <pubDate>Mon, 07 Sep 2020 0:00:00 </pubDate>
        <author>contact@roclongboarding.info (Seth Hendrick)</author>
        <guid isPermaLink=""true"">https://www.roclongboarding.info/Cool Places/2020/09/07/ErieCanalClinton.html</guid>
        <description>Post 2</description>
    </item>
  </channel> 
</rss>
";

            const string missingPost =
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<rss version=""2.0"" xmlns:atom=""http://www.w3.org/2005/Atom"">
<channel>
    <title>Rochester Longboarding Info | Seth Hendrick</title>
    <link>https://www.roclongboarding.info</link>
    <atom:link href=""https://www.roclongboarding.info/rss.xml"" rel=""self"" type=""application/rss+xml"" />
    <description>Places to Longboard in Rochester, NY</description>
    <language>en-us</language>
    <pubDate>Mon, 06 May 2024 11:46:25 +00:00</pubDate>
    <lastBuildDate>Mon, 06 May 2024 11:46:25 +00:00</lastBuildDate>
    <item>
        <title>Lake Ontario State Parkway Trail: Beach Segment</title>
        <link>https://www.roclongboarding.info/Meh Places/2020/11/14/LakeOntarioStateParkwayTrail.html</link>
        <pubDate>Sat, 14 Nov 2020 0:00:00 </pubDate>
        <author>contact@roclongboarding.info (Seth Hendrick)</author>
        <guid isPermaLink=""true"">https://www.roclongboarding.info/Meh Places/2020/11/14/LakeOntarioStateParkwayTrail.html</guid>
        <description>Post 0</description>
    </item>
    <item>
        <title>Lehigh Valley Trail: John Street</title>
        <link>https://www.roclongboarding.info/Meh Places/2020/09/27/JohnStreet.html</link>
        <pubDate>Sun, 27 Sep 2020 0:00:00 </pubDate>
        <author>contact@roclongboarding.info (Seth Hendrick)</author>
        <guid isPermaLink=""true"">https://www.roclongboarding.info/Meh Places/2020/09/27/JohnStreet.html</guid>
        <description>Post 1</description>
    </item>
  </channel> 
</rss>
";

            string fileLocation = Path.Combine( testDir.FullName, "RocLongboardingTest.xml" );
            Uri url = new Uri( SethPath.ToUri( fileLocation ) );

            var config = new FeedConfig(
                url,
                "roclongboarding.info",
                "SomePassword",
                new Uri( "https://at.shendrick.net" ),
                "0 0 * * * ?",
                null,
                null,
                null,
                true
            );

            PdsAccount expectedAccount = config.ToPdsAccount();

            var scheduledParams = new StubScheduledEventParameters( this.MockApi );

            var feedReader = new FeedReader( Client, config );
            var uut = new FeedUpdateEvent( feedReader );

            // Initial read, should not produce any posts or warnings.
            {
                File.WriteAllText( fileLocation, initialFeed );
                feedReader.Initialize();
                uut.ExecuteEvent( scheduledParams ).Wait();

                // Should not get any warnings or errors if there's no updates.
                A.CallTo( () => this.MockLogger.Warning( A<string>.Ignored ) ).MustNotHaveHappened();
                A.CallTo( () => this.MockLogger.Error( A<string>.Ignored ) ).MustNotHaveHappened();
                A.CallTo( () => this.MockLogger.Fatal( A<string>.Ignored ) ).MustNotHaveHappened();

                // Should not have posted anything.
                A.CallTo( () => this.MockPoster.Post( A<PdsAccount>.Ignored, A<PdsPost>.Ignored ) ).MustNotHaveHappened();

                Assert.AreEqual<uint>( 0, uut.FailedFetches );
            }

            // Second read, should post one thing.
            {
                File.WriteAllText( fileLocation, updatedFeed );
                Captured<PdsPost> capturedPost = A.Captured<PdsPost>();
                A.CallTo( () => this.MockPoster.Post( expectedAccount, capturedPost._ ) ).DoesNothing();
                uut.ExecuteEvent( scheduledParams ).Wait();

                // Should not get any warnings or errors if there is no failure.
                A.CallTo( () => this.MockLogger.Warning( A<string>.Ignored ) ).MustNotHaveHappened();
                A.CallTo( () => this.MockLogger.Error( A<string>.Ignored ) ).MustNotHaveHappened();
                A.CallTo( () => this.MockLogger.Fatal( A<string>.Ignored ) ).MustNotHaveHappened();

                // Should have posted one thing.
                A.CallTo( () => this.MockPoster.Post( expectedAccount, capturedPost._ ) ).MustHaveHappenedOnceExactly();

                Assert.AreEqual( 1, capturedPost.Values.Count );

                var expectedPost = new PdsPost
                {
                    Languages = ["en-us"],
                    PostAttachmentPage = new Uri( "https://www.roclongboarding.info/Meh Places/2020/11/14/LakeOntarioStateParkwayTrail.html" ),
                    PostContents = "Rochester Longboarding Info | Seth Hendrick: Lake Ontario State Parkway Trail: Beach Segment"
                };
                AssertEx.ArePdsPostsEqual( expectedPost, capturedPost.GetLastValue() );
                Assert.AreEqual<uint>( 0, uut.FailedFetches );
            }

            // Third read when missing something.  Should not post anything
            {
                File.WriteAllText( fileLocation, missingPost );
                Captured<PdsPost> capturedPost = A.Captured<PdsPost>();
                A.CallTo( () => this.MockPoster.Post( A<PdsAccount>.Ignored, capturedPost._ ) ).DoesNothing();
                uut.ExecuteEvent( scheduledParams ).Wait();

                // Should not get any warnings or errors if there is no failure.
                A.CallTo( () => this.MockLogger.Warning( A<string>.Ignored ) ).MustNotHaveHappened();
                A.CallTo( () => this.MockLogger.Error( A<string>.Ignored ) ).MustNotHaveHappened();
                A.CallTo( () => this.MockLogger.Fatal( A<string>.Ignored ) ).MustNotHaveHappened();

                // Should not have posted anything.
                Assert.AreEqual( 0, capturedPost.Values.Count );
                Assert.AreEqual<uint>( 0, uut.FailedFetches );
            }
        }

        /// <summary>
        /// Ensures if there are two updates that need to go out, they do.
        /// </summary>
        [TestMethod]
        public void DoubleUpdateTest()
        {
            // Setup
            const string initialFeed =
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<rss version=""2.0"" xmlns:atom=""http://www.w3.org/2005/Atom"">
  <channel>
    <title>RIT Linux Users Group</title>
    <description>RIT Linux Users Group Announcements</description>
    <link>https://ritlug.com/</link>
    <atom:link href=""https://ritlug.com/feed.xml"" rel=""self"" type=""application/rss+xml"" />
    <pubDate>Fri, 06 Dec 2024 16:47:13 +0000</pubDate>
    <lastBuildDate>Fri, 06 Dec 2024 16:47:13 +0000</lastBuildDate>
    <generator>Jekyll v3.10.0</generator>    
      <item>
        <title>Week #12: Lightning Talks</title>
        <description>&lt;p&gt;Hi everyone!&lt;/p&gt;

&lt;p&gt;This week, we are doing lightning talks! Anyone is free to bring a 5-10 minute presentation about any topic they’d like - it could be FOSS/Linux related but does not have to. No need to let us know ahead of time, and if you need a computer or adapter to present we would be happy to provide one.&lt;/p&gt;

&lt;p&gt;See you all there!&lt;/p&gt;
</description>
        <pubDate>Fri, 15 Nov 2024 00:00:00 +0000</pubDate>
        <link>https://ritlug.com/announcements/2024/11/15/lightning_talks/</link>
        <guid isPermaLink=""true"">https://ritlug.com/announcements/2024/11/15/lightning_talks/</guid>
        
        
        <category>announcements</category>
        
      </item>
</channel>
</rss>
";

            const string updatedFeed =
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<rss version=""2.0"" xmlns:atom=""http://www.w3.org/2005/Atom"">
  <channel>
    <title>RIT Linux Users Group</title>
    <description>RIT Linux Users Group Announcements</description>
    <link>https://ritlug.com/</link>
    <atom:link href=""https://ritlug.com/feed.xml"" rel=""self"" type=""application/rss+xml"" />
    <pubDate>Fri, 06 Dec 2024 16:47:13 +0000</pubDate>
    <lastBuildDate>Fri, 06 Dec 2024 16:47:13 +0000</lastBuildDate>
    <generator>Jekyll v3.10.0</generator>
    
      <item>
        <title>Week #14: End of Semester</title>
        <description>&lt;p&gt;Hi everyone!&lt;/p&gt;

&lt;p&gt;With finals coming up, this week’s meeting will be more focused on hanging out; there will be no talk today. As usual, we will meet today at 4:30 - 6:00 PM in GOL-2455. Thanks to everyone who came to meetings this semester, and an extra thanks to everyone who presented!&lt;/p&gt;

&lt;p&gt;See you there!&lt;/p&gt;
</description>
        <pubDate>Fri, 06 Dec 2024 00:00:00 +0000</pubDate>
        <link>https://ritlug.com/announcements/2024/12/06/end_of_semester/</link>
        <guid isPermaLink=""true"">https://ritlug.com/announcements/2024/12/06/end_of_semester/</guid>
        
        
        <category>announcements</category>
        
      </item>
    
      <item>
        <title>Week #13: Bootstrapping</title>
        <description>&lt;p&gt;Hi everyone!&lt;/p&gt;

&lt;p&gt;This week Ryan will be giving a talk about software bootstrapping, reproducibility, and why those are hard problems.&lt;/p&gt;

&lt;p&gt;See you all there!&lt;/p&gt;
</description>
        <pubDate>Fri, 22 Nov 2024 00:00:00 +0000</pubDate>
        <link>https://ritlug.com/announcements/2024/11/22/bootstrapping/</link>
        <guid isPermaLink=""true"">https://ritlug.com/announcements/2024/11/22/bootstrapping/</guid>
        
        
        <category>announcements</category>
        
      </item>
    
      <item>
        <title>Week #12: Lightning Talks</title>
        <description>&lt;p&gt;Hi everyone!&lt;/p&gt;

&lt;p&gt;This week, we are doing lightning talks! Anyone is free to bring a 5-10 minute presentation about any topic they’d like - it could be FOSS/Linux related but does not have to. No need to let us know ahead of time, and if you need a computer or adapter to present we would be happy to provide one.&lt;/p&gt;

&lt;p&gt;See you all there!&lt;/p&gt;
</description>
        <pubDate>Fri, 15 Nov 2024 00:00:00 +0000</pubDate>
        <link>https://ritlug.com/announcements/2024/11/15/lightning_talks/</link>
        <guid isPermaLink=""true"">https://ritlug.com/announcements/2024/11/15/lightning_talks/</guid>
        
        
        <category>announcements</category>
        
      </item>
</channel>
</rss>
";

            const string missingPost =
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<rss version=""2.0"" xmlns:atom=""http://www.w3.org/2005/Atom"">
  <channel>
    <title>RIT Linux Users Group</title>
    <description>RIT Linux Users Group Announcements</description>
    <link>https://ritlug.com/</link>
    <atom:link href=""https://ritlug.com/feed.xml"" rel=""self"" type=""application/rss+xml"" />
    <pubDate>Fri, 06 Dec 2024 16:47:13 +0000</pubDate>
    <lastBuildDate>Fri, 06 Dec 2024 16:47:13 +0000</lastBuildDate>
    <generator>Jekyll v3.10.0</generator>
    
      <item>
        <title>Week #14: End of Semester</title>
        <description>&lt;p&gt;Hi everyone!&lt;/p&gt;

&lt;p&gt;With finals coming up, this week’s meeting will be more focused on hanging out; there will be no talk today. As usual, we will meet today at 4:30 - 6:00 PM in GOL-2455. Thanks to everyone who came to meetings this semester, and an extra thanks to everyone who presented!&lt;/p&gt;

&lt;p&gt;See you there!&lt;/p&gt;
</description>
        <pubDate>Fri, 06 Dec 2024 00:00:00 +0000</pubDate>
        <link>https://ritlug.com/announcements/2024/12/06/end_of_semester/</link>
        <guid isPermaLink=""true"">https://ritlug.com/announcements/2024/12/06/end_of_semester/</guid>
        
        
        <category>announcements</category>
        
      </item>
    
      <item>
        <title>Week #13: Bootstrapping</title>
        <description>&lt;p&gt;Hi everyone!&lt;/p&gt;

&lt;p&gt;This week Ryan will be giving a talk about software bootstrapping, reproducibility, and why those are hard problems.&lt;/p&gt;

&lt;p&gt;See you all there!&lt;/p&gt;
</description>
        <pubDate>Fri, 22 Nov 2024 00:00:00 +0000</pubDate>
        <link>https://ritlug.com/announcements/2024/11/22/bootstrapping/</link>
        <guid isPermaLink=""true"">https://ritlug.com/announcements/2024/11/22/bootstrapping/</guid>
        
        
        <category>announcements</category>
        
      </item>
</channel>
</rss>";

            string fileLocation = Path.Combine( testDir.FullName, "RitLugLatest.xml" );
            Uri url = new Uri( SethPath.ToUri( fileLocation ) );

            var config = new FeedConfig(
                url,
                "ritlug.com",
                "SomePassword",
                new Uri( "https://at.shendrick.net" ),
                "0 0 * * * ?",
                null,
                null,
                ["en-GB"],
                true
            );

            PdsAccount expectedAccount = config.ToPdsAccount();

            var scheduledParams = new StubScheduledEventParameters( this.MockApi );

            var feedReader = new FeedReader( Client, config );
            var uut = new FeedUpdateEvent( feedReader );

            // Initial read, should not produce any posts or warnings.
            {
                File.WriteAllText( fileLocation, initialFeed );
                feedReader.Initialize();
                uut.ExecuteEvent( scheduledParams ).Wait();

                // Should not get any warnings or errors if there's no updates.
                A.CallTo( () => this.MockLogger.Warning( A<string>.Ignored ) ).MustNotHaveHappened();
                A.CallTo( () => this.MockLogger.Error( A<string>.Ignored ) ).MustNotHaveHappened();
                A.CallTo( () => this.MockLogger.Fatal( A<string>.Ignored ) ).MustNotHaveHappened();

                // Should not have posted anything.
                A.CallTo( () => this.MockPoster.Post( A<PdsAccount>.Ignored, A<PdsPost>.Ignored ) ).MustNotHaveHappened();

                Assert.AreEqual<uint>( 0, uut.FailedFetches );
            }

            // Second read, should produce two posts.
            {
                File.WriteAllText( fileLocation, updatedFeed );
                Captured<PdsPost> capturedPost = A.Captured<PdsPost>();
                A.CallTo( () => this.MockPoster.Post( expectedAccount, capturedPost._ ) ).DoesNothing();
                uut.ExecuteEvent( scheduledParams ).Wait();

                // Should not get any warnings or errors if there is no failure.
                A.CallTo( () => this.MockLogger.Warning( A<string>.Ignored ) ).MustNotHaveHappened();
                A.CallTo( () => this.MockLogger.Error( A<string>.Ignored ) ).MustNotHaveHappened();
                A.CallTo( () => this.MockLogger.Fatal( A<string>.Ignored ) ).MustNotHaveHappened();

                // Should have posted one thing.
                A.CallTo( () => this.MockPoster.Post( expectedAccount, capturedPost._ ) ).MustHaveHappenedTwiceExactly();

                Assert.AreEqual( 2, capturedPost.Values.Count );

                var expectedPost1 = new PdsPost
                {
                    Languages = ["en-GB"],
                    PostAttachmentPage = new Uri( "https://ritlug.com/announcements/2024/11/22/bootstrapping/" ),
                    PostContents = "RIT Linux Users Group: Week #13: Bootstrapping"
                };

                var expectedPost2 = new PdsPost
                {
                    Languages = ["en-GB"],
                    PostAttachmentPage = new Uri( "https://ritlug.com/announcements/2024/12/06/end_of_semester/" ),
                    PostContents = "RIT Linux Users Group: Week #14: End of Semester"
                };

                AssertEx.ArePdsPostsEqual( expectedPost1, capturedPost.Values[0] );
                AssertEx.ArePdsPostsEqual( expectedPost2, capturedPost.Values[1] );
                Assert.AreEqual<uint>( 0, uut.FailedFetches );
            }

            // Third read when missing something.  Should not post anything
            {
                File.WriteAllText( fileLocation, missingPost );
                Captured<PdsPost> capturedPost = A.Captured<PdsPost>();
                A.CallTo( () => this.MockPoster.Post( A<PdsAccount>.Ignored, capturedPost._ ) ).DoesNothing();
                uut.ExecuteEvent( scheduledParams ).Wait();

                // Should not get any warnings or errors if there is no failure.
                A.CallTo( () => this.MockLogger.Warning( A<string>.Ignored ) ).MustNotHaveHappened();
                A.CallTo( () => this.MockLogger.Error( A<string>.Ignored ) ).MustNotHaveHappened();
                A.CallTo( () => this.MockLogger.Fatal( A<string>.Ignored ) ).MustNotHaveHappened();

                // Should not have posted anything.
                Assert.AreEqual( 0, capturedPost.Values.Count );
                Assert.AreEqual<uint>( 0, uut.FailedFetches );
            }
        }

        /// <summary>
        /// Ensures if we fail to update with <see cref="FeedConfig.AlertThreshold"/>
        /// set to 0, we always get an alert.
        /// </summary>
        [TestMethod]
        public void FailedToUpdateWithZeroAttemptsTest()
        {
            // Setup
            const string initialFeed =
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<rss version=""2.0"" xmlns:atom=""http://www.w3.org/2005/Atom"">
<channel>
    <title>Rochester Longboarding Info | Seth Hendrick</title>
    <link>https://www.roclongboarding.info</link>
    <atom:link href=""https://www.roclongboarding.info/rss.xml"" rel=""self"" type=""application/rss+xml"" />
    <description>Places to Longboard in Rochester, NY</description>
    <language>en-us</language>
    <pubDate>Mon, 06 May 2024 11:46:25 +00:00</pubDate>
    <lastBuildDate>Mon, 06 May 2024 11:46:25 +00:00</lastBuildDate>
    <item>
        <title>Lehigh Valley Trail: John Street</title>
        <link>https://www.roclongboarding.info/Meh Places/2020/09/27/JohnStreet.html</link>
        <pubDate>Sun, 27 Sep 2020 0:00:00 </pubDate>
        <author>contact@roclongboarding.info (Seth Hendrick)</author>
        <guid isPermaLink=""true"">https://www.roclongboarding.info/Meh Places/2020/09/27/JohnStreet.html</guid>
        <description>Post 1</description>
    </item>
    <item>
        <title>Erie Canal Trail: Between Clinton and Lock 33</title>
        <link>https://www.roclongboarding.info/Cool Places/2020/09/07/ErieCanalClinton.html</link>
        <pubDate>Mon, 07 Sep 2020 0:00:00 </pubDate>
        <author>contact@roclongboarding.info (Seth Hendrick)</author>
        <guid isPermaLink=""true"">https://www.roclongboarding.info/Cool Places/2020/09/07/ErieCanalClinton.html</guid>
        <description>Post 2</description>
    </item>
  </channel> 
</rss>
";

            const string updatedFeed =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
<rss version=""2.0"" xmlns:atom=""http://www.w3.org/2005/Atom"">
<channel>
    <title>Rochester Longboarding Info | Seth Hendrick</title>
    <link>https://www.roclongboarding.info</link>
    <atom:link href=""https://www.roclongboarding.info/rss.xml"" rel=""self"" type=""application/rss+xml"" />
    <description>Places to Longboard in Rochester, NY</description>
    <language>en-us</language>
    <pubDate>Mon, 06 May 2024 11:46:25 +00:00</pubDate>
    <lastBuildDate>Mon, 06 May 2024 11:46:25 +00:00</lastBuildDate>
    <item>
        <title>Lake Ontario State Parkway Trail: Beach Segment</title>
        <link>https://www.roclongboarding.info/Meh Places/2020/11/14/LakeOntarioStateParkwayTrail.html</link>
        <pubDate>Sat, 14 Nov 2020 0:00:00 </pubDate>
        <author>contact@roclongboarding.info (Seth Hendrick)</author>
        <guid isPermaLink=""true"">https://www.roclongboarding.info/Meh Places/2020/11/14/LakeOntarioStateParkwayTrail.html</guid>
        <description>Post 0</description>
    </item>
    <item>
        <title>Lehigh Valley Trail: John Street</title>
        <link>https://www.roclongboarding.info/Meh Places/2020/09/27/JohnStreet.html</link>
        <pubDate>Sun, 27 Sep 2020 0:00:00 </pubDate>
        <author>contact@roclongboarding.info (Seth Hendrick)</author>
        <guid isPermaLink=""true"">https://www.roclongboarding.info/Meh Places/2020/09/27/JohnStreet.html</guid>
        <description>Post 1</description>
    </item>
    <item>
        <title>Erie Canal Trail: Between Clinton and Lock 33</title>
        <link>https://www.roclongboarding.info/Cool Places/2020/09/07/ErieCanalClinton.html</link>
        <pubDate>Mon, 07 Sep 2020 0:00:00 </pubDate>
        <author>contact@roclongboarding.info (Seth Hendrick)</author>
        <guid isPermaLink=""true"">https://www.roclongboarding.info/Cool Places/2020/09/07/ErieCanalClinton.html</guid>
        <description>Post 2</description>
    </item>
  </channel> 
</rss>
";
            string fileLocation = Path.Combine( testDir.FullName, "ZeroAttemptsTest.xml" );
            Uri url = new Uri( SethPath.ToUri( fileLocation ) );

            var config = new FeedConfig(
                url,
                "roclongboarding.info",
                "SomePassword",
                new Uri( "https://at.shendrick.net" ),
                "0 0 * * * ?",
                null,
                0, // <- No alerts, expect all failures to produce an error message.
                null,
                true
            );

            PdsAccount expectedAccount = config.ToPdsAccount();

            var scheduledParams = new StubScheduledEventParameters( this.MockApi );

            var feedReader = new FeedReader( Client, config );
            var uut = new FeedUpdateEvent( feedReader );

            // Initial read, should not produce any posts or warnings.
            {
                File.WriteAllText( fileLocation, initialFeed );
                feedReader.Initialize();
                uut.ExecuteEvent( scheduledParams ).Wait();

                // Should not get any warnings or errors if there's no updates.
                A.CallTo( () => this.MockLogger.Warning( A<string>.Ignored ) ).MustNotHaveHappened();
                A.CallTo( () => this.MockLogger.Error( A<string>.Ignored ) ).MustNotHaveHappened();
                A.CallTo( () => this.MockLogger.Fatal( A<string>.Ignored ) ).MustNotHaveHappened();

                // Should not have posted anything.
                A.CallTo( () => this.MockPoster.Post( A<PdsAccount>.Ignored, A<PdsPost>.Ignored ) ).MustNotHaveHappened();

                Assert.AreEqual<uint>( 0, uut.FailedFetches );
            }

            // Second read, should make a warning.
            {
                Captured<string> capturedMessage = A.Captured<string>();
                File.Delete( fileLocation );

                A.CallTo( () => this.MockLogger.Warning( capturedMessage._ ) ).DoesNothing();
                uut.ExecuteEvent( scheduledParams ).Wait();

                // Should get 1 warning.  No errors or worse though.
                A.CallTo( () => this.MockLogger.Warning( A<string>.Ignored ) ).MustHaveHappenedOnceExactly();
                A.CallTo( () => this.MockLogger.Error( A<string>.Ignored ) ).MustNotHaveHappened();
                A.CallTo( () => this.MockLogger.Fatal( A<string>.Ignored ) ).MustNotHaveHappened();

                // Should not have posted anything.
                A.CallTo( () => this.MockPoster.Post( A<PdsAccount>.Ignored, A<PdsPost>.Ignored ) ).MustNotHaveHappened();

                Assert.AreEqual( 1, capturedMessage.Values.Count );
                Assert.AreEqual<uint>( 1, uut.FailedFetches );
            }

            // A third read, should not make a warning; don't want to spam
            // notifications.
            {
                Captured<string> capturedMessage = A.Captured<string>();

                A.CallTo( () => this.MockLogger.Warning( capturedMessage._ ) ).DoesNothing();
                uut.ExecuteEvent( scheduledParams ).Wait();

                // Should get 1 warning.  No errors or worse though.
                A.CallTo( () => this.MockLogger.Warning( A<string>.Ignored ) ).MustHaveHappenedOnceExactly();
                A.CallTo( () => this.MockLogger.Error( A<string>.Ignored ) ).MustNotHaveHappened();
                A.CallTo( () => this.MockLogger.Fatal( A<string>.Ignored ) ).MustNotHaveHappened();

                // Should not have posted anything.
                A.CallTo( () => this.MockPoster.Post( A<PdsAccount>.Ignored, A<PdsPost>.Ignored ) ).MustNotHaveHappened();

                Assert.AreEqual( 0, capturedMessage.Values.Count );
                Assert.AreEqual<uint>( 2, uut.FailedFetches );
            }

            // A fourth read, this one works.
            {
                File.WriteAllText( fileLocation, updatedFeed );

                Captured<string> capturedMessage = A.Captured<string>();
                A.CallTo( () => this.MockLogger.Warning( capturedMessage._ ) ).DoesNothing();

                Captured<PdsPost> capturedPost = A.Captured<PdsPost>();
                A.CallTo( () => this.MockPoster.Post( expectedAccount, capturedPost._ ) ).DoesNothing();
                uut.ExecuteEvent( scheduledParams ).Wait();

                // Should not get any warnings or errors if there is no failure.
                A.CallTo( () => this.MockLogger.Warning( A<string>.Ignored ) ).MustHaveHappenedOnceExactly();
                A.CallTo( () => this.MockLogger.Error( A<string>.Ignored ) ).MustNotHaveHappened();
                A.CallTo( () => this.MockLogger.Fatal( A<string>.Ignored ) ).MustNotHaveHappened();

                // Should have posted one thing.
                A.CallTo( () => this.MockPoster.Post( expectedAccount, capturedPost._ ) ).MustHaveHappenedOnceExactly();

                Assert.AreEqual( 1, capturedPost.Values.Count );

                var expectedPost = new PdsPost
                {
                    Languages = ["en-us"],
                    PostAttachmentPage = new Uri( "https://www.roclongboarding.info/Meh Places/2020/11/14/LakeOntarioStateParkwayTrail.html" ),
                    PostContents = "Rochester Longboarding Info | Seth Hendrick: Lake Ontario State Parkway Trail: Beach Segment"
                };
                AssertEx.ArePdsPostsEqual( expectedPost, capturedPost.GetLastValue() );
                Assert.AreEqual( 0, capturedMessage.Values.Count );
                Assert.AreEqual<uint>( 0, uut.FailedFetches );
            }
        }

        /// <summary>
        /// Ensures if we fail to update with <see cref="FeedConfig.AlertThreshold"/>
        /// set to 1, we always get an alert after the first failure.
        /// </summary>
        [TestMethod]
        public void FailedToUpdateWithOneAttemptsTest()
        {
            // Setup
            const string initialFeed =
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<rss version=""2.0"" xmlns:atom=""http://www.w3.org/2005/Atom"">
<channel>
    <title>Rochester Longboarding Info | Seth Hendrick</title>
    <link>https://www.roclongboarding.info</link>
    <atom:link href=""https://www.roclongboarding.info/rss.xml"" rel=""self"" type=""application/rss+xml"" />
    <description>Places to Longboard in Rochester, NY</description>
    <language>en-us</language>
    <pubDate>Mon, 06 May 2024 11:46:25 +00:00</pubDate>
    <lastBuildDate>Mon, 06 May 2024 11:46:25 +00:00</lastBuildDate>
    <item>
        <title>Lehigh Valley Trail: John Street</title>
        <link>https://www.roclongboarding.info/Meh Places/2020/09/27/JohnStreet.html</link>
        <pubDate>Sun, 27 Sep 2020 0:00:00 </pubDate>
        <author>contact@roclongboarding.info (Seth Hendrick)</author>
        <guid isPermaLink=""true"">https://www.roclongboarding.info/Meh Places/2020/09/27/JohnStreet.html</guid>
        <description>Post 1</description>
    </item>
    <item>
        <title>Erie Canal Trail: Between Clinton and Lock 33</title>
        <link>https://www.roclongboarding.info/Cool Places/2020/09/07/ErieCanalClinton.html</link>
        <pubDate>Mon, 07 Sep 2020 0:00:00 </pubDate>
        <author>contact@roclongboarding.info (Seth Hendrick)</author>
        <guid isPermaLink=""true"">https://www.roclongboarding.info/Cool Places/2020/09/07/ErieCanalClinton.html</guid>
        <description>Post 2</description>
    </item>
  </channel> 
</rss>
";
            string fileLocation = Path.Combine( testDir.FullName, "OneAttemptsTest.xml" );
            Uri url = new Uri( SethPath.ToUri( fileLocation ) );

            var config = new FeedConfig(
                url,
                "roclongboarding.info",
                "SomePassword",
                new Uri( "https://at.shendrick.net" ),
                "0 0 * * * ?",
                null,
                1, // <- Allow 1 failure before notifying people.
                null,
                true
            );

            PdsAccount expectedAccount = config.ToPdsAccount();

            var scheduledParams = new StubScheduledEventParameters( this.MockApi );

            var feedReader = new FeedReader( Client, config );
            var uut = new FeedUpdateEvent( feedReader );

            // Initial read, should not produce any posts or warnings.
            {
                File.WriteAllText( fileLocation, initialFeed );
                feedReader.Initialize();
                uut.ExecuteEvent( scheduledParams ).Wait();

                // Should not get any warnings or errors if there's no updates.
                A.CallTo( () => this.MockLogger.Warning( A<string>.Ignored ) ).MustNotHaveHappened();
                A.CallTo( () => this.MockLogger.Error( A<string>.Ignored ) ).MustNotHaveHappened();
                A.CallTo( () => this.MockLogger.Fatal( A<string>.Ignored ) ).MustNotHaveHappened();

                // Should not have posted anything.
                A.CallTo( () => this.MockPoster.Post( A<PdsAccount>.Ignored, A<PdsPost>.Ignored ) ).MustNotHaveHappened();

                Assert.AreEqual<uint>( 0, uut.FailedFetches );
            }

            // Second read, should NOT make a warning.
            {
                File.Delete( fileLocation );

                uut.ExecuteEvent( scheduledParams ).Wait();

                A.CallTo( () => this.MockLogger.Warning( A<string>.Ignored ) ).MustNotHaveHappened();
                A.CallTo( () => this.MockLogger.Error( A<string>.Ignored ) ).MustNotHaveHappened();
                A.CallTo( () => this.MockLogger.Fatal( A<string>.Ignored ) ).MustNotHaveHappened();

                // Should not have posted anything.
                A.CallTo( () => this.MockPoster.Post( A<PdsAccount>.Ignored, A<PdsPost>.Ignored ) ).MustNotHaveHappened();

                Assert.AreEqual<uint>( 1, uut.FailedFetches );
            }

            // Third read, should make a warning.
            {
                Captured<string> capturedMessage = A.Captured<string>();
                File.Delete( fileLocation );

                A.CallTo( () => this.MockLogger.Warning( capturedMessage._ ) ).DoesNothing();
                uut.ExecuteEvent( scheduledParams ).Wait();

                // Should get 1 warning.  No errors or worse though.
                A.CallTo( () => this.MockLogger.Warning( A<string>.Ignored ) ).MustHaveHappenedOnceExactly();
                A.CallTo( () => this.MockLogger.Error( A<string>.Ignored ) ).MustNotHaveHappened();
                A.CallTo( () => this.MockLogger.Fatal( A<string>.Ignored ) ).MustNotHaveHappened();

                // Should not have posted anything.
                A.CallTo( () => this.MockPoster.Post( A<PdsAccount>.Ignored, A<PdsPost>.Ignored ) ).MustNotHaveHappened();

                Assert.AreEqual( 1, capturedMessage.Values.Count );
                Assert.AreEqual<uint>( 2, uut.FailedFetches );
            }

            // A fourth read, should not make a warning; don't want to spam
            // notifications.
            {
                Captured<string> capturedMessage = A.Captured<string>();

                A.CallTo( () => this.MockLogger.Warning( capturedMessage._ ) ).DoesNothing();
                uut.ExecuteEvent( scheduledParams ).Wait();

                // Should get 1 warning.  No errors or worse though.
                A.CallTo( () => this.MockLogger.Warning( A<string>.Ignored ) ).MustHaveHappenedOnceExactly();
                A.CallTo( () => this.MockLogger.Error( A<string>.Ignored ) ).MustNotHaveHappened();
                A.CallTo( () => this.MockLogger.Fatal( A<string>.Ignored ) ).MustNotHaveHappened();

                // Should not have posted anything.
                A.CallTo( () => this.MockPoster.Post( A<PdsAccount>.Ignored, A<PdsPost>.Ignored ) ).MustNotHaveHappened();

                Assert.AreEqual( 0, capturedMessage.Values.Count );
                Assert.AreEqual<uint>( 3, uut.FailedFetches );
            }

            // The last read read, this one works, but no updates are there, so nothing happens.
            {
                File.WriteAllText( fileLocation, initialFeed );

                Captured<string> capturedMessage = A.Captured<string>();
                A.CallTo( () => this.MockLogger.Warning( capturedMessage._ ) ).DoesNothing();

                uut.ExecuteEvent( scheduledParams ).Wait();

                // Should not get any warnings or errors if there is no failure.
                A.CallTo( () => this.MockLogger.Warning( A<string>.Ignored ) ).MustHaveHappenedOnceExactly();
                A.CallTo( () => this.MockLogger.Error( A<string>.Ignored ) ).MustNotHaveHappened();
                A.CallTo( () => this.MockLogger.Fatal( A<string>.Ignored ) ).MustNotHaveHappened();

                // Should not have posted anything.
                A.CallTo( () => this.MockPoster.Post( A<PdsAccount>.Ignored, A<PdsPost>.Ignored ) ).MustNotHaveHappened();

                Assert.AreEqual( 0, capturedMessage.Values.Count );
                Assert.AreEqual<uint>( 0, uut.FailedFetches );
            }
        }
    }
}
