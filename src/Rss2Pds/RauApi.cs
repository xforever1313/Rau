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

using System.Collections.ObjectModel;
using Rau.EventScheduler;
using Rau.Logging;
using Rau.Standard;
using Rau.Standard.EventScheduler;
using Rau.Standard.Logging;

namespace Rau
{
    internal sealed class RauApi : IRauApi, IDisposable
    {
        // ---------------- Fields ----------------

        private readonly ScheduledEventManager eventManager;
        
        private readonly Dictionary<Guid, IRauPlugin> plugins;
        
        // ---------------- Constructor ----------------

        public RauApi(
            Serilog.ILogger log
        )
        {
            this.plugins = new Dictionary<Guid, IRauPlugin>();
            this.Plugins = new ReadOnlyDictionary<Guid, IRauPlugin>( this.plugins );

            this.eventManager = new ScheduledEventManager( this, log );
            this.DateTime = new RauDateTimeFactory();
            this.Logger = new RauLogger( log );
            this.PdsPoster = new PdsPoster();
        }
        
        // ---------------- Properties ----------------

        public IDateTimeFactory DateTime { get; }

        public IScheduledEventManager EventScheduler => this.eventManager;
        
        public IRauLogger Logger { get; }

        public IPdsPoster PdsPoster { get; }
        
        public IReadOnlyDictionary<Guid, IRauPlugin> Plugins { get; }
        
        // ---------------- Methods ----------------
        
        // TODO:
        // - Load Plugins
        // - Compile Configuration
        // - Start scheduled events
        
        public void Dispose()
        {
            foreach( KeyValuePair<Guid, IRauPlugin> plugin in this.plugins.ToArray() )
            {
                IDisposable dispose = plugin.Value;
                try
                {
                    dispose.Dispose();
                    this.plugins.Remove( plugin.Key );
                }
                catch( Exception e )
                {
                    this.Logger.Error(
                        $"Error when disposing plugin {plugin.Value.PluginName}:{Environment.NewLine}{e}"
                    );
                }
            }
            
            this.eventManager.Dispose();
        }
    }
}