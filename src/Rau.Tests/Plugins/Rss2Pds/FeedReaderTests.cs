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
using Rau.Plugins.Rss2Pds;
using Rau.Standard.Configuration;
using SethCS.Extensions;

namespace Rau.Tests.Plugins.Rss2Pds
{
    [TestClass]
    // Since we write to a file system, do not parallelize so nothing
    // weird happens.
    [DoNotParallelize]
    public sealed class FeedReaderTests
    {
        // ---------------- Fields ----------------

        private static readonly string assemblyPath =
            Path.GetDirectoryName( typeof( FeedReaderTests ).Assembly.Location ) ??
            throw new InvalidOperationException( "Unable to get folder location of the assembly" );

        private static readonly DirectoryInfo testDir =
            new DirectoryInfo( Path.Combine( assemblyPath, "FeedReaderTests" ) );

        private static HttpClient? client;
        
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
        
        // ---------------- Properties ----------------

        public HttpClient Client
        {
            get
            {
                Assert.IsNotNull( client );
                return client;
            }
        }
        
        // ---------------- Tests ----------------

        [TestMethod]
        public void RocLongboardingTest()
        { 
            // Setup
            const string initialRocLongboardingFeed = 
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

            var rauConfig = new RauConfig( null )
            {
                DefaultLanguages = new List<string>() { "en-US" }
            };
            
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
            
            var uut = new FeedReader( Client, config );
            
            // Act
            File.WriteAllText( fileLocation, initialRocLongboardingFeed );
            uut.Initialize();
            
            File.WriteAllText( fileLocation, updatedFeed );
            List<SyndicationItem> firstPass = uut.UpdateAsync().Result;
            List<SyndicationItem> secondPass = uut.UpdateAsync().Result;

            File.WriteAllText( fileLocation, missingPost );
            List<SyndicationItem> passWithMissingPost = uut.UpdateAsync().Result;
            
            // Check
            Assert.AreEqual(
                "Rochester Longboarding Info | Seth Hendrick",
                uut.FeedTitle
            );
            Assert.AreEqual( "en-us", uut.FeedLanguage );
            
            Assert.AreEqual( 1, firstPass.Count );
            Assert.AreEqual( 0, secondPass.Count );
            Assert.AreEqual( 0, passWithMissingPost.Count );

            var expectedPost = new PdsPost
            {
                Languages = ["en-us"],
                PostAttachmentPage = new Uri( "https://www.roclongboarding.info/Meh Places/2020/11/14/LakeOntarioStateParkwayTrail.html" ),
                PostContents = "Rochester Longboarding Info | Seth Hendrick: Lake Ontario State Parkway Trail: Beach Segment"
            };
            PdsPost post = firstPass[0].GeneratePost( uut, rauConfig );
            AssertEx.ArePdsPostsEqual( expectedPost, post );
        }
        
        /// <summary>
        /// Tests an ATOM feed with no language specified.
        /// </summary>
        [TestMethod]
        public void RitlugLatest()
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

            var rauConfig = new RauConfig( null )
            {
                DefaultLanguages = new List<string>() { "en-US" }
            };
            
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
            
            var uut = new FeedReader( Client, config );
            
            // Act
            File.WriteAllText( fileLocation, initialFeed );
            uut.Initialize();
            
            File.WriteAllText( fileLocation, updatedFeed );
            List<SyndicationItem> firstPass = uut.UpdateAsync().Result;
            List<SyndicationItem> secondPass = uut.UpdateAsync().Result;

            File.WriteAllText( fileLocation, missingPost );
            List<SyndicationItem> passWithMissingPost = uut.UpdateAsync().Result;
            
            // Check
            Assert.AreEqual(
                "RIT Linux Users Group",
                uut.FeedTitle
            );
            Assert.IsNull( uut.FeedLanguage ); // No language specified.  Leave null.
            
            Assert.AreEqual( 2, firstPass.Count );
            Assert.AreEqual( 0, secondPass.Count );
            Assert.AreEqual( 0, passWithMissingPost.Count );

            var expectedPost1 = new PdsPost
            {
                Languages = ["en-GB"],
                PostAttachmentPage = new Uri( "https://ritlug.com/announcements/2024/11/22/bootstrapping/" ),
                PostContents = "RIT Linux Users Group: Week #13: Bootstrapping"
            };
            PdsPost post1 = firstPass[0].GeneratePost( uut, rauConfig );
            AssertEx.ArePdsPostsEqual( expectedPost1, post1 );
            
