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
    public record class PdsAccount
    {
        // ---------------- Constructor ----------------

        /// <summary>
        /// The user name to login as.
        /// </summary>
        public required string UserName { get; init; }

        /// <summary>
        /// The password to login with.  It is strongly recommended to use app-passwords
        /// if using blue sky.
        /// </summary>
        public required string Password { get; init; }

        /// <summary>
        /// The URL to the PDS instance.  Defaulted to blue sky's.
        /// </summary>
        public required Uri Instance { get; init; } = new Uri( "https://bsky.social" );
    }
}
