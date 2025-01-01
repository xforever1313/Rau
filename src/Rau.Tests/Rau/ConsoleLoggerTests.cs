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
using System.Text;
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
    public sealed class ConsoleLoggerTests
    {
        // ---------------- Fields ----------------

        private static DirectoryInfo? testDirectory;

        // ---------------- Setup / Teardown ----------------

        [ClassInitialize]
        public static void FixtureSetup( TestContext testContext )
        {
            testDirectory = RauProcessRunner.GetTestDirectory( nameof( ConsoleLoggerTests ) );
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
            string configFileContents = GetConfigFileContents( logLevel, workingDirectory );

            // Act

            using var runner = new RauProcessRunner( workingDirectory, configFileContents );
            using var stdOut = new StdOutCapturer( runner );
            runner.Start();
            runner.StopProcess();

            // Check
            Assert.AreEqual( 0, runner.ExitCode );

            string consoleContents = stdOut.ToString();

            Assert.IsTrue( consoleContents.Contains( GetDebugMessage( RauLogLevel.Verbose ) ) );
            Assert.IsTrue( consoleContents.Contains( GetDebugMessage( RauLogLevel.Debug ) ) );
            Assert.IsTrue( consoleContents.Contains( GetDebugMessage( RauLogLevel.Information ) ) );
            Assert.IsTrue( consoleContents.Contains( GetDebugMessage( RauLogLevel.Error ) ) );
            Assert.IsTrue( consoleContents.Contains( GetDebugMessage( RauLogLevel.Fatal ) ) );
        }

        [TestMethod]
        public void DebugFileLogTest()
        {
            // Setup
            const RauLogLevel logLevel = RauLogLevel.Debug;

            var workingDirectory = new DirectoryInfo( Path.Combine( TestDirectory.FullName, $"{logLevel}Test" ) );
            string configFileContents = GetConfigFileContents( logLevel, workingDirectory );

            // Act
            using var runner = new RauProcessRunner( workingDirectory, configFileContents );
            using var stdOut = new StdOutCapturer( runner );
            runner.Start();
            runner.StopProcess();

            // Check
            Assert.AreEqual( 0, runner.ExitCode );

            string consoleContents = stdOut.ToString();

            Assert.IsFalse( consoleContents.Contains( GetDebugMessage( RauLogLevel.Verbose ) ) );
            Assert.IsTrue( consoleContents.Contains( GetDebugMessage( RauLogLevel.Debug ) ) );
            Assert.IsTrue( consoleContents.Contains( GetDebugMessage( RauLogLevel.Information ) ) );
            Assert.IsTrue( consoleContents.Contains( GetDebugMessage( RauLogLevel.Error ) ) );
            Assert.IsTrue( consoleContents.Contains( GetDebugMessage( RauLogLevel.Fatal ) ) );
        }

        [TestMethod]
        public void InformationFileLogTest()
        {
            // Setup
            const RauLogLevel logLevel = RauLogLevel.Information;

            var workingDirectory = new DirectoryInfo( Path.Combine( TestDirectory.FullName, $"{logLevel}Test" ) );
            string configFileContents = GetConfigFileContents( logLevel, workingDirectory );

            // Act
            using var runner = new RauProcessRunner( workingDirectory, configFileContents );
            using var stdOut = new StdOutCapturer( runner );
            runner.Start();
            runner.StopProcess();

            // Check
            Assert.AreEqual( 0, runner.ExitCode );

            string consoleContents = stdOut.ToString();

            Assert.IsFalse( consoleContents.Contains( GetDebugMessage( RauLogLevel.Verbose ) ) );
            Assert.IsFalse( consoleContents.Contains( GetDebugMessage( RauLogLevel.Debug ) ) );
            Assert.IsTrue( consoleContents.Contains( GetDebugMessage( RauLogLevel.Information ) ) );
            Assert.IsTrue( consoleContents.Contains( GetDebugMessage( RauLogLevel.Error ) ) );
            Assert.IsTrue( consoleContents.Contains( GetDebugMessage( RauLogLevel.Fatal ) ) );
        }

        [TestMethod]
        public void ErrorFileLogTest()
        {
            // Setup
            const RauLogLevel logLevel = RauLogLevel.Error;

            var workingDirectory = new DirectoryInfo( Path.Combine( TestDirectory.FullName, $"{logLevel}Test" ) );
            string configFileContents = GetConfigFileContents( logLevel, workingDirectory );

            // Act
            using var runner = new RauProcessRunner( workingDirectory, configFileContents );
            using var stdOut = new StdOutCapturer( runner );
            runner.Start();
            runner.StopProcess();

            // Check
            Assert.AreEqual( 0, runner.ExitCode );

            string consoleContents = stdOut.ToString();

            Assert.IsFalse( consoleContents.Contains( GetDebugMessage( RauLogLevel.Verbose ) ) );
            Assert.IsFalse( consoleContents.Contains( GetDebugMessage( RauLogLevel.Debug ) ) );
            Assert.IsFalse( consoleContents.Contains( GetDebugMessage( RauLogLevel.Information ) ) );
            Assert.IsTrue( consoleContents.Contains( GetDebugMessage( RauLogLevel.Error ) ) );
            Assert.IsTrue( consoleContents.Contains( GetDebugMessage( RauLogLevel.Fatal ) ) );
        }

        [TestMethod]
        public void FatalFileLogTest()
        {
            // Setup
            const RauLogLevel logLevel = RauLogLevel.Fatal;

            var workingDirectory = new DirectoryInfo( Path.Combine( TestDirectory.FullName, $"{logLevel}Test" ) );
            string configFileContents = GetConfigFileContents( logLevel, workingDirectory );

            // Act
            using var runner = new RauProcessRunner( workingDirectory, configFileContents );
            using var stdOut = new StdOutCapturer( runner );
            runner.Start();
            runner.StopProcess();

            // Check
            Assert.AreEqual( 0, runner.ExitCode );

            string consoleContents = stdOut.ToString();

            Assert.IsFalse( consoleContents.Contains( GetDebugMessage( RauLogLevel.Verbose ) ) );
            Assert.IsFalse( consoleContents.Contains( GetDebugMessage( RauLogLevel.Debug ) ) );
            Assert.IsFalse( consoleContents.Contains( GetDebugMessage( RauLogLevel.Information ) ) );
            Assert.IsFalse( consoleContents.Contains( GetDebugMessage( RauLogLevel.Error ) ) );
            Assert.IsTrue( consoleContents.Contains( GetDebugMessage( RauLogLevel.Fatal ) ) );
        }

        // ---------------- Test Helpers ----------------

        private static string GetConfigFileContents( RauLogLevel logLevel, DirectoryInfo testDirectory )
        {
            string fileContents =
$@"
public override void ConfigureRauSettings( IRauConfigurator configurator )
{{
    configurator.UsePersistenceDirectory( @""{testDirectory.FullName}"" );

    configurator.SetConsoleLogLevel( {nameof( RauLogLevel )}.{logLevel} );
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
            return $"RAU CONSOLE LOGGER TEST: {nameof( RauLogLevel )}.{logLevel}";
        }

        private class StdOutCapturer : IDisposable
        {
            // ---------------- Fields ----------------

            private readonly RauProcessRunner runner;

            private readonly StringBuilder stdOut;

            // ---------------- Constructor ----------------

            public StdOutCapturer( RauProcessRunner runner )
            {
                this.runner = runner;
                this.stdOut = new StringBuilder();

                this.runner.StandardOutReceived += Runner_StandardOutReceived;
            }

            // ---------------- Methods ----------------

            public override string ToString()
            {
                return this.stdOut.ToString();
            }

            public void Dispose()
            {
                this.runner.StandardOutReceived -= Runner_StandardOutReceived;
            }

            private void Runner_StandardOutReceived( string obj )
            {
                this.stdOut.AppendLine( obj );
            }
        }
    }
}
