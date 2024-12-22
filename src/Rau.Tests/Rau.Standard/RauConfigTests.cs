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

using Rau.Standard.Configuration;
using SethCS.Exceptions;

namespace Rau.Tests.Rau.Standard
{
    [TestClass]
    public sealed class RauConfigTests
    {
        // ---------------- Tests ----------------

        // -------- Validation Tests --------

        /// <summary>
        /// Ensures the default configuration object
        /// validates.
        /// </summary>
        [TestMethod]
        public void DefaultConfigValidatesTest()
        {
            // Setup
            var versionsToTest = new Version?[]
            {
                null,
                new Version( 1, 2, 3 )
            };

            // Act / Check
            foreach( Version? version in versionsToTest )
            {
                var uut = new RauConfig( version );

                uut.Validate(); // Should not throw.
            }
        }

        /// <summary>
        /// Ensures valid characters does not reuslt in a validation error.
        /// </summary>
        [TestMethod]
        public void ValidCharacterLimitValidationTest()
        {
            // Setup
            var validValues = new uint[]
            {
                1, 2, 10, 100, 300, 1000, uint.MaxValue
            };

            // Act / Check
            foreach( uint value in validValues )
            {
                var uut = new RauConfig( null )
                {
                    CharacterLimit = value
                };

                uut.Validate();
            }
        }

        /// <summary>
        /// Ensures invalid characters results in a validation error.
        /// </summary>
        [TestMethod]
        public void InvalidCharacterLimitValidationTest()
        {
            // Setup
            var invalidValues = new uint[]
            {
                0
            };

            // Act / Check
            foreach( uint value in invalidValues )
            {
                var uut = new RauConfig( null )
                {
                    CharacterLimit = value
                };

                Assert.ThrowsException<ListedValidationException>(
                    () => uut.Validate()
                );
            }
        }

        /// <summary>
        /// Ensures all log files are valid values.
        /// </summary>
        [TestMethod]
        public void LogFileValidationTest()
        {
            // Setup
            var validValues = new FileInfo?[]
            {
                null,
                new FileInfo( "log.txt" )
            };

            // Act / Check
            foreach( FileInfo? value in validValues )
            {
                var uut = new RauConfig( null )
                {
                    LogFile = value
                };

                uut.Validate();
            }
        }

        /// <summary>
        /// Ensures valid metrics ports do not result in an invalid
        /// error.
        /// </summary>
        [TestMethod]
        public void ValidMetricsPortTest()
        {
            // Setup
            var validValues = new ushort?[]
            {
                null,
                1,
                100,
                80,
                443,
                8080,
                9100,
                ushort.MaxValue
            };

            // Act / Check
            foreach( ushort? value in validValues )
            {
                var uut = new RauConfig( null )
                {
                    MetricsPort = value
                };

                uut.Validate();
            }
        }

        /// <summary>
        /// Ensures invalid metrics ports result in an invalid
        /// error.
        /// </summary>
        [TestMethod]
        public void InvalidMetricsPortTest()
        {
            // Setup
            var validValues = new ushort?[]
            {
                0
            };

            // Act / Check
            foreach( ushort? value in validValues )
            {
                var uut = new RauConfig( null )
                {
                    MetricsPort = value
                };

                Assert.ThrowsException<ListedValidationException>(
                    () => uut.Validate()
                );
            }
        }

        /// <summary>
        /// If both telegram settings are specified, no
        /// validation errors should happen.
        /// </summary>
        [TestMethod]
        public void ValidTelegramTest()
        {
            // Setup
            var uut = new RauConfig( null )
            {
                TelegramBotToken = "000000000:AAAAAAaAAa2AaAAaoAAAA-a_aaAAaAaaaAA",
                TelegramChatId = "-0000000000000"
            };

            // Act / Check
            uut.Validate();
        }

        /// <summary>
        /// If one or the other Telegram settings is specifed,
        /// but not the other, this is an invalid configuration.
        /// </summary>
        [TestMethod]
        public void InvalidTelegramTest()
        {
            var uut = new RauConfig( null )
            {
                TelegramBotToken = "000000000:AAAAAAaAAa2AaAAaoAAAA-a_aaAAaAaaaAA",
                TelegramChatId = null
            };
            Assert.ThrowsException<ListedValidationException>( () => uut.Validate() );

            uut = new RauConfig( null )
            {
                TelegramBotToken = "000000000:AAAAAAaAAa2AaAAaoAAAA-a_aaAAaAaaaAA",
                TelegramChatId = ""
            };
            Assert.ThrowsException<ListedValidationException>( () => uut.Validate() );

            uut = new RauConfig( null )
            {
                TelegramBotToken = null,
                TelegramChatId = "-0000000000000"
            };
            Assert.ThrowsException<ListedValidationException>( () => uut.Validate() );

            uut = new RauConfig( null )
            {
                TelegramBotToken = "",
                TelegramChatId = "-0000000000000"
            };
            Assert.ThrowsException<ListedValidationException>( () => uut.Validate() );
        }

        [TestMethod]
        public void ValidUserAgentTest()
        {
            // Setup
            var validUserAgents = new string[]
            {
                "user-agent"
            };

            // Act / Check
            foreach( string value in validUserAgents )
            {
                var uut = new RauConfig( null )
                {
                    UserAgentName = value
                };

                uut.Validate();
            }
        }

        [TestMethod]
        public void InvalidUserAgentTest()
        {
            // Setup
            var invalidUserAgents = new string[]
            {
                "",
                "Invalid User Agent With Space!"
            };

            // Act / Check
            foreach( string value in invalidUserAgents )
            {
                var uut = new RauConfig( null )
                {
                    UserAgentName = value
                };

                Assert.ThrowsException<ListedValidationException>(
                    () => uut.Validate()
                );
            }
        }

        [TestMethod]
        public void ValidUserAgentVersionTest()
        {
            // Setup
            var validUserAgents = new Version?[]
            {
                null,
                new Version( 1, 2, 3 )
            };

            // Act / Check
            foreach( Version? value in validUserAgents )
            {
                var uut = new RauConfig( null )
                {
                    UserAgentVersion = value
                };

                uut.Validate();
            }
        }
    }
}