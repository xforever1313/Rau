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

using Rau.Standard;

namespace Rau.Plugins.Canary
{
    public sealed class AccountManager
    {
        // ---------------- Fields ----------------

        private readonly Dictionary<PdsAccount, int> accounts;

        private readonly IRauApi api;

        // ---------------- Constructor ----------------

        public AccountManager( IRauApi api )
        {
            this.accounts = new Dictionary<PdsAccount, int>();
            this.api = api;
        }

        // ---------------- Methods ----------------

        /// <summary>
        /// Adds the account with the given post details.
        /// </summary>
        /// <param name="cronString">
        /// How often to post.
        /// </param>
        /// <param name="account">The account that will post.</param>
        /// <param name="post">What to post.</param>
        public void AddAccount( PdsAccount account, PdsPost post, string cronString )
        {
            if( this.accounts.ContainsKey( account ) )
            {
                throw new ArgumentException( $"Account {account} already added" );
            }
            int eventId = this.api.EventScheduler.ConfigureEvent(
                new ChirpEvent( account, post, cronString, this.api.PdsPoster )
            );
            this.accounts[account] = eventId;
        }

        /// <summary>
        /// Removes the given account, which means the account will no longer post.
        /// If the account does not exist, this becomes a no-op.
        /// </summary>
        public void RemoveAccount( PdsAccount account )
        {
            if( this.accounts.ContainsKey( account ) )
            {
                this.api.EventScheduler.RemoveEvent( this.accounts[account] );
                this.accounts.Remove( account );
            }
        }

        // ---------------- Helper Classes ----------------

        private sealed class PostConfig
        {
            public required PdsPost Post { get; init; }

            public required string CronString { get; init; }
        }
    }
}
