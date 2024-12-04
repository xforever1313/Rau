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
using Rau.Standard.Configuration;

namespace Rau.Configuration
{
    internal static class RauConfigExtensions
    {
        /// <summary>
        /// Tries to validate the configuration object.
        /// Returns an empty list if there are no configuration failures.
        /// </summary>
        public static List<string> TryValidate( this RauConfig config )
        {
            var errors = new List<string>();

            return errors;
        }
    }
}