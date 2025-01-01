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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Prometheus;
using Quartz;
using Rau.Logging;
using Rau.Standard;
using Rau.Standard.Configuration;
using Serilog;
using Serilog.Sinks.Telegram.Alternative;

namespace Rau
{
    internal static class HostingExtensions
    {
        public static Serilog.ILogger CreateLog(
            RauConfig config,
            Action<Exception> onTelegramFailure
        )
        {
            var logger = new LoggerConfiguration()
                // Use all levels, but each sink will
                // specify the level to use individually.
                .MinimumLevel.Verbose()
                .WriteTo.Console( config.ConsoleLogLevel.ToSerilogLevel() );

            bool useFileLogger = false;
            bool useTelegramLogger = false;

            FileInfo? logFile = config.LogFile;
            if( logFile is not null )
            {
                useFileLogger = true;
                logger.WriteTo.File(
                    logFile.FullName,
                    restrictedToMinimumLevel: config.LogFileLevel.ToSerilogLevel(),
                    retainedFileCountLimit: 10,
                    fileSizeLimitBytes: 512 * 1000 * 1000, // 512 MB
                    shared: false
                );
            }

            string? telegramBotToken = config.TelegramBotToken;
            string? telegramChatId = config.TelegramChatId;
            if(
                ( string.IsNullOrWhiteSpace( telegramBotToken ) == false ) &&
                ( string.IsNullOrWhiteSpace( telegramChatId ) == false )
            )
            {
                useTelegramLogger = true;
                var telegramOptions = new TelegramSinkOptions(
                    botToken: telegramBotToken,
                    chatId: telegramChatId,
                    dateFormat: "dd.MM.yyyy HH:mm:sszzz",
                    applicationName: nameof( Rau ),
                    failureCallback: onTelegramFailure
                );
                logger.WriteTo.Telegram(
                    telegramOptions,
                    restrictedToMinimumLevel: config.TelegramLogLevel.ToSerilogLevel()
                );
            }

            Serilog.ILogger log = logger.CreateLogger();
            log.Information( $"Using File Logging: {useFileLogger}." );
            log.Information( $"Using Telegram Logging: {useTelegramLogger}." );

            return log;
        }

        public static WebApplication ConfigurePdsServices(
            this WebApplicationBuilder builder,
            IRauApi api
        )
        {
            builder.Services.AddSingleton<IRauApi>( api );
            builder.Services.AddQuartz(
                q =>
                {
                    #if false
                    JobKey jobKey = JobKey.Create( typeof( TJob ).Name );
                    q.AddJob<TJob>( jobKey );

                    q.AddTrigger(
                        ( ITriggerConfigurator trigerConfig ) =>
                        {
                            trigerConfig.WithCronSchedule(
                                config.CronString,
                                ( CronScheduleBuilder cronBuilder ) =>
                                {
                                    // Server is in NY.
                                    cronBuilder.InTimeZone( TimeZoneInfo.FindSystemTimeZoneById( "America/New_York" ) );
                                    // If we misfire, just do nothing.  This isn't exactly
                                    // the most important application
                                    cronBuilder.WithMisfireHandlingInstructionDoNothing();
                                    cronBuilder.Build();
                                }
                            );
                            trigerConfig.WithDescription( $"Chirp!" );
                            trigerConfig.ForJob( jobKey );
                            trigerConfig.StartNow();
                        }
                    );
                    #endif
                }
            );

            builder.Services.AddQuartzHostedService(
                options =>
                {
                    options.AwaitApplicationStarted = true;
                    options.WaitForJobsToComplete = true;
                }
            );

            WebApplication app = builder.Build();
            if( api.Config.MetricsPort is not null )
            {
                builder.WebHost.UseUrls( $"http://0.0.0.0:{api.Config.MetricsPort}" );

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

            return app;
        }
    }
}
