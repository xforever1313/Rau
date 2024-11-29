﻿//
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

using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Serilog;
using Serilog.Sinks.Telegram.Alternative;

namespace Rss2Pds
{
    internal static class HostingExtensions
    {
        public static Serilog.ILogger CreateLog(
            Rss2PdsConfig config,
            Action<Exception> onTelegramFailure
        )
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Console( Serilog.Events.LogEventLevel.Information );

            bool useFileLogger = false;
            bool useTelegramLogger = false;

            FileInfo? logFile = config.LogFile;
            if( logFile is not null )
            {
                useFileLogger = true;
                logger.WriteTo.File(
                    logFile.FullName,
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
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
                    applicationName: config.ApplicationContext,
                    failureCallback: onTelegramFailure
                );
                logger.WriteTo.Telegram(
                    telegramOptions,
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning
                );
            }

            Serilog.ILogger log = logger.CreateLogger();
            log.Information( $"Using File Logging: {useFileLogger}." );
            log.Information( $"Using Telegram Logging: {useTelegramLogger}." );

            return log;
        }

        public static IServiceCollection ConfigurePdsServices(
            this IServiceCollection services,
            Rss2PdsConfig config
        )
        {
            services.AddSingleton<Rss2PdsConfig>( config );
            services.AddQuartz(
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

            services.AddQuartzHostedService(
                options =>
                {
                    options.AwaitApplicationStarted = true;
                    options.WaitForJobsToComplete = true;
                }
            );

            return services;
        }
    }
}
