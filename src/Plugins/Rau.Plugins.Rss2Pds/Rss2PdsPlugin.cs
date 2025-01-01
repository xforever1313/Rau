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

using System.Net.Http.Headers;
using Rau.Standard;
using Rau.Standard.Configuration;

[assembly: RauConfigurationNamespace( $"Rau.Plugins.Rss2Pds" )]

namespace Rau.Plugins.Rss2Pds
{
    [RauPlugin( PluginId )]
    public sealed class Rss2PdsPlugin : IRauPlugin
    {
        // ---------------- Fields ----------------

        internal const string PluginId = "9022224B-60D6-403A-B2F0-FC74DEBD7E04";

        public static readonly Guid PluginGuid = Guid.Parse( PluginId );

        private readonly HttpClient httpClient;

        private IRauApi? api;

        private FeedManager? feedManager;

        // ---------------- Constructor ----------------

        public Rss2PdsPlugin()
        {
            this.PluginVersion = GetType().Assembly.GetName().Version ?? new Version( 0, 0, 0 );
            this.httpClient = new HttpClient();

            this.httpClient.DefaultRequestHeaders.UserAgent.Clear();
            this.httpClient.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue( "Rau.Plugins.Rss2Pds", this.PluginVersion.ToString( 3 ) )
            );
        }

        // ---------------- Properties ----------------

        public string PluginName => "Canary";

        public Version PluginVersion { get; }

        Guid IRauPlugin.PluginGuid => PluginGuid;

        internal IRauApi Api
        {
            get
            {
                if( this.api is null )
                {
                    throw new InvalidOperationException( $"Tried to grab the API before {nameof( Initialize )} was called." );
                }

                return this.api;
            }
        }

        public FeedManager FeedManager =>
            this.feedManager ?? throw new InvalidOperationException( $"Tried to access feed manager before {nameof( Initialize )} was called." );

        // ---------------- Methods ----------------

        public void Initialize( IRauApi api, IRauPluginInitializationArgs initArgs )
        {
            this.api = api;
            this.feedManager = new FeedManager( this.httpClient, this.api );
        }

        public void Dispose()
        {
            this.feedManager?.RemoveAllFeeds();
            this.feedManager = null;
            this.api = null;
        }
    }
}
