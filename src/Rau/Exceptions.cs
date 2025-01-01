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

using SethCS.Exceptions;

namespace Rau
{
    /// <summary>
    /// Exception that is thrown when there's an exception when compiling
    /// that is probably not the user's fault.
    /// </summary>
    public class ConfigCompilerException : Exception
    {
        public ConfigCompilerException( string message ) :
            base( message )
        {
        }
    }
    
    /// <summary>
    /// Exception that is thrown when a user has an invalid configuration.
    /// </summary>
    public class InvalidConfigurationException : ListedValidationException
    {
        public InvalidConfigurationException( string context, IEnumerable<string> errors ) :
            base( context, errors )
        {
        }
    }
}