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

using Rau.Standard.Logging;

namespace Rau.Logging
{
    internal sealed class RauLogger : IRauLogger
    {
        // ---------------- Fields ----------------

        private readonly Serilog.ILogger log;

        // ---------------- Constructor ----------------

        public RauLogger( Serilog.ILogger log )
        {
            this.log = log;
        }
        
        // ---------------- Methods ----------------
        public void Debug( string message )
        {
            this.log.Debug( message );
        }

        public void Verbose( string message )
        {
            this.log.Verbose( message );
        }

        public void Information( string message )
        {
            this.log.Information( message );
        }

        public void Warning( string message )
        {
            this.log.Warning( message );
        }

        public void Error( string message )
        {
            this.log.Error( message );
        }

        public void Fatal( string message )
        {
            this.log.Fatal( message );
        }
    }
}
