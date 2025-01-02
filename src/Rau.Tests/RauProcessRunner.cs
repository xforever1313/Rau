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

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Rau.Tests
{
    public class RauProcessRunner : IDisposable
    {
        // ---------------- Events ----------------

        public event Action<string>? StandardOutReceived;
        public event Action<string>? StandardErrorReceived;

        // ---------------- Fields ----------------

        private static readonly TimeSpan defaultTimeout = new TimeSpan( 0, 0, 10 );

        private readonly ManualResetEventSlim runningEvent;
        private readonly Process rauProcess;

        private readonly string configFileContents;

        private readonly DirectoryInfo testDirectory;
        private readonly FileInfo configFileLocation;

        private bool isStarted;
        private bool isStopped;
        private bool isDisposed;

        // ---------------- Constructor ----------------

        public RauProcessRunner( DirectoryInfo testDirectory, string configFileContents)
        {
            string? assemblyLocation = Path.GetDirectoryName( GetType().Assembly.Location );
            Assert.IsNotNull( assemblyLocation );
            string csproj = Path.Combine(
                assemblyLocation, // runtime
                "..",             // Debug
                "..",             // bin
                "..",             // Rau.Tests
                "..",             // src
                "Rau",
                "Rau.csproj"
            );

            this.configFileLocation = new FileInfo( Path.Combine( testDirectory.FullName, "Rau.Config.cs" ) );

            var processStartInfo = new ProcessStartInfo(
                "dotnet",
                $@"run --no-build --no-restore --project=""{csproj}"" -- --use_enter_to_exit --config_file=""{this.configFileLocation.FullName}"""
            )
            {
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false,
                ErrorDialog = false
            };

            this.runningEvent = new ManualResetEventSlim( false );
            this.rauProcess = new Process
            {
                StartInfo = processStartInfo
            };
            this.rauProcess.OutputDataReceived += RauProcess_OutputDataReceived;
            this.rauProcess.ErrorDataReceived += RauProcess_ErrorDataReceived;

            this.configFileContents = configFileContents;
            this.testDirectory = testDirectory;

            this.configFileContents = configFileContents;

            this.isStarted = false;
            this.isStopped = false;
            this.isDisposed = false;
        }

        // ---------------- Properties ----------------

        public int? ExitCode { get; private set; }

        // ---------------- Methods ----------------

        public static DirectoryInfo GetTestDirectory( string fixtureName )
        {
            string assemblyPath = Path.GetDirectoryName( typeof( RauProcessRunner ).Assembly.Location ) ??
                throw new InvalidOperationException( "Unable to get folder location of the assembly" );

            string testDirectory = Path.Combine( assemblyPath, fixtureName );
            return new DirectoryInfo( testDirectory );
        }

        public void Start()
        {
            Assert.IsFalse( this.isStarted, "Process has already ran and exited.  Please make a new one." );
            if( this.testDirectory.Exists )
            {
                Directory.Delete( this.testDirectory.FullName, true );
            }

            Directory.CreateDirectory( this.testDirectory.FullName );
            File.WriteAllText( this.configFileLocation.FullName, this.configFileContents );

            this.rauProcess.Start();
            this.rauProcess.BeginOutputReadLine();
            this.rauProcess.BeginErrorReadLine();
            this.isStarted = true;
            Assert.IsTrue( this.runningEvent.Wait( defaultTimeout ), "Did not get CTRL+C message within timeout" );
        }

        public void StopProcess( TimeSpan? timeout = null )
        {
            if( this.isStarted == false )
            {
                return;
            }
            else if( this.isStopped )
            {
                return;
            }

            this.rauProcess.StandardInput.WriteLine();
            this.rauProcess.StandardInput.Flush();
            bool terminated = this.rauProcess.WaitForExit( timeout ?? defaultTimeout );
            try
            {
                if( terminated == false )
                {
                    this.rauProcess.Kill();
                    Assert.IsTrue(
                        this.rauProcess.WaitForExit( timeout ?? defaultTimeout ),
                        "Process did not die."
                    );
                }

                Assert.IsTrue( terminated, "Process had to be killed." );
            }
            finally
            {
                this.ExitCode = this.rauProcess.ExitCode;
                this.isStopped = true;
            }
        }

        public void Dispose()
        {
            if( this.isDisposed )
            {
                return;
            }

            try
            {
                StopProcess();
                if( this.testDirectory.Exists )
                {
                    Directory.Delete( this.testDirectory.FullName, true );
                }
            }
            finally
            {
                this.rauProcess.OutputDataReceived -= RauProcess_OutputDataReceived;
                this.rauProcess.ErrorDataReceived -= RauProcess_ErrorDataReceived;
                this.rauProcess.Dispose();
                this.runningEvent.Dispose();
                this.isDisposed = true;
            }
        }

        private void RauProcess_ErrorDataReceived( object sender, DataReceivedEventArgs e )
        {
            if( string.IsNullOrEmpty( e.Data ) == false )
            {
                Console.Error.WriteLine( e.Data );
                this.StandardErrorReceived?.Invoke( e.Data );
            }
        }

        private void RauProcess_OutputDataReceived( object sender, DataReceivedEventArgs e )
        {
            if( string.IsNullOrEmpty( e.Data ) == false )
            {
                Console.WriteLine( e.Data );
                this.StandardOutReceived?.Invoke( e.Data );
                if( e.Data.Contains( Program.RunningMessage ) )
                {
                    this.runningEvent.Set();
                }
            }
        }
    }
}
