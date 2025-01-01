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

using Rau.Standard.Configuration;
using Rau.Standard.Logging;

namespace Rau.Configuration
{
    public static class IRauConfiguratorExtensions
    {
        /// <summary>
        /// Sets the persistence directory for Rau.
        /// </summary>
        public static void UsePersistenceDirectory( this IRauConfigurator rau, string newDirectory )
        {
            rau.UsePersistenceDirectory( new DirectoryInfo( newDirectory ) );
        }

        /// <summary>
        /// Sets the persistence directory for Rau.
        /// </summary>
        public static void UsePersistenceDirectory( this IRauConfigurator rau, DirectoryInfo newDirectory )
        {
            RauConfig config = rau.Config with
            {
                PersistenceLocation = newDirectory
            };

            rau.Configure( config );
        }

        /// <summary>
        /// Sets the maximum character limit for messages.
        /// This value must be greater than zero.
        /// </summary>
        public static void SetCharacterLimit( this IRauConfigurator rau, uint newValue )
        {
            RauConfig config = rau.Config with
            {
                CharacterLimit = newValue
            };

            rau.Configure( config );
        }

        /// <summary>
        /// Enables file logging if called.
        /// </summary>
        /// <param name="filePath">The absolute file path to write the log file to.</param>
        /// <param name="logLevel">
        /// <seealso cref="RauConfig.LogFileLevel"/>.
        /// Leave null use the default level.
        /// </param>
        public static void LogToFile(
            this IRauConfigurator rau,
            string filePath,
            RauLogLevel? logLevel = null
        )
        {
            rau.LogToFile( new FileInfo( filePath ), logLevel );
        }

        /// <summary>
        /// Enables file logging if called.
        /// </summary>
        /// <param name="filePath">The absolute file path to write the log file to.</param>
        /// <param name="logLevel">
        /// <seealso cref="RauConfig.LogFileLevel"/>.
        /// Leave null use the default level.
        /// </param>
        public static void LogToFile(
            this IRauConfigurator rau,
            FileInfo filePath,
            RauLogLevel? logLevel = null
        )
        {
            RauConfig config = rau.Config with
            {
                LogFile = filePath
            };

            if( logLevel is not null )
            {
                config = config with
                {
                    LogFileLevel = logLevel.Value
                };
            }

            rau.Configure( config );
        }

        public static void SetConsoleLogLevel( this IRauConfigurator rau, RauLogLevel level )
        {
            RauConfig config = rau.Config with
            {
                ConsoleLogLevel = level
            };

            rau.Configure( config );
        }

        /// <summary>
        /// Enables Prometheus metrics that can be scraped at the given port.
        /// The port must be greater than 0.
        /// </summary>
        public static void UseMetricsAtPort( this IRauConfigurator rau, ushort newValue )
        {
            RauConfig config = rau.Config with
            {
                MetricsPort = newValue
            };

            rau.Configure( config );
        }

        /// <summary>
        /// Enables logging any warnings or greater messages to Telegram
        /// </summary>
        public static void LogToTelegram(
            this IRauConfigurator rau,
            string telegramBotToken,
            string telegramChatId
        )
        {
            RauConfig config = rau.Config with
            {
                TelegramBotToken = telegramBotToken,
                TelegramChatId = telegramChatId
            };

            rau.Configure( config );
        }

        /// <summary>
        /// Sets the user agent name when sending HTTP requests
        /// to a PDS.
        /// </summary>
        public static void OverridePdsUserAgent(
            this IRauConfigurator rau,
            string userAgentName,
            Version? userAgentVersion
        )
        {
            RauConfig config = rau.Config with
            {
                UserAgentName = userAgentName,
                UserAgentVersion = userAgentVersion
            };

            rau.Configure( config );
        }
    }
}
