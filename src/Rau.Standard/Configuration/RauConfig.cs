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

namespace Rau.Standard.Configuration
{
    public record class RauConfig
    {
        // ---------------- Constructor ----------------
        
        internal RauConfig( Version? assemblyVersion )
        {
            this.UserAgentVersion = assemblyVersion;
        }
        
        // ---------------- Properties ----------------
        
        /// <summary>
        /// The character limit of a post.
        /// Defaulted to Blue Sky's limit.
        /// </summary>
        public uint CharacterLimit { get; init; } = 300;
        
        /// <summary>
        /// File to write log messages to.
        /// Set to null to disable logging messages to a file.
        /// </summary>
        public FileInfo? LogFile { get; init; }
        
        /// <summary>
        /// The port to expose for Prometheus metrics.
        /// Set to null to not enable metrics.
        /// </summary>
        public ushort? MetricsPort { get; init; }
        
        /// <summary>
        /// The Telegram bot token to use for logging messages.  See more information
        /// on how to do that:
        /// https://docs.teleirc.com/en/latest/user/quick-start/#create-a-telegram-bot
        ///
        /// Set to null to not log to Telegram.
        /// </summary>
        public string? TelegramBotToken { get; init; }

        /// <summary>
        /// The chat ID of the Telegram chat to log to.
        ///
        /// Set to null to not log to Telegram.
        /// </summary>
        public string? TelegramChatId { get; init; }

        /// <summary>
        /// The user agent name to use when posting to the PDS.
        /// </summary>
        public string UserAgentName { get; init; } = "rau";

        /// <summary>
        /// The version to use when posting to the PDS.
        /// </summary>
        public Version? UserAgentVersion { get; init; }
    }
}