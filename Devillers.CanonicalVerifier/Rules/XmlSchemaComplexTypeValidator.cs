using System.Xml.Schema;
using FluentValidation;

namespace Devillers.CanonicalVerifier.Rules
{
    public class XmlSchemaComplexTypeValidator : AbstractValidator<XmlSchemaComplexType>
    {
        public XmlSchemaComplexTypeValidator()
        {
            RuleFor(x => x.Block)
                .Equal(XmlSchemaDerivationMethod.None)
                .WithMessage("A complex type should not have a block specifier")
                .WithErrorCode("CT001");

            RuleFor(x => x.Final)
                .Equal(XmlSchemaDerivationMethod.None)
                .WithMessage("A complex type should not have a final specifier")
                .WithErrorCode("CT002");

            RuleFor(x => x.IsMixed)
                .NotEqual(true)
                .WithMessage("A complex type should not be mixed")
                .WithErrorCode("CT003");

            RuleFor(x => x.Name)
                .Must(x => x.EndsWith("Base"))
                .When(x => x.IsAbstract)
                .WithMessage("Abstract types should have a name ending with 'Base'")
                .WithErrorCode("CT004");

            RuleFor(x => x.Name)
                .Matches("^[A-Z][a-z]+(?:[A-Z][a-z]+)*$")
                .WithMessage("The name of a complex type should always be Pascal Case")
                .WithErrorCode("CT005");
        }
    }
}
