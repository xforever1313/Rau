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

using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace Rau.Configuration
{
    internal sealed class ConfigCompiler
    {
        // ---------------- Fields ----------------

        private readonly Regex pluginRegex = new Regex(
            @"^\s*#plugin\s+(?<plugin>[^\r\n]+)",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture
        );

        private readonly Func<string> readConfigFile;
        
        // ---------------- Constructor ----------------

        public ConfigCompiler( FileInfo configFile ) :
            this( () => File.ReadAllText( configFile.FullName ) )
        {
        }

        public ConfigCompiler( string code ) :
            this( () => code )
        {
        }

        private ConfigCompiler( Func<string> readConfigFile )
        {
            this.readConfigFile = readConfigFile;
        }

        // ---------------- Properties ----------------
        
        internal string? ConfigFileSourceCode { get; private set; }
        
        // ---------------- Methods ----------------

        /// <summary>
        /// Preprocesses the source file and determines where plugin files are.
        /// </summary>
        public IReadOnlyCollection<FileInfo> Preprocess()
        {
            var plugins = new List<FileInfo>();
            var fileContents = new StringBuilder();

            using var reader = new StringReader( this.readConfigFile() );
            string? line = reader.ReadLine();
            while( line is not null )
            {
                Match match = pluginRegex.Match( line );
                if( match.Success )
                {
                    plugins.Add( new FileInfo( match.Groups["plugin"].Value ) );
                }
                else
                {
                    fileContents.AppendLine( line );
                }

                line = reader.ReadLine();
            }
            
            this.ConfigFileSourceCode = fileContents.ToString();
            return plugins;
        }

        public ApiBuilder Compile( IEnumerable<string> namespaces )
        {
            if( this.ConfigFileSourceCode is null )
            {
                throw new InvalidOperationException(
                    "Can not compile config file, it has not been preprocessed yet."
                );
            }
            
            const string expectedType = "Rau.Config.CompiledApiBuilder";
            
            Assembly asm = CompileAsm( namespaces );

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

        private Assembly CompileAsm( IEnumerable<string> namespaces )
        {
            string code = GetCode( namespaces );

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

        private string GetCode( IEnumerable<string> namespaces )
        {
            if( this.ConfigFileSourceCode is null )
            {
                throw new InvalidOperationException(
                    "Can not compile config file, it has not been preprocessed yet."
                );
            }
            
            var namespaceBuilder = new StringBuilder();
            namespaceBuilder.AppendLine( "using System;" );
            namespaceBuilder.AppendLine( "using System.Collections.Generic;" );
            namespaceBuilder.AppendLine( "using System.IO;" );
            namespaceBuilder.AppendLine( "using Rau;" );
            namespaceBuilder.AppendLine( "using Rau.Configuration;" );
            namespaceBuilder.AppendLine( "using Rau.Standard;" );
            namespaceBuilder.AppendLine( "using Rau.Standard.Configuration;" );
            namespaceBuilder.AppendLine( "using Rau.Standard.EventScheduler;" );
            namespaceBuilder.AppendLine( "using Rau.Standard.Logging;" );
            foreach( string ns in namespaces )
            {
                namespaceBuilder.AppendLine( $"using {ns};" );
            }

            string code = this.ConfigFileSourceCode;

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
