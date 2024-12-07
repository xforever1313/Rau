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

namespace Rau.Tests.Rau.Configuration
{
    [TestClass]
    public sealed class IRauConfiguratorExtensionsTests
    {
        // ---------------- Fields ----------------

        private RauConfigurator? uut;

        // ---------------- Setup / Teardown ----------------

        [TestInitialize]
        public void TestSetup()
        {
            this.uut = new RauConfigurator( null );
        }

        [TestCleanup]
        public void TestTeardown()
        {
        }

        // ---------------- Properties ----------------

        private RauConfigurator Uut
        {
            get
            {
                Assert.IsNotNull( this.uut );
                return this.uut;
            }
        }

        // ---------------- Tests ----------------

        [TestMethod]
        public void SetCharacterLimitTest()
        {
            // Setup
            const uint newValue = 1000;

            // Act
            this.Uut.SetCharacterLimit( newValue );

            // Check
            Assert.AreEqual( newValue, this.Uut.Config.CharacterLimit );
        }

        [TestMethod]
        public void SetLogFileAsStringTest()
        {
            // Setup
            string path = GetType().Assembly.Location;

            // Act
            this.Uut.LogToFile( path );

            // Check
            FileInfo? logFileLocation = this.Uut.Config.LogFile;
            Assert.IsNotNull( logFileLocation );
            Assert.AreEqual( new FileInfo( path ).FullName, logFileLocation.FullName );

        }

        [TestMethod]
        public void SetLogFileAsFileInfoTest()
        {
            // Setup
            FileInfo path = new FileInfo( GetType().Assembly.Location );

            // Act
            this.Uut.LogToFile( path );

            // Check
            FileInfo? logFileLocation = this.Uut.Config.LogFile;
            Assert.IsNotNull( logFileLocation );
            Assert.AreEqual( path.FullName, logFileLocation.FullName );
        }

        [TestMethod]
        public void UseMetricsAtPortTest()
        {
            // Setup
            ushort port = 8080;

            // Act
            this.Uut.UseMetricsAtPort( port );
            Assert.AreEqual( port, this.Uut.Config.MetricsPort );
        }

        [TestMethod]
        public void LogToTelegramTest()
        {
            // Setup
            const string telegramBotToken = "000000000:AAAAAAaAAa2AaAAaoAAAA-a_aaAAaAaaaAA";
            const string telegramChatId = "-0000000000000";

            // Act
            this.Uut.LogToTelegram( telegramBotToken, telegramChatId );

            // Check
            Assert.AreEqual( telegramBotToken, this.Uut.Config.TelegramBotToken );
            Assert.AreEqual( telegramChatId, this.Uut.Config.TelegramChatId );
        }

        [TestMethod]
        public void OverridePdsUserAgentTest()
        {
            // Setup
            const string userAgent = "new_pds_user_agent";
            var version = new Version( 1, 2, 3 );

            // Act
            this.Uut.OverridePdsUserAgent( userAgent, version );

            // Check
            Assert.AreEqual( userAgent, this.Uut.Config.UserAgentName );
            Assert.AreEqual( version, this.Uut.Config.UserAgentVersion );
        }
    }
}
