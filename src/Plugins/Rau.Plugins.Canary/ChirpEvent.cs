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

using Rau.Standard;
using Rau.Standard.EventScheduler;

namespace Rau.Plugins.Canary
{
    public sealed class ChirpEvent : ScheduledEvent
    {
        // ---------------- Fields ----------------

        private readonly PdsAccount account;
        private readonly Func<PdsPost> post;
        private readonly IPdsPoster poster;

        // ---------------- Constructor ----------------

        public ChirpEvent( PdsAccount account, Func<PdsPost> post, string cronString, IPdsPoster poster )
        {
            this.account = account;
            this.post = post;
            this.CronString = cronString;
            this.poster = poster;
        }

        // ---------------- Properties ----------------

        public override string EventName => $"{account.UserName} Canary";

        public override string CronString { get; }

        // ---------------- Methods ----------------

        public override async Task ExecuteEvent( IScheduledEventParameters eventParams )
        {
            await poster.Post( this.account, this.post.Invoke() );
        }
    }
}
