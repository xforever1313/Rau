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

namespace Rau.Standard.Configuration
{
    /// <summary>
    /// Attribute is used when one wants to add using statements to the compiled
    /// configuration file.
    ///
    /// This is useful for plugins so one doesn't have to include the plugin's
    /// namespace to use any methods the plugin exposes.
    /// </summary>
    [AttributeUsage( AttributeTargets.Assembly, AllowMultiple = true )]
    public sealed class RauConfigurationNamespaceAttribute : Attribute
    {
        // ---------------- Constructor ----------------
        
        public RauConfigurationNamespaceAttribute( string namespaceName )
        {
            this.NamespaceName = namespaceName;
        }
        
        // ---------------- Properties ----------------
        
        public string NamespaceName { get; }
    }
}