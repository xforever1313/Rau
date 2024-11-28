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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Prometheus;
using Serilog;
using SethCS.Extensions;

namespace Rss2Pds
{
    public class Program
    {
        public static int Main( string[] args )
        {
            Version? version = typeof( Program ).Assembly.GetName()?.Version;
            Console.WriteLine( $"Version: {version?.ToString( 3 ) ?? string.Empty}." );

            var config = new Rss2PdsConfig( version );
            {
                List<string> errors = config.TryValidate();
                if( errors.Any() )
                {
                    Console.WriteLine( "Bot is misconfigured." );
                    Console.WriteLine( errors.ToListString( " - " ) );
                    return 1;
                }
            }

            Serilog.ILogger? log = null;

            void OnTelegramFailure( Exception e )
            {
                log?.Warning( $"Telegram message did not send:{Environment.NewLine}{e}" );
            }

            using var httpClient = new BskyHttpClientFactory( config );
            try
            {
                log = HostingExtensions.CreateLog( config, OnTelegramFailure );

                WebApplicationBuilder builder = WebApplication.CreateBuilder( args );

                builder.Logging.ClearProviders();
                builder.Services.AddControllersWithViews();
                builder.Host.UseSerilog( log );
                builder.Services.AddSingleton<IHttpClientFactory>( httpClient );
                builder.Services.ConfigurePdsServices( config );

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
                return 2;
            }
            finally
            {
                log?.Information( "Application Exiting" );
            }

            return 0;

        }
    }
}