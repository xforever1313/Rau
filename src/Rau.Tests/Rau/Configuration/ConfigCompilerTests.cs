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

using Rau.Plugins.Canary;

namespace Rau.Tests.Rau.Configuration
{
    [TestClass]
    public sealed class ConfigCompilerTests
    {
        // ---------------- Fields ----------------

        private static FileInfo? canaryPluginLocation;
        
        // ---------------- Setup / Teardown ----------------

        [ClassInitialize]
        public static void ClassInitialize( TestContext testContext )
        {
            canaryPluginLocation = new FileInfo( typeof( CanaryPlugin ).Assembly.Location );
        }
        
        // ---------------- Properties ----------------

        public static FileInfo CanaryPluginLocation
        {
            get
            {
                Assert.IsNotNull( canaryPluginLocation );
                return canaryPluginLocation;
            }
        }
        
        // ---------------- Tests ----------------

        [TestMethod]
        public void PreProcessTest()
        {
            // Setup
            string configFileMinusPlugins =
$@"
public override void ConfigureRauSettings( IRauConfigurator configurator )
{{
}}

public override void ConfigureBot( IRauApi rau )
{{
    rau.AddCanaryAccountWithDefaultMessage(
        new PdsAccount
        {{
            UserName =  ""canary.at.shendrick.net"",
            Password = """",
            Instance = new Uri( ""https://at.shendrick.net"" )
        }},
        ""0 13 * * * ?""
    );
}}
";
            string configFileWithPlugins =
$@"#plugin {CanaryPluginLocation.FullName}
{configFileMinusPlugins}";
            
            // Act
            var uut = new ConfigCompiler( configFileWithPlugins );
            IReadOnlyCollection<FileInfo> pluginPaths = uut.Preprocess();
            
            // Check
            Assert.AreEqual( configFileMinusPlugins, uut.ConfigFileSourceCode );
            Assert.AreEqual( 1, pluginPaths.Count() );
            Assert.AreEqual( CanaryPluginLocation.FullName, pluginPaths.First().FullName );
        }
    }
}