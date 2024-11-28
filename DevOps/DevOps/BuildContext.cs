//
// Rss2Pds - A bot that reads RSS feeds and posts them to a AT-Proto PDS node
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

using Cake.Common.Tools.DotNet.MSBuild;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;

namespace DevOps
{
    public class BuildContext : FrostingContext
    {
        // ---------------- Constructor ----------------

        public BuildContext( ICakeContext context ) :
            base( context )
        {
            this.RepoRoot = context.Environment.WorkingDirectory;
            this.SrcDir = this.RepoRoot.Combine( "src" );
            this.Solution = this.SrcDir.CombineWithFilePath( "Rss2Pds.sln" );
            this.DistFolder = this.RepoRoot.Combine( "dist" );

            this.BlueSkyDistFolder = this.DistFolder.Combine( "bluesky" );
            this.BlueSkyLooseFilesDistFolder = this.BlueSkyDistFolder.Combine( "files" );
            this.BlueSkyZipFilesDistFolder= this.BlueSkyDistFolder.Combine( "zip" );

            this.TestResultsFolder = this.RepoRoot.Combine( "TestResults" );
            this.TestCsProj = this.SrcDir.CombineWithFilePath( "Rss2Pds.Tests/Rss2Pds.Tests.csproj" );
        }

        // ---------------- Properties ----------------

        public DirectoryPath RepoRoot { get; }

        public DirectoryPath SrcDir { get; }

        public FilePath Solution { get; }

        public DirectoryPath DistFolder { get; }

        public DirectoryPath BlueSkyDistFolder { get; }

        public DirectoryPath BlueSkyLooseFilesDistFolder { get; }

        public DirectoryPath BlueSkyZipFilesDistFolder { get; }

        public DirectoryPath TestResultsFolder { get; }

        public FilePath TestCsProj { get; }

        // ---------------- Functions ----------------

        public DotNetMSBuildSettings GetBuildSettings()
        {
            var settings = new DotNetMSBuildSettings();

            settings.SetMaxCpuCount( System.Environment.ProcessorCount );

            return settings;
        }
    }
}
