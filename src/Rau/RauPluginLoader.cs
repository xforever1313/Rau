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
using System.Reflection;
using Rau.Standard;
using Rau.Standard.Configuration;
using SethCS.Exceptions;

namespace Rau
{
    internal sealed class RauPluginLoader
    {
        // ---------------- Fields ----------------

        private readonly HashSet<string> configurationNamespaces;

        private readonly Dictionary<Guid, IRauPlugin> loadedPlugins;
        
        // ---------------- Constructor ----------------

        public RauPluginLoader()
        {
            this.configurationNamespaces = new HashSet<string>();
            this.ConfigurationNamespaces = this.configurationNamespaces;
            
            this.loadedPlugins = new Dictionary<Guid, IRauPlugin>();
            this.LoadedPlugins = new ReadOnlyDictionary<Guid, IRauPlugin>( this.loadedPlugins );
        }
        
        // ---------------- Properties ----------------
        
        /// <summary>
        /// All the collected <see cref="RauConfigurationNamespaceAttribute"/>
        /// found in each of the plugins.
        /// </summary>
        public IReadOnlyCollection<string> ConfigurationNamespaces { get; }

        /// <summary>
        /// List of loaded plugins so far.
        /// </summary>
        public IReadOnlyDictionary<Guid, IRauPlugin> LoadedPlugins { get; }
        
        // ---------------- Methods ----------------

        public void AddPlugin( FileInfo assemblyPath )
        {
            AddPlugin( Assembly.LoadFrom( assemblyPath.FullName ) );
        }
        
        public void AddPlugin( Assembly assembly )
        {
            var errors = new List<string>();
            
            foreach( Type type in assembly.GetTypes() )
            {
                RauPluginAttribute? pluginAttr = type.GetCustomAttribute<RauPluginAttribute>();
                if( pluginAttr is null )
                {
                    continue;
                }
                else if( Guid.TryParse( pluginAttr.PluginId, out Guid pluginGuid ) == false )
                {
                    errors.Add( $"Plugin ID associated with {type.FullName} is not a valid GUID.  Got: {pluginGuid}." );
                    continue;
                }
                else if( ValidatePlugin( pluginGuid, type, out string errorMessage ) == false )
                {
                    errors.Add( errorMessage );
                    continue;
                }
                else
                {
                    object? pluginObj = Activator.CreateInstance( type );
                    IRauPlugin? plugin = pluginObj as IRauPlugin;
                    if( plugin is null )
                    {
                        errors.Add( $"Unable to create instance of {type.FullName}." );
                        continue;
                    }

                    this.loadedPlugins.Add( pluginGuid, plugin );
                }
            }
            
            LoadNamespaces( assembly );
            
            if( errors.Any() )
            {
                throw new ListedValidationException(
                    "Error when loading assembly " + assembly.FullName,
                    errors
                );
            }
        }
        
        private bool ValidatePlugin( Guid pluginId, Type type, out string errorMessage )
        {
            if( type.IsAbstract )
            {
                errorMessage = $"Type {type.FullName} is abstract, can not use as a plugin implementation.";
                return false;
            }
            else if( type.IsAssignableTo( typeof( IRauPlugin ) ) == false )
            {
                errorMessage = $"Type {type.FullName} can not be assigned to the {nameof( IRauPlugin )} interface.  Please ensure it implements this interface.";
                return false;
            }
            else if( this.loadedPlugins.ContainsKey( pluginId ) )
            {
                errorMessage = $"Plugin ID '{pluginId}' already exists.  Its possible another plugin loaded a feed type and used the same ID.";
                return false;
            }

            errorMessage = "";
            return true;
        }

        private void LoadNamespaces( Assembly plugin )
        {
            IEnumerable<RauConfigurationNamespaceAttribute> namespaces =
                plugin.GetCustomAttributes<RauConfigurationNamespaceAttribute>();
            
            foreach( RauConfigurationNamespaceAttribute ns in namespaces )
            {
                if( this.configurationNamespaces.Contains( ns.NamespaceName ) == false )
                {
                    this.configurationNamespaces.Add( ns.NamespaceName );
                }
            }
        }
    }
}