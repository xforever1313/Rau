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
    }
}