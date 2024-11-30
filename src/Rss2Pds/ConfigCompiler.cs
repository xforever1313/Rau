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

using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using SethCS.Exceptions;

namespace Rss2Pds
{
    internal sealed class ConfigCompiler
    {
        // ---------------- Fields ----------------

        private readonly Version assemblyVersion;
        
        // ---------------- Constructor ----------------

        public ConfigCompiler( Version assemblyVersion )
        {
            this.assemblyVersion = assemblyVersion;
        }

        // ---------------- Methods ----------------

        public Rss2PdsConfig Compile( FileInfo sourceFile )
        {
            Assembly asm = CompileAsm( sourceFile );

            Type? type = asm.GetType( "Rss2Pds.Config.CompiledConfig" );
            if( type is null )
            {
                throw new ConfigCompilerException(
                    "Type of Rss2Pds.Config.CompiledConfig not found in dynamic assembly."
                );
            }
            
            object? obj = Activator.CreateInstance( type );
            if( obj is null )
            {
                throw new ConfigCompilerException(
                    "Failed to activate compiled config object."
                );
            }
            
            object? configObject = type.InvokeMember(
                "Build",
                BindingFlags.Default | BindingFlags.InvokeMethod,
                null,
                obj,
                null
            );

            if( configObject is null )
            {
                throw new ConfigCompilerException(
                    "Failed to invoke Build method."
                );
            }
            
            return (Rss2PdsConfig)configObject;
        }

        private Assembly CompileAsm( FileInfo sourceFile )
        {
            string code = GetCode( sourceFile );

            // Taken from https://stackoverflow.com/a/47732064
            Assembly[] refs = AppDomain.CurrentDomain.GetAssemblies();
            List<MetadataReference> references = refs.Where( a => a.IsDynamic == false )
                .Select( a => MetadataReference.CreateFromFile( a.Location ) )
                .ToList<MetadataReference>();

            references.Add( MetadataReference.CreateFromFile( typeof( Uri ).Assembly.Location ) );
            
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText( code );
            
            var options = new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary
            )
            {
            };
            
            CSharpCompilation compilation = CSharpCompilation.Create(
                "ConfigurationAssembly",
                syntaxTrees: [syntaxTree],
                references: references,
                options: options
            );

            using var ms = new MemoryStream();
            EmitResult result = compilation.Emit( ms );

            if( result.Success == false )
            {
                var errors = new List<string>();
                foreach( Diagnostic diagnostic in result.Diagnostics )
                {
                    errors.Add( $"{diagnostic.Id}: {diagnostic.GetMessage()}" );
                }

                throw new InvalidConfigurationException(
                    "Errors compiling config file.  Please fix these errors and try again.",
                    errors
                );
            }

            ms.Seek(0, SeekOrigin.Begin );
            return Assembly.Load( ms.ToArray() );
        }

        private string GetCode( FileInfo sourceFile )
        {
            string code = File.ReadAllText( sourceFile.FullName );

            code =
$@"
using System;
using System.Collections.Generic;
using Rss2Pds;

namespace Rss2Pds.Config
{{
    public record class CompiledConfig : Rss2PdsConfig
    {{
        public CompiledConfig() :
            base( new Version( {this.assemblyVersion.Major}, {this.assemblyVersion.Major}, {this.assemblyVersion.Build} ) )
        {{
        }}

        public Rss2PdsConfig Build()
        {{
            {code}
            return this;
        }}
    }}
}}
";
            return code;
        }
    }
}
