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

using Rau.Standard;
using Rau.Standard.Configuration;
using Rau.Standard.Logging;

[assembly:RauConfigurationNamespace( $"Rau.Plugins.Canary" )]

namespace Rau.Plugins.Canary
{
    [RauPlugin( PluginId )]
    public sealed class CanaryPlugin : IRauPlugin
    {
        // ---------------- Fields ----------------

        internal const string PluginId = "C603CF33-1056-4794-BF3D-743200FBB79B";

        public static readonly Guid PluginGuid = Guid.Parse( PluginId );

        private IRauApi? api;

        private IRauLogger? pluginLogger;

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

        internal IRauLogger PluginLogger
        {
            get
            {
                if( this.pluginLogger is null )
                {
                    throw new InvalidOperationException( $"Tried to grab the plugin logger before {nameof( Initialize )} was called." );
                }

                return this.pluginLogger;
            }
        }

        // ---------------- Methods ----------------

        /// <summary>
        /// The default post this plugin will produce.
        /// </summary>
        public static PdsPost DefaultPost( IDateTimeFactory dateTimeFactory, PdsAccount account )
        {
            var uptime = TimeSpan.FromMilliseconds( Environment.TickCount64 );
            DateTime timeStamp = dateTimeFactory.Now;

            return DefaultPost( timeStamp, uptime, account );
        }

        public static PdsPost DefaultPost( DateTime timeStamp, TimeSpan uptime, PdsAccount account )
        {            
            string post = $"Chirp! The PDS at {account.Instance.Host} is still online as of: {timeStamp:dddd, MMMM d yyyy, h:mmtt} server time.{Environment.NewLine}" +
                          $"Server's been up for {uptime.Days} days, {uptime.Hours} hours. #Uptime";

            return new PdsPost( post );
        }
        
        public void Initialize( IRauApi api, IRauPluginInitializationArgs initArgs )
        {
            this.api = api;
            this.pluginLogger = initArgs.PluginLogger;

            this.accountManager = new AccountManager( this.Api );
        }

        public void Dispose()
        {
            this.api = null;
        }
    }
}
