using Rau.Plugins.Canary;

namespace Rau.Tests.Rau
{
    [TestClass]
    public sealed class RauPluginLoaderTests
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
        public void LoadCanaryPluginTest()
        {
            // Setup
            var uut = new RauPluginLoader();
            
            // Act
            uut.AddPlugin( CanaryPluginLocation );
            
            // Check
            Assert.AreEqual( 1, uut.LoadedPlugins.Count );
            Assert.AreEqual( 1, uut.ConfigurationNamespaces.Count );
            
            Assert.IsTrue( uut.LoadedPlugins.ContainsKey( CanaryPlugin.PluginGuid ) );
            Assert.IsTrue( uut.ConfigurationNamespaces.Contains( "Rau.Plugins.Canary" ) );
        }
    }
}
