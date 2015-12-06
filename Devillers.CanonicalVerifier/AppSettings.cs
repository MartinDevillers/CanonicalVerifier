using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Devillers.CanonicalVerifier
{
    public enum RuleSetting
    {
        Ignore,
        Warning,
        Error
    }

    public static class Settings
    {
        public static string DomainNamespacePrefix { get { return GetString("DomainNamespacePrefix"); } }
        public static string DomainFilePrefix { get { return GetString("DomainFilePrefix"); } }
        public static string MessageNamespacePrefix { get { return GetString("MessageNamespacePrefix"); } }
        public static string MessageFilePrefix { get { return GetString("MessageFilePrefix"); } }


        private static Dictionary<string, RuleSetting> rules;
        public static Dictionary<string, RuleSetting> Rules
        {
            get
            {
                return rules ?? (rules = ((Hashtable) ConfigurationManager.GetSection("rules"))
                    .Cast<DictionaryEntry>()
                    .ToDictionary(x => (string) x.Key, x => (RuleSetting) Enum.Parse(typeof (RuleSetting), (string) x.Value)));
            }
        }
 
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

        public static RuleSetting GetSettingForRule(string code)
        {
            if (Rules.ContainsKey(code))
                return Rules[code];
            return RuleSetting.Error;
        }
    }
}
