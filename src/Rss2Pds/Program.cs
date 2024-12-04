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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Mono.Options;
using Prometheus;
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
                RauPluginLoader pluginLoader = LoadPlugins( configFile );

                ApiBuilder apiBuilder = GetBuilder( configFile, pluginLoader.ConfigurationNamespaces );
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

                using var api = new RauApi( config, httpClient, log );
                apiBuilder.ConfigureBot( api );
                
                WebApplicationBuilder builder = WebApplication.CreateBuilder( args );

                builder.Logging.ClearProviders();
                builder.Services.AddControllersWithViews();
                builder.Host.UseSerilog( log );
                builder.Services.AddSingleton<IHttpClientFactory>( httpClient );
                builder.Services.ConfigurePdsServices( api );

                WebApplication app = builder.Build();
                if( config.MetricsPort is not null )
                {
                    builder.WebHost.UseUrls( $"http://0.0.0.0:{config.MetricsPort}" );

                    app.UseRouting();

                    // Per https://learn.microsoft.com/en-us/aspnet/core/diagnostics/asp0014?view=aspnetcore-8.0:
                    // Warnings from this rule can be suppressed if
                    // the target UseEndpoints invocation is invoked without
                    // any mappings as a strategy to organize middleware ordering.
                    #pragma warning disable ASP0014 // Suggest using top level route registrations
                    app.UseEndpoints(
                        endpoints =>
                        {
                            endpoints.MapMetrics( "/Metrics" );
                        }
                    );
                    #pragma warning restore ASP0014 // Suggest using top level route registrations
                }

                log.Information( "Application Running..." );

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

        private static ApiBuilder GetBuilder( FileInfo fileInfo, IEnumerable<string> usingStatements )
        {
            ArgumentNullException.ThrowIfNull( version );

            Console.WriteLine( "Compiling API Builder..." );

            var compiler = new ConfigCompiler( version );
            ApiBuilder builder = compiler.Compile( fileInfo, usingStatements );
            Console.WriteLine( "Compiling API Builder... Done!" );

            return builder;
        }

        private static RauConfig GetConfig( ApiBuilder builder )
        {
            var rauConfigurator = new RauConfigurator( version );
            builder.ConfigureRauSettings( rauConfigurator );

            return rauConfigurator.Config;
        }

        private static RauPluginLoader LoadPlugins( FileInfo configFile )
        {
            var pluginLoader = new RauPluginLoader();
            // TODO: Iterate through plugins
            return pluginLoader;
        }
    }
}