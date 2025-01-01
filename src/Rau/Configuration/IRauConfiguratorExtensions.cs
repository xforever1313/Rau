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

namespace Rau.Configuration
{
    public static class IRauConfiguratorExtensions
    {
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
        /// <param name="filePath">The absolute file path to msLogger to.</param>
        public static void LogToFile( this IRauConfigurator rau, string filePath )
        {
            rau.LogToFile( new FileInfo( filePath ) );
        }

        /// <summary>
        /// Enables file logging if called.
        /// </summary>
        /// <param name="fileName">The file name to msLogger to.</param>
        public static void LogToFile( this IRauConfigurator rau, FileInfo filePath )
        {
            RauConfig config = rau.Config with
            {
                LogFile = filePath
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
