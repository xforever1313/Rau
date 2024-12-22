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
//++failedFetches;

using System.ServiceModel.Syndication;
using Rau.Standard;
using Rau.Standard.EventScheduler;

namespace Rau.Plugins.Rss2Pds
{
    internal sealed class FeedUpdateEvent : ScheduledEvent
    {
        // ---------------- Fields ----------------

        private readonly IRauApi api;

        private readonly FeedReader feedReader;

        private uint failedFetches;
        private bool alerted;

        // ---------------- Constructor ----------------

        public FeedUpdateEvent( IRauApi api, FeedReader feedReder )
        {
            this.api = api;
            this.feedReader = feedReder;

            this.failedFetches = 0;
        }

        // ---------------- Properties ----------------

        public override string CronString => this.feedReader.FeedConfig.CronString;

        // ---------------- Methods ----------------

        public override async Task ExecuteEvent( IScheduledEventParameters eventParams )
        {
            try
            {
                this.api.Logger.Information( $"Checking feed for updates: {this.feedReader.FeedConfig.FeedUrl}" );
                List<SyndicationItem> updatedItems = await this.feedReader.UpdateAsync();

                if( updatedItems.Count == 0 )
                {
                    this.api.Logger.Information( $"No new items found in feed: {this.feedReader.FeedConfig.FeedUrl}" );
                }

                this.failedFetches = 0;
                this.alerted = false;
            }
            catch( Exception e )
            {
                ++failedFetches;
                if(
                    ( failedFetches >= this.feedReader.FeedConfig.AlertThreshold ) &&
                    ( alerted == false )
                )
                {
                    this.api.Logger.Warning(
                        $"Unable to fetch feed '{this.feedReader.FeedConfig.FeedUrl}' after {this.feedReader.FeedConfig.AlertThreshold} attempts.  Most recent exception: {Environment.NewLine}{e}"
                    );
                    this.alerted = true;
                }
                else
                {
                    this.api.Logger.Debug(
                        $"Error when fetching feed: {this.feedReader.FeedConfig.FeedUrl}{Environment.NewLine}{e}"
                    );
                }
            }
        }
    }
}