            var expectedPost2 = new PdsPost
            {
                Languages = ["en-GB"],
                PostAttachmentPage = new Uri( "https://ritlug.com/announcements/2024/12/06/end_of_semester/" ),
                PostContents = "RIT Linux Users Group: Week #14: End of Semester"
            };
            PdsPost post2 = firstPass[1].GeneratePost( uut, rauConfig );
            AssertEx.ArePdsPostsEqual( expectedPost2, post2 );
        }
        
        /// <summary>
        /// Tests what happens if a post appears with
        /// the date in between already cached posts.
        /// </summary>
        [TestMethod]
        public void RitlugRedditLatest()
        { 
            // Setup
            const string initialFeed = 
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<feed xmlns=""http://www.w3.org/2005/Atom"" xmlns:media=""http://search.yahoo.com/mrss/"">
  <category term=""RITLUG"" label=""r/RITLUG""/>
  <updated>2024-12-27T02:41:43+00:00</updated>
  <icon>https://www.redditstatic.com/icon.png/</icon>
  <id>/r/ritlug/.rss</id>
  <link rel=""self"" href=""https://old.reddit.com/r/ritlug/.rss"" type=""application/atom+xml""/>
  <link rel=""alternate"" href=""https://old.reddit.com/r/ritlug/"" type=""text/html""/>
  <subtitle>The official subreddit for the RIT Linux Users Group, RITlug! All announcements and club-related discussions take place on our various chat channels.</subtitle>
  <title>RIT Linux Users Group</title>
  <entry>
    <author>
      <name>/u/ryan77627</name>
      <uri>https://old.reddit.com/user/ryan77627</uri>
    </author>
    <category term=""RITLUG"" label=""r/RITLUG""/>
    <content type=""html"">Where to find us!</content>
    <id>t3_10kg65u</id>
    <link href=""https://old.reddit.com/r/RITLUG/comments/10kg65u/where_to_find_us_links/""/>
    <updated>2023-01-24T21:02:43+00:00</updated>
    <published>2023-01-24T21:02:43+00:00</published>
    <title>Where to find us + Links</title>
  </entry>
  <entry>
    <author>
      <name>/u/ritlug</name>
      <uri>https://old.reddit.com/user/ritlug</uri>
    </author>
    <category term=""RITLUG"" label=""r/RITLUG""/>
    <content type=""html"">Week 14!</content>
    <id>t3_35aov1</id>
    <media:thumbnail url=""https://external-preview.redd.it/XltUh1UUqG8HkKRf4HXGFOQaOrJzsE3lY7Uyl1JlaXs.jpg?width=108&amp;crop=smart&amp;auto=webp&amp;s=b0bf86abc594a5ecdac465ac3028b58f1e454e95""/>
    <link href=""https://old.reddit.com/r/RITLUG/comments/35aov1/week_14_meeting_last_one/""/>
    <updated>2015-05-08T15:00:24+00:00</updated>
    <published>2015-05-08T15:00:24+00:00</published>
    <title>Week 14 Meeting (Last One!)</title>
  </entry>
  <entry>
    <author>
      <name>/u/ritlug</name>
      <uri>https://old.reddit.com/user/ritlug</uri>
    </author>
    <category term=""RITLUG"" label=""r/RITLUG""/>
    <content type=""html"">Week 8</content>
    <id>t3_2zmtur</id>
    <media:thumbnail url=""https://external-preview.redd.it/XltUh1UUqG8HkKRf4HXGFOQaOrJzsE3lY7Uyl1JlaXs.jpg?width=108&amp;crop=smart&amp;auto=webp&amp;s=b0bf86abc594a5ecdac465ac3028b58f1e454e95""/>
    <link href=""https://old.reddit.com/r/RITLUG/comments/2zmtur/week_8_meeting_imagine_rit/""/>
    <updated>2015-03-19T22:06:44+00:00</updated>
    <published>2015-03-19T22:06:44+00:00</published>
    <title>Week 8 Meeting (Imagine RIT)</title>
  </entry>
</feed>
";

            const string updatedFeed = 
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<feed xmlns=""http://www.w3.org/2005/Atom"" xmlns:media=""http://search.yahoo.com/mrss/"">
  <category term=""RITLUG"" label=""r/RITLUG""/>
  <updated>2024-12-27T02:41:43+00:00</updated>
  <icon>https://www.redditstatic.com/icon.png/</icon>
  <id>/r/ritlug/.rss</id>
  <link rel=""self"" href=""https://old.reddit.com/r/ritlug/.rss"" type=""application/atom+xml""/>
  <link rel=""alternate"" href=""https://old.reddit.com/r/ritlug/"" type=""text/html""/>
  <subtitle>The official subreddit for the RIT Linux Users Group, RITlug! All announcements and club-related discussions take place on our various chat channels.</subtitle>
  <title>RIT Linux Users Group</title>
  <entry>
    <author>
      <name>/u/ryan77627</name>
      <uri>https://old.reddit.com/user/ryan77627</uri>
    </author>
    <category term=""RITLUG"" label=""r/RITLUG""/>
    <content type=""html"">Where to find us!</content>
    <id>t3_10kg65u</id>
    <link href=""https://old.reddit.com/r/RITLUG/comments/10kg65u/where_to_find_us_links/""/>
    <updated>2023-01-24T21:02:43+00:00</updated>
    <published>2023-01-24T21:02:43+00:00</published>
    <title>Where to find us + Links</title>
  </entry>
  <entry>
    <author>
      <name>/u/ritlug</name>
      <uri>https://old.reddit.com/user/ritlug</uri>
    </author>
    <category term=""RITLUG"" label=""r/RITLUG""/>
    <content type=""html"">Week 14!</content>
    <id>t3_35aov1</id>
    <media:thumbnail url=""https://external-preview.redd.it/XltUh1UUqG8HkKRf4HXGFOQaOrJzsE3lY7Uyl1JlaXs.jpg?width=108&amp;crop=smart&amp;auto=webp&amp;s=b0bf86abc594a5ecdac465ac3028b58f1e454e95""/>
    <link href=""https://old.reddit.com/r/RITLUG/comments/35aov1/week_14_meeting_last_one/""/>
    <updated>2015-05-08T15:00:24+00:00</updated>
    <published>2015-05-08T15:00:24+00:00</published>
    <title>Week 14 Meeting (Last One!)</title>
  </entry>
  <entry>
    <author>
      <name>/u/ritlug</name>
      <uri>https://old.reddit.com/user/ritlug</uri>
    </author>
    <category term=""RITLUG"" label=""r/RITLUG""/>
    <content type=""html"">Imagine RIT!</content>
    <id>t3_34mm0c</id>
    <media:thumbnail url=""https://external-preview.redd.it/XltUh1UUqG8HkKRf4HXGFOQaOrJzsE3lY7Uyl1JlaXs.jpg?width=108&amp;crop=smart&amp;auto=webp&amp;s=b0bf86abc594a5ecdac465ac3028b58f1e454e95""/>
    <link href=""https://old.reddit.com/r/RITLUG/comments/34mm0c/imagine_rit_today/""/>
    <updated>2015-05-02T15:08:43+00:00</updated>
    <published>2015-05-02T15:08:43+00:00</published>
    <title>Imagine RIT Today!</title>
  </entry>
  <entry>
    <author>
      <name>/u/ritlug</name>
      <uri>https://old.reddit.com/user/ritlug</uri>
    </author>
    <category term=""RITLUG"" label=""r/RITLUG""/>
    <content type=""html"">Week 8</content>
    <id>t3_2zmtur</id>
    <media:thumbnail url=""https://external-preview.redd.it/XltUh1UUqG8HkKRf4HXGFOQaOrJzsE3lY7Uyl1JlaXs.jpg?width=108&amp;crop=smart&amp;auto=webp&amp;s=b0bf86abc594a5ecdac465ac3028b58f1e454e95""/>
    <link href=""https://old.reddit.com/r/RITLUG/comments/2zmtur/week_8_meeting_imagine_rit/""/>
    <updated>2015-03-19T22:06:44+00:00</updated>
    <published>2015-03-19T22:06:44+00:00</published>
    <title>Week 8 Meeting (Imagine RIT)</title>
  </entry>
</feed>
";

            const string missingPost = 
@"<?xml version=""1.0"" encoding=""UTF-8""?>
<feed xmlns=""http://www.w3.org/2005/Atom"" xmlns:media=""http://search.yahoo.com/mrss/"">
  <category term=""RITLUG"" label=""r/RITLUG""/>
  <updated>2024-12-27T02:41:43+00:00</updated>
  <icon>https://www.redditstatic.com/icon.png/</icon>
  <id>/r/ritlug/.rss</id>
  <link rel=""self"" href=""https://old.reddit.com/r/ritlug/.rss"" type=""application/atom+xml""/>
  <link rel=""alternate"" href=""https://old.reddit.com/r/ritlug/"" type=""text/html""/>
  <subtitle>The official subreddit for the RIT Linux Users Group, RITlug! All announcements and club-related discussions take place on our various chat channels.</subtitle>
  <title>RIT Linux Users Group</title>
  <entry>
    <author>
      <name>/u/ryan77627</name>
      <uri>https://old.reddit.com/user/ryan77627</uri>
    </author>
    <category term=""RITLUG"" label=""r/RITLUG""/>
    <content type=""html"">Where to find us!</content>
    <id>t3_10kg65u</id>
    <link href=""https://old.reddit.com/r/RITLUG/comments/10kg65u/where_to_find_us_links/""/>
    <updated>2023-01-24T21:02:43+00:00</updated>
    <published>2023-01-24T21:02:43+00:00</published>
    <title>Where to find us + Links</title>
  </entry>
  <entry>
    <author>
      <name>/u/ritlug</name>
      <uri>https://old.reddit.com/user/ritlug</uri>
    </author>
    <category term=""RITLUG"" label=""r/RITLUG""/>
    <content type=""html"">Week 14!</content>
    <id>t3_35aov1</id>
    <media:thumbnail url=""https://external-preview.redd.it/XltUh1UUqG8HkKRf4HXGFOQaOrJzsE3lY7Uyl1JlaXs.jpg?width=108&amp;crop=smart&amp;auto=webp&amp;s=b0bf86abc594a5ecdac465ac3028b58f1e454e95""/>
    <link href=""https://old.reddit.com/r/RITLUG/comments/35aov1/week_14_meeting_last_one/""/>
    <updated>2015-05-08T15:00:24+00:00</updated>
    <published>2015-05-08T15:00:24+00:00</published>
    <title>Week 14 Meeting (Last One!)</title>
  </entry>
  <entry>
    <author>
      <name>/u/ritlug</name>
      <uri>https://old.reddit.com/user/ritlug</uri>
    </author>
    <category term=""RITLUG"" label=""r/RITLUG""/>
    <content type=""html"">Week 8</content>
    <id>t3_2zmtur</id>
    <media:thumbnail url=""https://external-preview.redd.it/XltUh1UUqG8HkKRf4HXGFOQaOrJzsE3lY7Uyl1JlaXs.jpg?width=108&amp;crop=smart&amp;auto=webp&amp;s=b0bf86abc594a5ecdac465ac3028b58f1e454e95""/>
    <link href=""https://old.reddit.com/r/RITLUG/comments/2zmtur/week_8_meeting_imagine_rit/""/>
    <updated>2015-03-19T22:06:44+00:00</updated>
    <published>2015-03-19T22:06:44+00:00</published>
    <title>Week 8 Meeting (Imagine RIT)</title>
  </entry>
</feed>";
        
            string fileLocation = Path.Combine( testDir.FullName, "RitlugReddit.xml" );
            Uri url = new Uri( SethPath.ToUri( fileLocation ) );

            var rauConfig = new RauConfig( null )
            {
                DefaultLanguages = new List<string>() { "en-US" }
            };
            
            var config = new FeedConfig(
                url,
                "ritlug.com",
                "SomePassword",
                new Uri( "https://at.shendrick.net" ),
                "0 0 * * * ?",
                null,
                null,
                ["en-US"],
                true
            );
            
            var uut = new FeedReader( Client, config );
            
            // Act
            File.WriteAllText( fileLocation, initialFeed );
            uut.Initialize();
            
            File.WriteAllText( fileLocation, updatedFeed );
            List<SyndicationItem> firstPass = uut.UpdateAsync().Result;
            List<SyndicationItem> secondPass = uut.UpdateAsync().Result;

            File.WriteAllText( fileLocation, missingPost );
            List<SyndicationItem> passWithMissingPost = uut.UpdateAsync().Result;
            
            // Check
            Assert.AreEqual(
                "RIT Linux Users Group",
                uut.FeedTitle
            );
            Assert.IsNull( uut.FeedLanguage ); // No language specified.  Leave null.
            
            Assert.AreEqual( 1, firstPass.Count );
            Assert.AreEqual( 0, secondPass.Count );
            Assert.AreEqual( 0, passWithMissingPost.Count );

            var expectedPost1 = new PdsPost
            {
                Languages = ["en-US"],
                PostAttachmentPage = new Uri( "https://old.reddit.com/r/RITLUG/comments/34mm0c/imagine_rit_today/" ),
                PostContents = "RIT Linux Users Group: Imagine RIT Today!"
            };
            PdsPost post1 = firstPass[0].GeneratePost( uut, rauConfig );
            AssertEx.ArePdsPostsEqual( expectedPost1, post1 );
        }
    }
}