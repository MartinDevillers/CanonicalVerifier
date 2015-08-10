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

namespace Devillers.CanonicalVerifier
{
    public static class Program
    {
        private static int errorCount;
        private static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Program));
        static void Main(string[] args)
        {
            if (args.Length != 1 || string.IsNullOrEmpty(args[0]) || !System.IO.Directory.Exists(args[0]))
            {
                Error("!!!", "Argument missing or not an existing directory");
                Environment.Exit(-2);
            }

            Info("START", "Started Devillers.CanonicalVerifier for directory: " + args[0]);

            var set = new XmlSchemaSet();

            foreach (var file in System.IO.Directory.EnumerateFiles(args[0], "*.xsd"))
            {
                string filename = System.IO.Path.GetFileName(file);

                try
                {
                    Info(filename, "Started file-level validation");

                    bool messageSchema = filename.StartsWith(Settings.MessageFilePrefix);
                    string name = null;
                    string version = null;
                    string namespaceSuffix = null;

                    Info(filename, messageSchema ? "This is a message schema" : "This is a domain schema");

                    var fileMatch = Regex.Match(filename, @"^(?<name>[a-zA-Z]+)\.(?<version>v[0-9])+\.xsd$");
                    if (fileMatch.Success)
                    {
                        name = fileMatch.Groups["name"].Value;
                        if (messageSchema)
                        {
                            name = name.Substring(Settings.MessageFilePrefix.Length);
                        }
                        version = fileMatch.Groups["version"].Value;
                        namespaceSuffix = string.Concat(name.ToLowerInvariant(), ":", version.ToLowerInvariant());

                        Info(filename, "Namespace based on filename: " + namespaceSuffix);
                    }
                    else
                    {
                        Error(filename, "Filename does not match <name>.<version> convention.");
                    }

                    var xsd = XmlSchema.Read(new XmlTextReader(file), (o, e) => Error(file, e.Message));

                    if (messageSchema)
                    {
                        if (Settings.MessageNamespacePrefix != null && !xsd.TargetNamespace.StartsWith(Settings.MessageNamespacePrefix))
                        {
                            Error(filename, string.Format("Root targetNamespace does not start with expected message prefix. Expected: {0}, Actual: {1}", Settings.MessageNamespacePrefix, xsd.TargetNamespace));
                        }
                        if (xsd.Includes.OfType<XmlSchema>().Any(x => !xsd.TargetNamespace.StartsWith(Settings.MessageNamespacePrefix)))
                        {
                            Error(filename, string.Format("Message schema should have no dependencies on other message schema's."));
                        }
                        if (!xsd.Items.OfType<XmlSchemaElement>().Select(x => x.Name).SequenceEqual(xsd.Items.OfType<XmlSchemaComplexType>().Select(x => x.Name)))
                        {
                            Error(filename, string.Format("Message schema should have an element for each complex type at the top level (Garden of Eden style)."));
                        }
                    }
                    else
                    {
                        if (Settings.DomainNamespacePrefix != null && !xsd.TargetNamespace.StartsWith(Settings.DomainNamespacePrefix))
                        {
                            Error(filename, string.Format("Root targetNamespace does not start with expected domain prefix. Expected: {0}, Actual: {1}", Settings.DomainNamespacePrefix, xsd.TargetNamespace));
                        }
                        if (xsd.Includes.Count != 0)
                        {
                            Error(filename, string.Format("Domain schema should have no external dependencies. Found {0} import-statements.", xsd.Includes.Count));
                        }
                        if (xsd.Items.OfType<XmlSchemaElement>().Any())
                        {
                            Error(filename, string.Format("Domain schema should only have complex types at the top level (*NO* Garden of Eden style)."));
                        }
                    }
                
                    if (namespaceSuffix != null && !xsd.TargetNamespace.EndsWith(namespaceSuffix))
                    {
                        Error(filename, string.Format("Root targetNamespace does not end with expected suffix. Expected: {0}, Actual: {1}", namespaceSuffix, xsd.TargetNamespace));
                    }

                    int elementCount = 0;
                    foreach (var element in All<XmlSchemaElement>(xsd.Items))
                    {
                        decimal minOccurs = element.MinOccurs;
                        decimal maxOccurs = element.MinOccurs;
                        bool nillable = element.IsNillable;

                        if (element.SchemaTypeName.Namespace == XmlSchema.Namespace)
                        {
                            if (!new[] {"string", "boolean", "int", "double", "decimal", "long", "date", "dateTime", "base64Binary"}.Contains(element.SchemaTypeName.Name))
                            {
                                Error(filename, element, string.Format("Primitive type '{0}' is not supported", element.SchemaTypeName.Name));
                            }
                        }
                        else if (!element.SchemaTypeName.Namespace.StartsWith(Settings.DomainNamespacePrefix))
                        {
                            Error(filename, element, "An element must be either a primitive type or a known schema type. Unknown namespace: " + element.SchemaTypeName.Namespace);
                        }

                        if (element.Block != XmlSchemaDerivationMethod.None)
                        {
                            Error(filename, element, "An element should not have a block specifier");
                        }
                        if (element.DefaultValue != null)
                        {
                            Error(filename, element, "An element should not have a default value");
                        }
                        if (element.Final != XmlSchemaDerivationMethod.None)
                        {
                            Error(filename, element, "An element should not have a final specifier");
                        }
                        if (element.FixedValue != null)
                        {
                            Error(filename, element, "An element should not have a fixed value");
                        }
                        if (element.Form != XmlSchemaForm.None)
                        {
                            Error(filename, element, "An element should have no form specifier");
                        }
                        if (element.Id != null)
                        {
                            Error(filename, element, "An element should have no id value");
                        }
                        if (!element.RefName.IsEmpty)
                        {
                            Error(filename, element, "An element should have no ref value");
                        }
                        if (!element.SubstitutionGroup.IsEmpty)
                        {
                            Error(filename, element, "An element should have an empty substitution group");
                        }
                        if (element.MinOccurs == 0 && !element.IsNillable)
                        {
                            Error(filename, element, "An element with minOccurs=0 should have nillable=true");
                        }
                        if (element.MinOccurs != 0 && element.IsNillable)
                        {
                            Error(filename, element, "An element with minOccurs!=0 should have nillable=false");
                        }
                        if (element.MaxOccurs != 0 && element.MaxOccurs != 1 && element.MaxOccursString != "unbounded")
                        {
                            Error(filename, element, "Only maxOccurs=0, maxOccurs=1 or maxOccurs=unbounded are allowed on an element");
                        }
                        if (element.Name[0] != char.ToUpperInvariant(element.Name[0]))
                        {
                            Error(filename, element, "Name of element should always start with uppercase");
                        }
                        elementCount++;
                    }
                    foreach (var complexType in All<XmlSchemaComplexType>(xsd.Items))
                    {
                        if (complexType.Block != XmlSchemaDerivationMethod.None)
                        {
                            Error(filename, complexType, "A complex type should not have a block specifier");
                        }
                        if (complexType.Final != XmlSchemaDerivationMethod.None)
                        {
                            Error(filename, complexType, "A complex type should not have a final specifier");
                        }
                        if (complexType.IsMixed)
                        {
                            Error(filename, complexType, "A complex type should not be mixed");
                        }

                        if (complexType.IsAbstract && !complexType.Name.EndsWith("Base"))
                        {
                            Error(filename, complexType, "Abstract types should have a name ending with 'Base'");
                        }

                        if (messageSchema)
                        {
                            if (complexType.Name.EndsWith("Request"))
                            {
                                if (complexType.ContentModel != null && complexType.ContentModel.Content is XmlSchemaComplexContentExtension)
                                {
                                    var extension = complexType.ContentModel.Content as XmlSchemaComplexContentExtension;
                                    if (!extension.BaseTypeName.Name.EndsWith("RequestBase"))
                                    {
                                        Error(filename, complexType, "Expected Request-type to extend RequestBase");
                                    }
                                }
                                else
                                {
                                    Error(filename, complexType, "Expected Request-type to extend a base-type");
                                }
                            }
                            if (complexType.Name.EndsWith("Response"))
                            {
                                if (complexType.ContentModel != null && complexType.ContentModel.Content is XmlSchemaComplexContentExtension)
                                {
                                    var extension = complexType.ContentModel.Content as XmlSchemaComplexContentExtension;
                                    if (!extension.BaseTypeName.Name.EndsWith("ResponseBase"))
                                    {
                                        Error(filename, complexType, "Expected Response-type to extend ResponseBase");
                                    }
                                }
                                else
                                {
                                    Error(filename, complexType, "Expected Response-type to extend a base-type");
                                }
                            }
                        }
                    }

                    Info(filename, "Validated " + elementCount + " elements");

                    if (!set.Contains(xsd.TargetNamespace))
                    {
                        set.Add(xsd);
                    }
                }
                catch (Exception ex)
                {
                    Error(filename, "Exception occurred: " + ex.Message);
                }
            }

            Info("SET", "Started set-level validation");
            set.ValidationEventHandler += (o, e) => Error(System.IO.Path.GetFileName(e.Exception.SourceUri), e.Exception.SourceSchemaObject, e.Message);
            set.Compile();
            Info("SET", "Validated set of " + set.Count + " schemas");

            Info("STOP", errorCount + " errors detected");

            Environment.Exit(errorCount == 0 ? 0 : -1);
        }


        private static IEnumerable<T> All<T>(XmlSchemaObjectCollection items) where T : XmlSchemaObject
        {
            foreach (var item in items)
            {
                var ct = item as XmlSchemaComplexType;
                if (ct != null)
                {
                    var xss = ct.Particle as XmlSchemaSequence;
                    if (xss != null)
                    {
                        foreach (var item1 in All<T>(xss.Items))
                        {
                            yield return item1;
                        }
                    }
                    if (ct.ContentModel != null)
                    {
                        var xscce = ct.ContentModel.Content as XmlSchemaComplexContentExtension;
                        if (xscce != null)
                        {
                            var xss2 = xscce.Particle as XmlSchemaSequence;
                            if (xss2 != null)
                            {
                                foreach (var item1 in All<T>(xss2.Items))
                                {
                                    yield return item1;
                                }
                            }
                        }
                    }
                }
                var value = item as T;
                if (value != null)
                {
                    yield return value;
                }
            }
        }

        static void Error(string file, XmlSchemaElement element, string message)
        {
            Error(file, string.Format("{0},{1}: Element: {2}: {3}", element.LineNumber, element.LinePosition, element.Name, message));
        }

        static void Error(string file, XmlSchemaComplexType element, string message)
        {
            Error(file, string.Format("{0},{1}: Type: {2}: {3}", element.LineNumber, element.LinePosition, element.Name, message));
        }

        static void Error(string file, XmlSchemaObject element, string message)
        {
            Error(file, string.Format("{0},{1}: {2}", element.LineNumber, element.LinePosition, message));
        }

        static void Error(string file, string message)
        {
            logger.ErrorFormat("{0}: {1}", file, message);
            errorCount++;
        }

        static void Info(string file, string message)
        {
            logger.InfoFormat("{0}: {1}", file, message);
        }
    }
}
