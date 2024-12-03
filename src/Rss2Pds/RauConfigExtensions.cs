using Rau.Standard;

namespace Rau
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
    }
}