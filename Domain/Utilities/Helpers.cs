using System;
using System.Collections.Generic;
using System.Globalization;

namespace CryptoKeeper.Domain.Utilities
{
    public static class Helpers
    {
        private static long CurrentHttpPostNonce { get; set; }

        internal static string GetCurrentHttpPostNonce()
        {
            var newHttpPostNonce = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 1514861849000;
            if (newHttpPostNonce > CurrentHttpPostNonce)
            {
                CurrentHttpPostNonce = newHttpPostNonce;
            }
            else
            {
                CurrentHttpPostNonce += 1;
            }
            return CurrentHttpPostNonce.ToString(CultureInfo.InvariantCulture);
        }

        internal static string ToHttpPostString(this Dictionary<string, object> dictionary)
        {
            var output = string.Empty;
            foreach (var entry in dictionary)
            {
                if (!(entry.Value is string valueString))
                {
                    output += "&" + entry.Key + "=" + entry.Value;
                }
                else
                {
                    output += "&" + entry.Key + "=" + valueString.Replace(' ', '+');
                }
            }

            return output.Substring(1);
        }
    }
}
