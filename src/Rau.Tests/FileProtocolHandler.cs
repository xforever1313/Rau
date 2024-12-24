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

namespace Rau.Tests
{
    public class FileProtocolHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken 
        )
        {
            if( request.RequestUri is null )
            {
                throw new InvalidOperationException( "Request URI is missing." );
            }
            else if( request.RequestUri.Scheme != "file" )
            {
                throw new InvalidOperationException( "Only file:// URIs are supported." );
            }

            if( request.Method != HttpMethod.Get )
            {
                throw new InvalidOperationException( "Only GET method is supported for file:// URIs." );
            }

            var response = new HttpResponseMessage( System.Net.HttpStatusCode.OK );
            var filePath = request.RequestUri.LocalPath;

            if( !File.Exists( filePath ) )
            {
                response.StatusCode = System.Net.HttpStatusCode.NotFound;
                return Task.FromResult( response );
            }

            var content = File.ReadAllBytes( filePath );
            response.Content = new ByteArrayContent( content );

            // Set the content type based on the file extension
            var contentType = "application/octet-stream"; // default
            var extension = Path.GetExtension( filePath ).ToLowerInvariant();
            switch( extension )
            {
                case ".txt":
                    contentType = "text/plain";
                    break;
                case ".html":
                case ".htm":
                    contentType = "text/html";
                    break;
                case ".json":
                    contentType = "application/json";
                    break;
                case ".xml":
                    contentType = "application/xml";
                    break;
            }

            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue( contentType );

            return Task.FromResult( response );
        }
    }
}
