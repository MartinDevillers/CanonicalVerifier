using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Devillers.CanonicalVerifier
{
    public class Settings
    {
        public static string DomainNamespacePrefix { get { return GetString("DomainNamespacePrefix"); } }
        public static string DomainFilePrefix { get { return GetString("DomainFilePrefix"); } }
        public static string MessageNamespacePrefix { get { return GetString("MessageNamespacePrefix"); } }
        public static string MessageFilePrefix { get { return GetString("MessageFilePrefix"); } }
        private static string GetString(string key)
        {
            var value = ConfigurationManager.AppSettings[key];

            if (string.IsNullOrWhiteSpace(value))
                return null;
            else
                return value;
        }

        private static bool GetBoolean(string key)
        {
            var value = ConfigurationManager.AppSettings[key];

            bool result;
            if (bool.TryParse(value, out result))
                return result;
            else
                return false;
        }
    }
}
