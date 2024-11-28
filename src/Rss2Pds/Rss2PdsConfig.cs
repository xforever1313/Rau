//
// Rss2Pds - A bot that reads RSS feeds and posts them to a AT-Proto PDS node
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

using SethCS.Exceptions;
using SethCS.Extensions;

namespace Rss2Pds
{
    public class Rss2PdsConfig
    {
        // ---------------- Constructor ----------------

        public Rss2PdsConfig()
        {
            this.BlueSkyUser = Environment.GetEnvironmentVariable( "BSKY_USER" ) ?? string.Empty;
            this.BlueSkyPassword = Environment.GetEnvironmentVariable( "BSKY_PASSWORD" ) ?? string.Empty;

            // Default to every hour on the hour.
            this.CronString = Environment.GetEnvironmentVariable( "CRON_STRING" ) ?? "0 0 * * * ?";

            this.Url = new Uri(
                Environment.GetEnvironmentVariable( "PDS_URL" ) ?? "https://bsky.social"
            );

            {
                string logFile = Environment.GetEnvironmentVariable( "LOG_FILE" ) ?? string.Empty;
                if( string.IsNullOrWhiteSpace( logFile ) == false )
                {
                    this.LogFile = new FileInfo( logFile );
                }
            }

            {
                string telegramBotToken = Environment.GetEnvironmentVariable( "TELEGRAM_BOT_TOKEN" ) ?? string.Empty;
                if( string.IsNullOrWhiteSpace( telegramBotToken ) == false )
                {
                    this.TelegramBotToken = telegramBotToken;
                }
            }

            {
                string telegramChatId = Environment.GetEnvironmentVariable( "TELEGRAM_CHAT_ID" ) ?? string.Empty;
                if( string.IsNullOrWhiteSpace( telegramChatId ) == false )
                {
                    this.TelegramChatId = telegramChatId;
                }
            }
        }

        // ---------------- Properties ----------------

        public int Port => 9105;

        public string BlueSkyUser { get; }

        public string BlueSkyPassword { get; }

        public FileInfo? LogFile { get; }

        public string? TelegramBotToken { get; }

        public string? TelegramChatId { get; }

        public string ApplicationContext => "PDS Canary Bot";

        public string CronString { get; }

        public Uri Url { get; }

        // ---------------- Functions ----------------

        public bool TryValidate( out string error )
        {
            var errors = new List<string>();

            if( string.IsNullOrWhiteSpace( this.BlueSkyUser ) )
            {
                errors.Add( "BSKY_USER env var not specfied" );
            }

            if( string.IsNullOrEmpty( this.BlueSkyPassword ) )
            {
                errors.Add( "BSKY_PASSWORD env var not specified" );
            }

            if( errors.Any() )
            {
                error = errors.ToListString( "-" );
                return false;
            }
            else
            {
                error = string.Empty;
                return true;
            }
        }

        public void Validate()
        {
            bool success = TryValidate( out string error );
            if( success == false )
            {
                throw new ValidationException( error );
            }
        }
    }
}
