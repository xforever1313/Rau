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

using System.Net;
using System.Net.Http.Headers;
using Rau.Standard.Configuration;

namespace Rau
{
    internal class BskyHttpClientFactory : IHttpClientFactory, IDisposable
    {
        // ---------------- Fields ----------------
        
        private readonly HttpClient client;

        private bool isDisposed;

        // ---------------- Constructor ----------------

        public BskyHttpClientFactory( RauConfig config )
        {
            this.isDisposed = false;
            var handler = new UrlModifyingHandler( config )
            {
                InnerHandler = new HttpClientHandler
                {
                    AutomaticDecompression = ( DecompressionMethods.GZip | DecompressionMethods.Deflate )
                }
            };

            this.client = new HttpClient( handler );
        }

        // ---------------- Methods ----------------

        public HttpClient CreateClient( string name )
        {
            if( this.isDisposed )
            {
                throw new ObjectDisposedException( GetType().Name );
            }

            return client;
        }

        public void Dispose()
        {
            if( this.isDisposed )
            {
                return;
            }

            this.client.Dispose();
            this.isDisposed = true;
        }

        private sealed class UrlModifyingHandler : DelegatingHandler
        {
            // ---------------- Fields ----------------

            private readonly RauConfig config;

            // ---------------- Constructor ----------------

            public UrlModifyingHandler( RauConfig config )
            {
                this.config = config;
            }

            // ---------------- Methods ----------------

            protected override async Task<HttpResponseMessage> SendAsync( HttpRequestMessage request, CancellationToken cancellationToken )
            {
                ArgumentNullException.ThrowIfNull( request.RequestUri );
                request.Headers.UserAgent.Clear();
                request.Headers.UserAgent.Add( new ProductInfoHeaderValue( this.config.UserAgentName, this.config.UserAgentVersion?.ToString( 3 ) ) );

                return await base.SendAsync( request, cancellationToken );
            }
        }
    }
}
