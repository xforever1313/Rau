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

namespace Rau.Standard.EventScheduler
{
    public interface IScheduledEventParameters
    {
        /// <summary>
        /// API to the Rau system.
        /// </summary>
        IRauApi Api { get; }

        /// <summary>
        /// The time the event actually fired.
        /// </summary>
        DateTimeOffset FireTimeUtc { get; }

        /// <summary>
        /// Returns the cancellation token which will be cancelled when 
        /// the job cancellation has been requested. 
        /// </summary>
        CancellationToken CancellationToken { get; }
    }
}
