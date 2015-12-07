using System.Linq;
using System.Xml.Schema;
using FluentValidation;

namespace Devillers.CanonicalVerifier.Rules
{
    public class XmlSchemaDesignPatternValidator : AbstractValidator<XmlSchema>
    {
        public XmlSchemaDesignPatternValidator(bool gardenOfEdenStyle)
        {
            if(gardenOfEdenStyle)
            {
                RuleFor(x => x.Items)
                    .Must(x => x.OfType<XmlSchemaElement>().Select(y => y.Name).SequenceEqual(x.OfType<XmlSchemaComplexType>().Select(y => y.Name)))
                    .WithMessage("Schema should have an element for each complex type at the top level (Garden of Eden style).")
                    .WithErrorCode("DP001");
            }
            else
            {
                RuleFor(x => x.Items)
                    .Must(x => x.OfType<XmlSchemaObject>().All(y => y is XmlSchemaType))
                    .WithMessage("Schema should only have complex types at the top level (*NO* Garden of Eden style).")
                    .WithErrorCode("DP002");
            }
        }
    }
}
