//
// Rau - A bot that reads RSS feeds and posts them to a AT-Proto PDS node
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
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace Rau.Configuration
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

        public ApiBuilder Compile( FileInfo sourceFile, IEnumerable<string> namespaces )
        {
            const string expectedType = "Rau.Config.CompiledApiBuilder";
            
            Assembly asm = CompileAsm( sourceFile, namespaces );

            Type? type = asm.GetType( expectedType );
            if( type is null )
            {
                throw new ConfigCompilerException(
                    $"Type of {expectedType} not found in dynamic assembly."
                );
            }

            object? obj = Activator.CreateInstance( type );
            if( obj is null )
            {
                throw new ConfigCompilerException(
                    "Failed to activate compiled config object."
                );
            }

            return (ApiBuilder)obj;
        }

        private Assembly CompileAsm( FileInfo sourceFile, IEnumerable<string> namespaces )
        {
            string code = GetCode( sourceFile, namespaces );

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

            ms.Seek( 0, SeekOrigin.Begin );
            return Assembly.Load( ms.ToArray() );
        }

        private string GetCode( FileInfo sourceFile, IEnumerable<string> namespaces )
        {
            var namespaceBuilder = new StringBuilder();
            namespaceBuilder.AppendLine( "using System;" );
            namespaceBuilder.AppendLine( "using System.Collections.Generic;" );
            namespaceBuilder.AppendLine( "using System.IO;" );
            namespaceBuilder.AppendLine( "using Rau;" );
            namespaceBuilder.AppendLine( "using Rau.Standard;" );
            namespaceBuilder.AppendLine( "using Rau.Standard.Configuration;" );
            namespaceBuilder.AppendLine( "using Rau.Standard.EventScheduler;" );
            namespaceBuilder.AppendLine( "using Rau.Standard.Logging;" );
            foreach( string ns in namespaces )
            {
                namespaceBuilder.AppendLine( ns );
            }
            
            string code = File.ReadAllText( sourceFile.FullName );

            code =
$@"{namespaceBuilder}
namespace Rau.Config
{{
    public sealed class CompiledApiBuilder : ApiBuilder
    {{
        public CompiledApiBuilder() :
            base()
        {{
        }}

        {code}
    }}
}}
";
            return code;
        }
    }
}
