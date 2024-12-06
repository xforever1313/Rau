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

namespace Rau
{
    internal sealed class PdsPoster : IPdsPoster
    {
        // ---------------- Fields ---------------- 
        
        private readonly IHttpClientFactory httpClientFactory;
        
        // ---------------- Constructor ---------------- 

        public PdsPoster( IHttpClientFactory httpClientFactory )
        {
            this.httpClientFactory = httpClientFactory;
        }
        
        // ---------------- Methods ----------------
        
        public Task Post( PdsAccount account, PdsPost postContents )
        {
            throw new NotImplementedException();
        }
    }
}