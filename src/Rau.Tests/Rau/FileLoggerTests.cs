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

using System.Runtime.CompilerServices;
using Rau.Standard.Configuration;
using Rau.Standard.Logging;

namespace Rau.Tests.Rau
{
    /// <summary>
    /// Tests to make sure the file logger works as configured.
    /// </summary>
    [TestClass]
    // Since we write to a file system, do not parallelize so nothing
    // weird happens.
    [DoNotParallelize]
    public sealed class FileLoggerTests
    {
        // ---------------- Fields ----------------

        private static DirectoryInfo? testDirectory;

        // ---------------- Setup / Teardown ----------------

        [ClassInitialize]
        public static void FixtureSetup( TestContext testContext )
        {
            testDirectory = RauProcessRunner.GetTestDirectory( nameof( FileLoggerTests ) );
            if( testDirectory.Exists )
            {
                testDirectory.Delete( true );
            }

            testDirectory.Create();
        }

        [ClassCleanup]
        public static void FixtureTeardown()
        {
            if( testDirectory?.Exists ?? false )
            {
                testDirectory.Delete( true );
            }
        }

        // ---------------- Properties ----------------

        private static DirectoryInfo TestDirectory
        {
            get
            {
                Assert.IsNotNull( testDirectory );
                return testDirectory;
            }
        }

        // ---------------- Tests ----------------

        [TestMethod]
        public void VerboseFileLogTest()
        {
            // Setup
            const RauLogLevel logLevel = RauLogLevel.Verbose;

            var workingDirectory = new DirectoryInfo( Path.Combine( TestDirectory.FullName, $"{logLevel}Test" ) );
            var fileLocation = new FileInfo( Path.Combine( workingDirectory.FullName, $"log.log" ) );
            string configFileContents = GetConfigFileContents( logLevel, fileLocation );

            // Act
            try
            {
                using var runner = new RauProcessRunner( workingDirectory, configFileContents );
                runner.Start();
                runner.StopProcess();

                GC.Collect();

                // Check
                Assert.AreEqual( 0, runner.ExitCode );
                Assert.IsTrue( fileLocation.Exists );

                string logFileContents = File.ReadAllText( fileLocation.FullName );

                Console.WriteLine();
                Console.WriteLine( "Log file contents: " );
                Console.WriteLine( logFileContents );

                Assert.IsTrue( logFileContents.Contains( GetDebugMessage( RauLogLevel.Verbose ) ) );
                Assert.IsTrue( logFileContents.Contains( GetDebugMessage( RauLogLevel.Debug ) ) );
                Assert.IsTrue( logFileContents.Contains( GetDebugMessage( RauLogLevel.Information ) ) );
                Assert.IsTrue( logFileContents.Contains( GetDebugMessage( RauLogLevel.Error ) ) );
                Assert.IsTrue( logFileContents.Contains( GetDebugMessage( RauLogLevel.Fatal ) ) );
            }
            finally
            {
                fileLocation.Delete();
            }
        }

        [TestMethod]
        public void DebugFileLogTest()
        {
            // Setup
            const RauLogLevel logLevel = RauLogLevel.Debug;

            var workingDirectory = new DirectoryInfo( Path.Combine( TestDirectory.FullName, $"{logLevel}Test" ) );
            var fileLocation = new FileInfo( Path.Combine( workingDirectory.FullName, $"log.log" ) );
            string configFileContents = GetConfigFileContents( logLevel, fileLocation );

            // Act
            using var runner = new RauProcessRunner( workingDirectory, configFileContents );
            runner.Start();
            runner.StopProcess();

            // Check
            Assert.AreEqual( 0, runner.ExitCode );
            Assert.IsTrue( fileLocation.Exists );

            string logFileContents = File.ReadAllText( fileLocation.FullName );

            Console.WriteLine();
            Console.WriteLine( "Log file contents: " );
            Console.WriteLine( logFileContents );

            Assert.IsFalse( logFileContents.Contains( GetDebugMessage( RauLogLevel.Verbose ) ) );
            Assert.IsTrue( logFileContents.Contains( GetDebugMessage( RauLogLevel.Debug ) ) );
            Assert.IsTrue( logFileContents.Contains( GetDebugMessage( RauLogLevel.Information ) ) );
            Assert.IsTrue( logFileContents.Contains( GetDebugMessage( RauLogLevel.Error ) ) );
            Assert.IsTrue( logFileContents.Contains( GetDebugMessage( RauLogLevel.Fatal ) ) );
        }

        [TestMethod]
        public void InformationFileLogTest()
        {
            // Setup
            const RauLogLevel logLevel = RauLogLevel.Information;

            var workingDirectory = new DirectoryInfo( Path.Combine( TestDirectory.FullName, $"{logLevel}Test" ) );
            var fileLocation = new FileInfo( Path.Combine( workingDirectory.FullName, $"log.log" ) );
            string configFileContents = GetConfigFileContents( logLevel, fileLocation );

            // Act
            using var runner = new RauProcessRunner( workingDirectory, configFileContents );
            runner.Start();
            runner.StopProcess();

            // Check
            Assert.AreEqual( 0, runner.ExitCode );
            Assert.IsTrue( fileLocation.Exists );

            string logFileContents = File.ReadAllText( fileLocation.FullName );

            Console.WriteLine();
            Console.WriteLine( "Log file contents: " );
            Console.WriteLine( logFileContents );

            Assert.IsFalse( logFileContents.Contains( GetDebugMessage( RauLogLevel.Verbose ) ) );
            Assert.IsFalse( logFileContents.Contains( GetDebugMessage( RauLogLevel.Debug ) ) );
            Assert.IsTrue( logFileContents.Contains( GetDebugMessage( RauLogLevel.Information ) ) );
            Assert.IsTrue( logFileContents.Contains( GetDebugMessage( RauLogLevel.Error ) ) );
            Assert.IsTrue( logFileContents.Contains( GetDebugMessage( RauLogLevel.Fatal ) ) );
        }

