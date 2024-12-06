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

namespace Rau.Standard
{
    public interface IRauPlugin : IDisposable
    {
        // ---------------- Properties ----------------

        /// <summary>
        /// The name of the plugin.
        /// </summary>
        string PluginName { get; }

        /// <summary>
        /// The GUID of the plugin.
        /// </summary>
        Guid PluginGuid { get; }

        /// <summary>
        /// The version of the plugin.
        /// </summary>
        Version PluginVersion { get; }

        // ---------------- Methods ----------------

        /// <summary>
        /// Initializes this plugin and passes in the API.
        /// </summary>
        void Initialize( IRauApi api );
    }
}
