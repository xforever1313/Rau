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

using Serilog.Events;
using Rau.Standard.Logging;

namespace Rau.Logging
{
    internal static class LoggingEnumsExtensions
    {
        public static LogEventLevel ToSerilogLevel( this RauLogLevel level ) => level switch
        {
            RauLogLevel.Verbose => LogEventLevel.Verbose,
            RauLogLevel.Debug => LogEventLevel.Debug,
            RauLogLevel.Information => LogEventLevel.Information,
            RauLogLevel.Warning => LogEventLevel.Warning,
            RauLogLevel.Error => LogEventLevel.Error,
            RauLogLevel.Fatal => LogEventLevel.Fatal,
            _ => throw new ArgumentException( $"Unknown log level: {level}", nameof( level ) )
        };
    }
}