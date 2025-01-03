﻿//
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
using Rau.Standard.EventScheduler;
using Rau.Standard.Logging;

namespace Rau.Standard
{
    /// <summary>
    /// The main interface into the Rau system.
    ///
    /// Plugin writers can write extension methods to this interface
    /// to allow plugin configuration during the initialize stage.
    /// </summary>
    public interface IRauApi
    {
        // ---------------- Properties ----------------

        /// <summary>
        /// Global configuration of Rau.
        /// </summary>
        RauConfig Config { get; }

        /// <summary>
        /// Provides the ability to get the current timestamp of the system.
        /// </summary>
        IDateTimeFactory DateTime { get; }

        /// <summary>
        /// Allows events to be scheduled.
        /// </summary>
        IScheduledEventManager EventScheduler { get; }

        /// <summary>
        /// Allows one to log messages without any context associated with them.
        /// It is not recommended to invoke this via plugins, rather use
        /// a plugin's <see cref="IRauPluginInitializationArgs.PluginLogger"/> instead.
        /// This should only be used during configuration of the bot, or when logging a message
        /// in a plugin where a plugin context doesn't make sense for some reason.
        /// </summary>
        IRauLogger Logger { get; }

        /// <summary>
        /// Allows one to post messages to a PDS.
        /// </summary>
        IPdsPoster PdsPoster { get; }

        /// <summary>
        /// List of all loaded plugins.
        /// </summary>
        IReadOnlyDictionary<Guid, IRauPlugin> Plugins { get; }
    }

    public static class IRauApiExtensions
    {
        public static TPlugin GetPlugin<TPlugin>( this IRauApi api, Guid pluginId )
        {
            if( api.Plugins.ContainsKey( pluginId ) == false )
            {
                throw new ArgumentException( $"Can not find plugin with ID: {pluginId}.", nameof( pluginId ) );
            }

            return (TPlugin)api.Plugins[pluginId];
        }
    }
}
