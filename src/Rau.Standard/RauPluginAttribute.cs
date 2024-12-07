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

namespace Rau.Standard
{
    /// <summary>
    /// The attribute that should be used on the class that implements
    /// <see cref="IRauPlugin"/>.
    /// 
    /// The combination of the two will match the plugin ID with the plugin implementation.
    /// </summary>
    [AttributeUsage( AttributeTargets.Class, AllowMultiple = false )]
    public class RauPluginAttribute : Attribute
    {
        // ---------------- Constructor ----------------

        /// <param name="pluginId">
        /// The GUID in string form (can not pass in a GUID at compile time, which attributes require).
        /// </param>
        public RauPluginAttribute( string pluginId )
        {
            this.PluginId = pluginId;
        }

        // ---------------- Properties ----------------

        public string PluginId { get; }
    }
}
