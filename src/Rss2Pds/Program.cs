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

using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Mono.Options;
using Rau.Standard;
using Rau.Standard.Configuration;
using Rau.Configuration;
using Serilog;
using SethCS.Extensions;
using SethCS.IO;

namespace Rau
{
    internal static class Program
    {
        private static Version? version = null;

        private static Serilog.ILogger? log = null;

        public static int Main( string[] args )
        {
            try
            {
                version = typeof( Program ).Assembly.GetName()?.Version;

                var options = new ArgumentParser( args );
                if( options.ShowHelp )
                {
                    Console.WriteLine( nameof( Rau ) + " - Posts RSS feeds to an AT-Proto PDS" );
                    options.PrintHelp( Console.Out );
                    return 0;
                }
                else if( options.ShowVersion )
                {
                    PrintVersion();
                    return 0;
                }
                else if( options.ShowLicense )
                {
                    PrintLicense();
                    return 0;
                }
                else if( options.ShowReadme )
                {
                    PrintReadme();
                    return 0;
                }
                else if( options.ShowCredits )
                {
                    PrintCredits();
                    return 0;
                }

                PrintVersion();

                FileInfo? configFile = options.ConfigFilePath;
                if( configFile is null )
                {
                    Console.WriteLine( "Config file is somehow null" );
                    return 2;
                }
                
                Console.WriteLine( $"Using config file: {configFile.FullName}" );
                if( configFile.Exists == false )
                {
                    Console.WriteLine( "Config file does not exist." );
                    return 4;
                }

                var configCompiler = new ConfigCompiler( configFile );
                IEnumerable<FileInfo>? pluginFiles = configCompiler.Preprocess();
                
                // First, need to load all the plugins from the assemblies
                // since we need the assemblies loaded before trying to compile the config file.
                RauPluginLoader? pluginLoader = LoadPlugins( pluginFiles );

                ApiBuilder? apiBuilder = GetBuilder( configCompiler, pluginLoader.ConfigurationNamespaces );
                RauConfig config = GetConfig( apiBuilder );
                {
                    List<string> errors = config.TryValidate();
                    if( errors.Any() )
                    {
                        Console.WriteLine( "Bot is misconfigured." );
                        Console.WriteLine( errors.ToListString( " - " ) );
                        return 1;
                    }
                }

                if( options.CheckConfigOnly )
                {
                    Console.WriteLine( "Bot configuration is valid!" );   
                    return 0;
                }

                log = HostingExtensions.CreateLog( config, OnTelegramFailure );
                
                using var httpClient = new BskyHttpClientFactory( config );
                
                // Plugins require an API to be created before initializing them.
                using var api = new RauApi( config, pluginLoader.LoadedPlugins, httpClient, log );
                InitPlugins( log, api );
                
                // Configure the bot only after the plugins were initialized.
                // Plugins may need to be initialized before they are configured.
                apiBuilder.ConfigureBot( api );
                
                WebApplicationBuilder builder = WebApplication.CreateBuilder( args );

                builder.Logging.ClearProviders();
                builder.Services.AddControllersWithViews();
                builder.Host.UseSerilog( log );
                builder.Services.AddSingleton<IHttpClientFactory>( httpClient );
                WebApplication app = builder.ConfigurePdsServices( api );

                log.Information( "Application Running..." );

                // These are no longer needed, set to null so they get collected.
                configCompiler = null;
                pluginFiles = null;
                apiBuilder = null;
                options = null;
                pluginLoader = null;
                app.Run();
            }
            catch( OptionException e )
            {
                Console.Error.WriteLine( "Error parsing options:" );
                Console.Error.WriteLine( e.ToString() );
                return 3;
            }
            catch( Exception e )
            {
                if( log is null )
                {
                    Console.Error.WriteLine( "FATAL ERROR:" );
                    Console.Error.WriteLine( e.ToString() );
                }
                else
                {
                    log.Fatal( "FATAL ERROR:" + Environment.NewLine + e );
                }
                return 100;
            }
            finally
            {
                log?.Information( "Application Exiting" );
            }

            return 0;

        }

        private static void PrintCredits()
        {
            string str = AssemblyResourceReader.ReadStringResource(
                typeof( Program ).Assembly, $"{nameof( Rau )}.Credits.md"
            );
            Console.WriteLine( str );
        }

        private static void PrintLicense()
        {
            string str = AssemblyResourceReader.ReadStringResource(
                typeof( Program ).Assembly, $"{nameof( Rau )}.License.md"
            );
            Console.WriteLine( str );
        }

        private static void PrintReadme()
        {
            string str = AssemblyResourceReader.ReadStringResource(
                typeof( Program ).Assembly, $"{nameof( Rau )}.Readme.md"
            );
            Console.WriteLine( str );
        }

        private static void PrintVersion()
        {
            Console.WriteLine( $"Version: {version?.ToString( 3 ) ?? string.Empty}." );
        }

        private static void OnTelegramFailure( Exception e )
        {
            log?.Warning( $"Telegram message did not send:{Environment.NewLine}{e}" );
        }

        private static ApiBuilder GetBuilder( ConfigCompiler compiler, IEnumerable<string> usingStatements )
        {
            ArgumentNullException.ThrowIfNull( version );

            Console.WriteLine( "Compiling API Builder..." );

            ApiBuilder builder = compiler.Compile( usingStatements );
            Console.WriteLine( "Compiling API Builder... Done!" );

            return builder;
        }

        private static RauConfig GetConfig( ApiBuilder builder )
        {
            var rauConfigurator = new RauConfigurator( version );
            builder.ConfigureRauSettings( rauConfigurator );

            return rauConfigurator.Config;
        }

        private static RauPluginLoader LoadPlugins( IEnumerable<FileInfo> pluginPaths )
        {
            var pluginLoader = new RauPluginLoader();
            foreach( FileInfo fileInfo in pluginPaths )
            {
                pluginLoader.AddPlugin( fileInfo );
            }
            return pluginLoader;
        }

        private static void InitPlugins( Serilog.ILogger log, RauApi api )
        {
            foreach( IRauPlugin plugin in api.Plugins.Values )
            {
                log.Debug( $"Loading plugin {plugin.PluginName}..." );
                Stopwatch watch = Stopwatch.StartNew();
                plugin.Initialize( api );
                log.Debug( $"Loaded plugin {plugin.PluginName}, took {watch.Elapsed.TotalSeconds} Seconds." );
            }
        }
    }
}