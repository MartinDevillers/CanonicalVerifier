using System.Xml.Schema;
using FluentValidation;

namespace Devillers.CanonicalVerifier.Rules
{
    public class XmlSchemaRootNamespaceValidator : AbstractValidator<XmlSchema>
    {
        public XmlSchemaRootNamespaceValidator(string expectedNamespacePrefix, string expectedNamespacePostfix)
        {
            RuleFor(x => x.TargetNamespace)
                .Must(x => x.StartsWith(expectedNamespacePrefix))
                .WithMessage(
                    "Root targetNamespace does not start with expected prefix. Expected: {0}, Actual: {1}",
                    x => expectedNamespacePrefix, x => x.TargetNamespace)
                .WithErrorCode("RN001");

            RuleFor(x => x.TargetNamespace)
                .Must(x => x.EndsWith(expectedNamespacePostfix))
                .WithMessage(
                    "Root targetNamespace does not end with expected postfix. Expected: {0}, Actual: {1}",
                    x => expectedNamespacePostfix, x => x.TargetNamespace)
                .WithErrorCode("RN002");
        }
    }
}
