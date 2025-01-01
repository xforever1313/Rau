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

using Rau.Standard.Logging;

namespace Rau.Standard
{
    public interface IRauPluginInitializationArgs
    {
        /// <summary>
        /// The absolute path of the recommended spot to put files that
        /// need to persist between when Rau shuts down and starts up again
        /// for this plugin.
        /// </summary>
        /// <remarks>
        /// The directory is not automatically created in case a plugin does not
        /// require persistence.  If a plugin does require persistence, it should
        /// create the directory if it doesn't exist inside of
        /// <see cref="IRauPlugin.Initialize"/>.
        /// </remarks>
        DirectoryInfo PersistenceLocation { get; }

        /// <summary>
        /// The log that is bound to this plugin.  Writing to this
        /// will prefix the log with this plugin's name.
        /// </summary>
        IRauLogger PluginLogger { get; }
    }
}
