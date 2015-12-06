using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;

namespace Devillers.CanonicalVerifier
{
    public class CanonicalSchemaSet
    {
        public List<CanonicalSchema> Schemas { get; set; }
        public List<ValidationEventArgs> XmlSchemaValidationErrors { get; set; }

        public CanonicalSchemaSet(string directory)
        {
            Schemas = System.IO.Directory.EnumerateFiles(directory, "*.xsd")
                .AsParallel()
                .Select(CanonicalSchema.Create)
                .ToList();

            XmlSchemaValidationErrors = Schemas
                .Select(x => x.XmlSchema)
                .Aggregate(new XmlSchemaSet(), AddToSet, CompileSetToErrors)
                .ToList();
        }

        private static XmlSchemaSet AddToSet(XmlSchemaSet set, XmlSchema schema)
        {
            if(!set.Contains(schema.TargetNamespace))
                set.Add(schema);
            return set;
        }

        private static IEnumerable<ValidationEventArgs> CompileSetToErrors(XmlSchemaSet set)
        {
            var errors = new List<ValidationEventArgs>();
            set.ValidationEventHandler += (o, e) => errors.Add(e);
            set.Compile();
            return errors;
        }
    }
}