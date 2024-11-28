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

using Quartz;
using Serilog.Extensions.Logging;
using X.Bluesky;

namespace Rss2Pds
{
    public sealed class SkeetJob : IJob
    {
        // ---------------- Fields ----------------

        private readonly Serilog.ILogger log;

        private readonly BlueskyClient client;

        // ---------------- Constructor ----------------

        public SkeetJob( Serilog.ILogger log, RssFeedConfig config, IHttpClientFactory httpClient )
        {
            this.log = log;

            var microsoftLogger = new SerilogLoggerFactory( log );
            this.client = new BlueskyClient(
                httpClient,
                config.Handle,
                config.Password,
                new string[] { "en", "en-US" },
                true,
                config.FeedUrl,
                microsoftLogger.CreateLogger<BlueskyClient>()
            );
        }

        // ---------------- Methods ----------------

        public async Task Execute( IJobExecutionContext context )
        {
            try
            {
                DateTime timeStamp = context.FireTimeUtc.DateTime;

                log.Information( "Sending Message..." );
                await this.client.Post( "post text" );
                log.Information( "Sending Message...Done!" );
            }
            catch( Exception e )
            {
                this.log.Error( $"Error sending message: {Environment.NewLine}{e}" );
            }
        }
    }
}
