﻿//
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

using System.Xml.Linq;
using Cake.Common.Solution;
using Cake.Common.Tools.DotNet.MSBuild;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;
using Seth.CakeLib;

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
            this.Solution = this.SrcDir.CombineWithFilePath( "Rau.sln" );
            this.ServiceProject = this.SrcDir.CombineWithFilePath( "Rau/Rau.csproj" );
            this.StandardProject = this.SrcDir.CombineWithFilePath( "Rau.Standard/Rau.Standard.csproj" );
            this.DistFolder = this.RepoRoot.Combine( "dist" );

            this.LooseFilesDistFolder = this.DistFolder.Combine( "files" );
            this.ZipFilesDistFolder= this.DistFolder.Combine( "zip" );

            this.TestResultsFolder = this.RepoRoot.Combine( "TestResults" );
            this.TestCsProj = this.SrcDir.CombineWithFilePath( "Rau.Tests/Rau.Tests.csproj" );

            var pluginProjects = new List<SolutionProject>();
            this.PluginProjects = pluginProjects.AsReadOnly();
            context.PerformActionOnSolutionCsProjectFiles(
                this.Solution,
                ( SolutionProject project ) =>
                {
                    lock( pluginProjects )
                    {
                        pluginProjects.Add( project );
                    }
                },
                ( SolutionProject project ) =>
                {
                    return project.Name.StartsWith( "Rau.Plugins" );
                }
            );
        }

        // ---------------- Properties ----------------

        public DirectoryPath RepoRoot { get; }

        public DirectoryPath SrcDir { get; }

        public FilePath Solution { get; }

        public FilePath ServiceProject { get; }

        public FilePath StandardProject { get; }

        public IReadOnlyCollection<SolutionProject> PluginProjects { get; }

        public DirectoryPath DistFolder { get; }

        public DirectoryPath LooseFilesDistFolder { get; }

        public DirectoryPath ZipFilesDistFolder { get; }

        public DirectoryPath TestResultsFolder { get; }

        public FilePath TestCsProj { get; }

        // ---------------- Functions ----------------

        public DotNetMSBuildSettings GetBuildSettings()
        {
            var settings = new DotNetMSBuildSettings();

            settings.SetMaxCpuCount( System.Environment.ProcessorCount );

            return settings;
        }

        public Version GetRauVersion()
        {
            var doc = XDocument.Load( this.ServiceProject.FullPath );
            foreach( XElement element in doc.Descendants() )
            {
                string elementName = element.Name.LocalName;
                if( string.IsNullOrEmpty( elementName ) )
                {
                    continue;
                }
                else if( "PropertyGroup" == elementName )
                {
                    foreach( XElement propertyGroup in element.Descendants() )
                    {
                        string propertyName = propertyGroup.Name.LocalName;
                        if( string.IsNullOrEmpty( propertyName ) )
                        {
                            continue;
                        }
                        else if( "Version" == propertyName )
                        {
                            return Version.Parse( propertyGroup.Value );
                        }
                    }
                }
            }

            throw new CakeException(
                $"Can not find version defined in {this.ServiceProject.FullPath}"
            );
        }
    }
}
