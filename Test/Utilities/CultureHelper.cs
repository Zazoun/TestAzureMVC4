using System;
using System.Collections.Generic;
using System.Linq;

namespace Test.Utilities
{
    public static class CultureHelper
    {
        private static readonly Dictionary<String, bool> _cultures = new Dictionary<string, bool> {
            {"fr-FR", true},
            {"en-US", false},
            {"de-DE", false}
        };

        public static string GetValidCulture(string name)
        {
            if (string.IsNullOrEmpty(name))
                return GetDefaultCulture();

            if (_cultures.ContainsKey(name))
                return name;

            // Find a close match. For example, if you have "en-US" defined and the user requests "en-GB", 
            // the function will return closes match that is "en-US" because at least the language is the same (ie English)
            return _cultures.Keys.DefaultIfEmpty(GetDefaultCulture()).FirstOrDefault(c => c.StartsWith(name.Substring(0, 2)));
        }

        public static string GetDefaultCulture()
        {
            return _cultures.Keys.ElementAt(0); // return Default culture

        }

        public static bool IsViewSeparate(string name)
        {
            return _cultures[name];
        }

    }
}