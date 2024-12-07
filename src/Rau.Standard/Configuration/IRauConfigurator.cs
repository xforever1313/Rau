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

namespace Rau.Standard.Configuration
{
    /// <summary>
    /// Interface that allows a user to configure
    /// Rau's global configuration.
    ///
    /// Plugin writers can write extension methods
    /// to this interface to allow configuration of their
    /// plugins during the configuration stage.
    /// </summary>
    public interface IRauConfigurator
    {
        // ---------------- Properties ----------------

        RauConfig Config { get; }

        // ---------------- Methods ----------------
        
        void Configure( RauConfig config );
    }
}