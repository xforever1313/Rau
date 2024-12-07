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

using System.Net.Http.Headers;
using Rau.Standard.Configuration;
using SethCS.Exceptions;

namespace Rau.Configuration
{
    internal static class RauConfigExtensions
    {
        /// <summary>
        /// Tries to validate the configuration object.
        /// Returns an empty list if there are no configuration failures.
        /// </summary>
        public static List<string> TryValidate( this RauConfig config )
        {
            var errors = new List<string>();

            return errors;
        }

        public static void Validate( this RauConfig config )
        {
            List<string> errors = config.TryValidate();

            if( config.CharacterLimit == 0 )
            {
                errors.Add( $"{nameof( config.CharacterLimit )} can not be zero." );
            }

            if( config.MetricsPort == 0 )
            {
                errors.Add( $"{nameof( config.MetricsPort )} can not be zero.)" );
            }

            if(
                string.IsNullOrWhiteSpace( config.TelegramBotToken ) &&
                ( string.IsNullOrWhiteSpace( config.TelegramChatId ) == false )
            )
            {
                errors.Add(
                    $"{nameof( config.TelegramBotToken )} can not be empty if {config.TelegramChatId} is specified."
                );
            }
            else if(
                ( string.IsNullOrWhiteSpace( config.TelegramBotToken ) == false )&&
                string.IsNullOrWhiteSpace( config.TelegramChatId )
            )
            {
                errors.Add(
                    $"{nameof( config.TelegramChatId )} can not be empty if {config.TelegramBotToken} is specified."
                );
            }

            try
            {
                var _ = new ProductInfoHeaderValue( config.UserAgentName, config.UserAgentVersion?.ToString( 3 ) );
            }
            catch( ThreadInterruptedException )
            {
                throw;
            }
            catch( Exception e )
            {
                errors.Add( "Invalid user agent: " + e.Message );
            }
            
            if( errors.Any() )
            {
                throw new ListedValidationException( 
                    $"Errors validating {nameof( RauConfig )}.",
                    errors 
                );
            }
        }
    }
}