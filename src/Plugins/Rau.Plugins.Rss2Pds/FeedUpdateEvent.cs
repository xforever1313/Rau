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

using System.ServiceModel.Syndication;
using Rau.Standard;
using Rau.Standard.EventScheduler;
using Rau.Standard.Logging;

namespace Rau.Plugins.Rss2Pds
{
    internal sealed class FeedUpdateEvent : ScheduledEvent
    {
        // ---------------- Fields ----------------

        private readonly FeedReader feedReader;

        private readonly IRauLogger pluginLogger;

        private bool alerted;

        // ---------------- Constructor ----------------

        public FeedUpdateEvent( FeedReader feedReder, IRauLogger pluginLogger )
        {
            this.feedReader = feedReder;
            this.pluginLogger = pluginLogger;

            this.FailedFetches = 0;
        }

        // ---------------- Properties ----------------

        public override string CronString => this.feedReader.FeedConfig.CronString;

        /// <summary>
        /// The number of failed fetches in a row.
        /// This is reset is a fetch is successful.
        /// </summary>
        public uint FailedFetches { get; private set; }

        // ---------------- Methods ----------------

        public override async Task ExecuteEvent( IScheduledEventParameters eventParams )
        {
            IRauApi api = eventParams.Api;

            try
            {
                this.pluginLogger.Debug( $"Checking feed for updates: {this.feedReader.FeedConfig.FeedUrl}" );
                List<SyndicationItem> updatedItems = await this.feedReader.UpdateAsync();

                if( updatedItems.Count == 0 )
                {
                    this.pluginLogger.Debug( $"No new items found in feed: {this.feedReader.FeedConfig.FeedUrl}" );
                }
                else
                {
                    this.pluginLogger.Debug( $"{updatedItems.Count} new items found in feed: {this.feedReader.FeedConfig.FeedUrl}" );

                    PdsAccount account = this.feedReader.FeedConfig.ToPdsAccount();

                    foreach( SyndicationItem item in updatedItems )
                    {
                        PdsPost post = item.GeneratePost( this.feedReader, eventParams.Api.Config );
                        await api.PdsPoster.Post( account, post );
                        await Task.Delay( this.feedReader.FeedConfig.PostDelayTime, eventParams.CancellationToken );
                    }
                }

                this.FailedFetches = 0;
                this.alerted = false;
            }
            catch( TaskCanceledException )
            {
                return;
            }
            catch( Exception e )
            {
                ++this.FailedFetches;
                if(
                    ( this.FailedFetches > this.feedReader.FeedConfig.AlertThreshold ) &&
                    ( this.alerted == false )
                )
                {
                    this.pluginLogger.Warning(
                        $"Unable to fetch feed '{this.feedReader.FeedConfig.FeedUrl}' after {this.feedReader.FeedConfig.AlertThreshold} attempts.  Most recent exception: {Environment.NewLine}{e}"
                    );
                    this.alerted = true;
                }
                else
                {
                    this.pluginLogger.Debug(
                        $"Error when fetching feed: {this.feedReader.FeedConfig.FeedUrl}{Environment.NewLine}{e}"
                    );
                }
            }
        }
    }
}
