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
                    Console.WriteLine( nameof( Rau ) + " - A bot framework to post to an AT-Proto PDS" );
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

                FileInfo? configFile = options.GetConfigFilePath();
                if( configFile is null )
                {
                    configFile = new FileInfo(
                        Path.Combine(
                            Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData ),
                            "Rau",
                            "Config.cs"
                        )
                    );

                    if( configFile.Exists == false )
                    {
                        DirectoryInfo? defaultDirectory = configFile.Directory;
                        if( ( defaultDirectory is not null ) && defaultDirectory.Exists == false )
                        {
                            Directory.CreateDirectory( defaultDirectory.FullName );
                        }
                    }

                    if( configFile.Exists == false )
                    {
                        WriteDefaultConfig( configFile );
                        Console.WriteLine( "No configuration file has been specifed, and a default one does not exist." );
                        Console.WriteLine( $"A default configuration file has been created at: {configFile.FullName}" );
                        Console.WriteLine( "Please fill out the configuration file, and re-run this program." );
                        Console.WriteLine();
                        Console.WriteLine( "See https://github.com/xforever1313/Rau/blob/main/Sample.Config.cs for an example on how to configure this bot." );
                        return 2;
                    }
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

                if( config.PersistenceLocation.Exists == false )
                {
                    Directory.CreateDirectory( config.PersistenceLocation.FullName );
                }
                log.Information( $"Using persistence located in: {config.PersistenceLocation.FullName}." );

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

                api.Init();
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
                var initArgs = new RauPluginInitializationArgs
                {
                    PersistenceLocation = new DirectoryInfo(
                        Path.Combine( api.Config.PersistenceLocation.FullName, plugin.PluginName )
                    )
                };

                log.Debug( $"Loading plugin {plugin.PluginName}..." );
                Stopwatch watch = Stopwatch.StartNew();
                plugin.Initialize( api, initArgs );
                log.Debug( $"Loaded plugin {plugin.PluginName}, took {watch.Elapsed.TotalSeconds} Seconds." );
            }
        }

        private static void WriteDefaultConfig( FileInfo fileLocation )
        {
            string fileContents =
$@"
/// <summary>
/// Configures the global settings for Rau.
/// This includes settings that are required to launch the service.
/// </summary>
public override void {nameof( ApiBuilder.ConfigureRauSettings )}( {nameof( IRauConfigurator )} rau )
{{
    // 300 is Blue Sky's character limit.
    rau.SetCharacterLimit( 300 );

    // Uncomment and change the directory to enable logging to
    // a file.
    // rau.LogToFile( ""/home/rau/rau.log"" );

    // Comment out to not expose a Prometheus port.
    rau.UseMetricsAtPort( 9100 );

    // Uncomment to override the default user agent information that is used
    // when sending HTTP requests to a PDS.
    // rau.OverridePdsUserAgent( ""my_bot"", new Version( 1, 2, 3 ) );
}}

/// <summary>
/// Configures the bot itself.  This method is run
/// after all plugins are loaded and initialized.
/// </summary>
public override void {nameof( ApiBuilder.ConfigureBot )}( {nameof( IRauApi )} rau )
{{
    throw new System.NotImplementedException( ""User must configure the bot before running it"" );
}}
";

            File.WriteAllText( fileLocation.FullName, fileContents );
        }
    }
}