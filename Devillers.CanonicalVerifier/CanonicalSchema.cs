using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;

namespace Devillers.CanonicalVerifier
{
    public class CanonicalSchema
    {
        public bool IsMessageSchema { get; private set; }
        public string Name { get; private set; }
        public string Version { get; private set; }
        public string NamespaceSuffix { get; private set; }
        public XmlSchema XmlSchema { get; set; }
        public List<ValidationEventArgs> XmlSchemaValidationErrors { get; set; }
        public XmlException XmlException { get; set; }

        private CanonicalSchema(string file)
        {
            string filename = System.IO.Path.GetFileName(file);
            IsMessageSchema = filename.StartsWith(Settings.MessageFilePrefix);
            XmlSchemaValidationErrors = new List<ValidationEventArgs>();
            try
            {
                XmlSchema = XmlSchema.Read(new XmlTextReader(file), (o, e) => XmlSchemaValidationErrors.Add(e));
            }
            catch (XmlException ex)
            {
                XmlException = ex;
            }
            
            var fileMatch = Regex.Match(filename, @"^(?<name>[a-zA-Z]+)\.(?<version>v[0-9])+\.xsd$");
            if (fileMatch.Success)
            {
                Name = fileMatch.Groups["name"].Value;
                if (IsMessageSchema)
                {
                    Name = Name.Substring(Settings.MessageFilePrefix.Length);
                }
                Version = fileMatch.Groups["version"].Value;
                NamespaceSuffix = string.Concat(Name.ToLowerInvariant(), ":", Version.ToLowerInvariant());

            }
            else
            {
                //Error(filename, "Filename does not match <name>.<version> convention.");
            }
        }

        public static CanonicalSchema Create(string file)
        {
            return new CanonicalSchema(file);
        }
    }
}