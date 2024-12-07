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

using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Pack;
using Cake.Core.IO;
using Cake.Frosting;

namespace DevOps.Pack
{
    [TaskName( "nuget_pack" )]
    public sealed class NugetPackTask : DevopsTask
    {
        // ---------------- Methods ----------------

        public override void Run( BuildContext context )
        {
            DirectoryPath outputDirectory = context.DistFolder.Combine( new DirectoryPath( "nuget" ) );

            context.CleanDirectory( outputDirectory );
            context.EnsureDirectoryExists( outputDirectory );

            var settings = new DotNetPackSettings
            {
                Configuration = "Release",
                OutputDirectory = outputDirectory.FullPath,
                IncludeSymbols = true,
                SymbolPackageFormat = "snupkg"
            };

            context.DotNetPack( context.StandardProject.ToString(), settings );
        }
    }
}
