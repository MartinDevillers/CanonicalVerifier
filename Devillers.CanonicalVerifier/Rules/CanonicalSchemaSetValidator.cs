using FluentValidation;

namespace Devillers.CanonicalVerifier.Rules
{
    public class CanonicalSchemaSetValidator : AbstractValidator<CanonicalSchemaSet>
    {
        public CanonicalSchemaSetValidator()
        {
            RuleForEach(x => x.XmlSchemaValidationErrors)
                .Empty()
                .WithMessage("{0}: {1}", (x, y) => y.Severity, (x, y) => y.Message)
                .WithErrorCode("XSD001");
            
            RuleForEach(x => x.Schemas).SetValidator(new CanonicalSchemaValidator())
                .OverridePropertyName("S");
        }
    }
}
