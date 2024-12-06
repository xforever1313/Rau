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

using Rau.Standard.EventScheduler;
using Quartz;
using Rau.Standard;
using Serilog.Extensions.Logging;

namespace Rau.EventScheduler
{
    /// <summary>
    /// Manages timed events.
    /// </summary>
    internal sealed class ScheduledEventManager : IDisposeableScheduledEventManager
    {
        // ---------------- Fields ----------------

        private const string apiJobKey = "api";

        private const string eventInfoKey = "eventInfo";

        private readonly RauApi api;

        private readonly Dictionary<int, ITrigger> events;

        private readonly IScheduler taskScheduler;

        private readonly IJobDetail job;

        private int nextId;

        // ---------------- Constructor ----------------

        public ScheduledEventManager( RauApi api, Serilog.ILogger log )
        {
            this.api = api;
            this.events = new Dictionary<int, ITrigger>();

            var msLogger = new SerilogLoggerFactory( log );
            Quartz.Logging.LogContext.SetCurrentLogProvider( msLogger );

            this.taskScheduler = SchedulerBuilder.Create()
                .WithName( $"{nameof( RauApi )} scheduler" )
                .WithInterruptJobsOnShutdown( true )
                .UseDedicatedThreadPool()
                .BuildScheduler()
                .Result;

            var jobData = new JobDataMap
            {
                [apiJobKey] = this.api
            };

            this.job = JobBuilder.Create<RauJob>()
                .WithIdentity( "Rau Event" )
                .StoreDurably()
                .SetJobData( jobData )
                .Build();

            this.taskScheduler.AddJob( this.job, false );

            this.nextId = 1;
        }

        // ---------------- Functions ----------------

        public void Start()
        {
            this.taskScheduler.Start();
        }

        public void Dispose()
        {
            this.taskScheduler.DeleteJob( this.job.Key );
            this.taskScheduler.Shutdown();
        }

        public int ConfigureEvent( ScheduledEvent e )
        {
            ITrigger CreateTrigger( string eventName )
            {
                var jobData = new JobDataMap
                {
                    [eventInfoKey] = e
                };

                ITrigger trigger = TriggerBuilder.Create()
                    .WithCronSchedule(
                        e.CronString,
                        ( CronScheduleBuilder cronBuilder ) =>
                        {
                            cronBuilder.InTimeZone( e.TimeZone );
                            cronBuilder.WithMisfireHandlingInstructionFireAndProceed();
                        }
                    )
                    .WithIdentity( eventName )
                    .UsingJobData( jobData )
                    .ForJob( this.job.Key )
                    .StartNow()
                    .Build();

                return trigger;
            }

            if( e.Id == 0 )
            {
                e.Id = nextId;
                ++nextId;

                string eventName = e.GetEventName();

                ITrigger trigger = CreateTrigger( eventName );
                this.events[e.Id] = trigger;
                this.taskScheduler.ScheduleJob( trigger );
            }
            else if( this.events.ContainsKey( e.Id ) == false )
            {
                throw new ArgumentException(
                    $"Can not find event with ID: {e.Id}",
                    nameof( e )
                );
            }
            else
            {
                string eventName = e.GetEventName();

                ITrigger trigger = this.events[e.Id];

                TriggerKey key = trigger.Key;
                trigger = CreateTrigger( eventName );
                this.taskScheduler.RescheduleJob( key, trigger );
            }

            return e.Id;
        }

        public void RemoveEvent( int eventId )
        {
            if( this.events.TryGetValue( eventId, out var trigger ) == false )
            {
                // If an event doesn't exist, just let it go.
                return;
            }

            this.taskScheduler.UnscheduleJob( trigger.Key );
            this.events.Remove( eventId );
        }

        private class RauJob : IJob
        {
            public Task Execute( IJobExecutionContext context )
            {
                object apiObj = context.MergedJobDataMap.Get( apiJobKey );
                if( apiObj is not IRauApi api )
                {
                    throw new InvalidCastException(
                        $"Could not cast job data type to {nameof( IRauApi )}."
                    );
                }

                object eventInfoObj = context.MergedJobDataMap.Get( eventInfoKey );
                if( eventInfoObj is not ScheduledEvent e )
                {
                    throw new InvalidCastException(
                        $"Could not cast job data type to {nameof( ScheduledEvent )}."
                    );
                }

                var eventParams = new ScheduledEventParameters( api, context );
                return e.ExecuteEvent( eventParams );
            }
        }
    }
}
