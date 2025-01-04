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

using Rau.Standard;

namespace Rau.Plugins.Canary
{
    public static class IRauApiExtensions
    {
        public static CanaryPlugin GetCanaryPlugin( this IRauApi api )
        {
            return api.GetPlugin<CanaryPlugin>( CanaryPlugin.PluginGuid );
        }

        /// <summary>
        /// Add an account that a canary message will get posted to every so often.
        /// </summary>
        /// <param name="account">
        /// The account information.
        /// </param>
        /// <param name="post">
        /// The post contents.
        /// </param>
        /// <param name="cronString">
        /// How often to post to the account.
        /// See this documentation on how to make a correct cron string:
        /// https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontriggers.html#cron-expressions
        /// </param>
        public static void AddCanaryAccount( this IRauApi api, PdsAccount account, PdsPost post, string cronString )
        {
            api.GetCanaryPlugin().AccountManager.AddAccount( account, post, cronString );
        }

        /// <summary>
        /// Add an account that a canary message will get posted to every so often.
        /// </summary>
        /// <param name="account">
        /// The account information.
        /// </param>
        /// <param name="post">
        /// Function pointer to retrieve a post.  This can be used if the post
        /// needs to change each time.
        /// </param>
        /// <param name="cronString">
        /// How often to post to the account.
        /// See this documentation on how to make a correct cron string:
        /// https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontriggers.html#cron-expressions
        /// </param>
        public static void AddCanaryAccount( this IRauApi api, PdsAccount account, Func<PdsPost> post, string cronString )
        {
            api.GetCanaryPlugin().AccountManager.AddAccount( account, post, cronString );
        }

        /// <summary>
        /// Add an account that a default canary message will get posted to every so often.
        /// The default message includes the hostname, the uptime, and a time stamp.
        /// </summary>
        /// <param name="account">
        /// The account information.
        /// </param>
        /// <param name="cronString">
        /// How often to post to the account.
        /// See this documentation on how to make a correct cron string:
        /// https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontriggers.html#cron-expressions
        /// </param>   
        public static void AddCanaryAccountWithDefaultMessage( this IRauApi api, PdsAccount account, string cronString )
        {
            api.AddCanaryAccount( account, () => CanaryPlugin.DefaultPost( api.DateTime, account ), cronString );
        }
    }
}
