using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Devillers.CanonicalVerifier.Rules;

namespace Devillers.CanonicalVerifier
{
    public static class Program
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Program));
        static void Main(string[] args)
        {
            if (args.Length != 1 || string.IsNullOrEmpty(args[0]) || !System.IO.Directory.Exists(args[0]))
            {
                logger.Error("Argument missing or not an existing directory");
                Environment.Exit(-2);
            }

            logger.Info("Started Devillers.CanonicalVerifier for directory: " + args[0]);

            var validator = new CanonicalSchemaSetValidator();
            var schemaSet = new CanonicalSchemaSet(args[0]);

            logger.InfoFormat("Parsed {0} schemas", schemaSet.Schemas.Count);

            var result = validator.Validate(schemaSet);
            int errors = 0, warnings = 0;

            foreach (var failure in result.Errors)
            {
                string message = string.Format("{0}:{1}: {2} (actual: {3})", failure.ErrorCode, failure.PropertyName, failure.ErrorMessage, failure.AttemptedValue);

                switch (Settings.GetSettingForRule(failure.ErrorCode))
                {
                    case RuleSetting.Error:
                        logger.Error(message);
                        errors++;
                        break;
                    case RuleSetting.Warning:
                        logger.Warn(message);
                        warnings++;
                        break;
                }
            }

            logger.InfoFormat("Found {0} errors and {1} warnings", errors, warnings);

            Environment.Exit(errors == 0 ? 0 : -1);
        }
    }
}
