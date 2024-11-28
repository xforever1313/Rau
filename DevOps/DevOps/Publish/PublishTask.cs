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

using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Publish;
using Cake.Core.IO;
using Cake.Frosting;

namespace DevOps.Publish
{
    [TaskName( "publish" )]
    [IsDependentOn( typeof( PublishBlueSkyTask ) )]
    public sealed class PublishAllTask : DevopsTask
    {
    }

    [TaskName( "publish_bluesky" )]
    public sealed class PublishBlueSkyTask : DevopsTask
    {
        // ---------------- Functions ----------------

        public override void Run( BuildContext context )
        {
            context.EnsureDirectoryExists( context.BlueSkyDistFolder );

            DirectoryPath looseFilesDir = context.BlueSkyLooseFilesDistFolder;
            context.EnsureDirectoryExists( looseFilesDir );
            context.CleanDirectory( looseFilesDir );

            context.Information( "Publishing App" );

            var publishOptions = new DotNetPublishSettings
            {
                Configuration = "Release",
                OutputDirectory = looseFilesDir.ToString(),
                MSBuildSettings = context.GetBuildSettings()
            };

            FilePath servicePath = context.SrcDir.CombineWithFilePath(
                "Rss2Pds.Bsky/Rss2Pds.Bsky.csproj"
            );

            context.DotNetPublish( servicePath.ToString(), publishOptions );
            context.Information( string.Empty );

            CopyRootFile( context, "Readme.md" );
            CopyRootFile( context, "Credits.md" );
            CopyRootFile( context, "License.md" );

            context.EnsureDirectoryExists( context.BlueSkyZipFilesDistFolder );
            context.CleanDirectory( context.BlueSkyZipFilesDistFolder );

            FilePath zipFile = context.BlueSkyZipFilesDistFolder.CombineWithFilePath( "Rss2Pds_BlueSky.zip" );
            context.Zip( context.BlueSkyLooseFilesDistFolder, zipFile );
        }

        private void CopyRootFile( BuildContext context, FilePath fileName )
        {
            fileName = context.RepoRoot.CombineWithFilePath( fileName );
            context.Information( $"Copying '{fileName}' to dist" );
            context.CopyFileToDirectory( fileName, context.BlueSkyLooseFilesDistFolder );
        }
    }
}
