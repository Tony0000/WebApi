using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Common
{
    public static class StringExtensions
    {
        public static IDictionary<string, string> ToDictionary(this string queryString)
        {
            var dict = new Dictionary<string, string>();
            queryString = queryString.Replace("?", string.Empty);
            var keyValueList = queryString.Split('&');
            foreach (var keyValue in keyValueList)
            {
                var kv = keyValue.Split('=');
                if (!string.IsNullOrWhiteSpace(kv[0]) && !string.IsNullOrWhiteSpace(kv[1]))
                {
                    kv[0] = kv[0];
                    dict[kv[0]] = kv[1].Unescape();
                }
            }

            return dict;
        }

        public static string Unescape(this string input) => input switch
        {
            null => throw new ArgumentNullException(nameof(input)),
            "" => throw new ArgumentException($"{nameof(input)} cannot be empty"),
            _ => Uri.UnescapeDataString(input)
        };
        
        
        public static string FirstCharToUpper(this string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty"),
                _ => input.First().ToString().ToUpper() + input.Substring(1)
            };
    
    }
}