        [TestMethod]
        public void ErrorFileLogTest()
        {
            // Setup
            const RauLogLevel logLevel = RauLogLevel.Error;

            var workingDirectory = new DirectoryInfo( Path.Combine( TestDirectory.FullName, $"{logLevel}Test" ) );
            var fileLocation = new FileInfo( Path.Combine( workingDirectory.FullName, $"log.log" ) );
            string configFileContents = GetConfigFileContents( logLevel, fileLocation );

            // Act
            using var runner = new RauProcessRunner( workingDirectory, configFileContents );
            runner.Start();
            runner.StopProcess();

            // Check
            Assert.AreEqual( 0, runner.ExitCode );
            Assert.IsTrue( fileLocation.Exists );

            string logFileContents = File.ReadAllText( fileLocation.FullName );

            Console.WriteLine();
            Console.WriteLine( "Log file contents: " );
            Console.WriteLine( logFileContents );

            Assert.IsFalse( logFileContents.Contains( GetDebugMessage( RauLogLevel.Verbose ) ) );
            Assert.IsFalse( logFileContents.Contains( GetDebugMessage( RauLogLevel.Debug ) ) );
            Assert.IsFalse( logFileContents.Contains( GetDebugMessage( RauLogLevel.Information ) ) );
            Assert.IsTrue( logFileContents.Contains( GetDebugMessage( RauLogLevel.Error ) ) );
            Assert.IsTrue( logFileContents.Contains( GetDebugMessage( RauLogLevel.Fatal ) ) );
        }

        [TestMethod]
        public void FatalFileLogTest()
        {
            // Setup
            const RauLogLevel logLevel = RauLogLevel.Fatal;

            var workingDirectory = new DirectoryInfo( Path.Combine( TestDirectory.FullName, $"{logLevel}Test" ) );
            var fileLocation = new FileInfo( Path.Combine( workingDirectory.FullName, $"log.log" ) );
            string configFileContents = GetConfigFileContents( logLevel, fileLocation );

            // Act
            using var runner = new RauProcessRunner( workingDirectory, configFileContents );
            runner.Start();
            runner.StopProcess();

            // Check
            Assert.AreEqual( 0, runner.ExitCode );
            Assert.IsTrue( fileLocation.Exists );

            string logFileContents = File.ReadAllText( fileLocation.FullName );

            Console.WriteLine();
            Console.WriteLine( "Log file contents: " );
            Console.WriteLine( logFileContents );

            Assert.IsFalse( logFileContents.Contains( GetDebugMessage( RauLogLevel.Verbose ) ) );
            Assert.IsFalse( logFileContents.Contains( GetDebugMessage( RauLogLevel.Debug ) ) );
            Assert.IsFalse( logFileContents.Contains( GetDebugMessage( RauLogLevel.Information ) ) );
            Assert.IsFalse( logFileContents.Contains( GetDebugMessage( RauLogLevel.Error ) ) );
            Assert.IsTrue( logFileContents.Contains( GetDebugMessage( RauLogLevel.Fatal ) ) );
        }

        // ---------------- Test Helpers ----------------

        private static string GetConfigFileContents( RauLogLevel logLevel, FileInfo fileLocation )
        {
            DirectoryInfo? directory = fileLocation.Directory;
            Assert.IsNotNull( directory );
            string fileContents =
$@"
public override void ConfigureRauSettings( IRauConfigurator configurator )
{{
    configurator.UsePersistenceDirectory( @""{directory.FullName}"" );

    configurator.SetConsoleLogLevel( {nameof( RauLogLevel )}.{RauLogLevel.Verbose} );
    configurator.LogToFile( @""{fileLocation.FullName}"", {nameof( RauLogLevel )}.{logLevel} );
}}

public override void ConfigureBot( IRauApi rau )
{{
    rau.Logger.Verbose( ""{GetDebugMessage( RauLogLevel.Verbose )}"" );
    rau.Logger.Debug( ""{GetDebugMessage( RauLogLevel.Debug )}"" );
    rau.Logger.Information( ""{ GetDebugMessage( RauLogLevel.Information )}"" );
    rau.Logger.Warning( ""{ GetDebugMessage( RauLogLevel.Warning )}"" );
    rau.Logger.Error( ""{ GetDebugMessage( RauLogLevel.Error )}"" );
    rau.Logger.Fatal( ""{ GetDebugMessage( RauLogLevel.Fatal )}"" );
}}
";
            Console.WriteLine();
            Console.WriteLine( "Config file contents:" );
            Console.WriteLine( fileContents );
            Console.WriteLine();

            return fileContents;
        }

        private static string GetDebugMessage( RauLogLevel logLevel )
        {
            return $"RAU FILE LOGGER TEST: {nameof( RauLogLevel )}.{logLevel}";
        }
    }
}
