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
using Serilog.Extensions.Logging;
using X.Bluesky;

namespace Rau
{
    internal sealed class PdsPoster : IPdsPoster
    {
        // ---------------- Fields ---------------- 

        private readonly IRauApi api;

        private readonly IHttpClientFactory httpClientFactory;

        private readonly SerilogLoggerFactory msLogger;

        private static readonly string[] languages = new string[]
        {
            "en",
            "en-US"
        };

        // ---------------- Constructor ---------------- 

        public PdsPoster(
            IRauApi api,
            IHttpClientFactory httpClientFactory,
            Serilog.ILogger log
        )
        {
            this.api = api;
            this.httpClientFactory = httpClientFactory;
            this.msLogger = new SerilogLoggerFactory( log );
        }
        
        // ---------------- Methods ----------------
        
        public async Task Post( PdsAccount account, PdsPost postContents )
        {
            var client = new BlueskyClient(
                this.httpClientFactory,
                account.UserName,
                account.Password,
                languages,
                true,
                account.Instance,
                this.msLogger.CreateLogger<BlueskyClient>()
            );

            if( postContents.PostAttachmentPage is null )
            {
                await client.Post( postContents.PostContents );
            }
            else
            {
                await client.Post( postContents.PostContents, postContents.PostAttachmentPage );
            }
        }
    }
}