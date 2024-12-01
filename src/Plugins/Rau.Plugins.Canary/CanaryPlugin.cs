//
// Rau - A bot that reads RSS feeds and posts them to a AT-Proto PDS node
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

using Rau.Standard;

namespace Rau.Plugins.Canary
{
    [RauPlugin( PluginId )]
    public sealed class CanaryPlugin : IRauPlugin
    {
        // ---------------- Fields ----------------

        internal const string PluginId = "{C603CF33-1056-4794-BF3D-743200FBB79B}";

        public static readonly Guid PluginGuid = Guid.Parse( PluginId );

        private IRauApi? api;

        private AccountManager? accountManager;

        // ---------------- Constructor ----------------

        public CanaryPlugin()
        {
            this.PluginVersion = GetType().Assembly.GetName().Version ?? new Version( 0, 0, 0 );
        }

        // ---------------- Properties ----------------

        public string PluginName => "Canary";

        public Version PluginVersion { get; }

        public AccountManager AccountManager
        {
            get
            {
                if( this.accountManager is null )
                {
                    throw new InvalidOperationException( $"Tried to account manager before {nameof( Initialize )} was called." );
                }

                return this.accountManager;
            }
        }

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

        // ---------------- Methods ----------------

        public void Initialize( IRauApi api )
        {
            this.api = api;
            this.accountManager = new AccountManager( this.Api );
        }

        public void Dispose()
        {
            this.api = null;
        }
    }
}
