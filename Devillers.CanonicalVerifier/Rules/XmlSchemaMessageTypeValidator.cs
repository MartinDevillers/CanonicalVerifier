using System.Xml.Schema;
using FluentValidation;

namespace Devillers.CanonicalVerifier.Rules
{
    public class XmlSchemaMessageTypeValidator : AbstractValidator<XmlSchemaComplexType>
    {
        public XmlSchemaMessageTypeValidator(string messagePostfix)
        {
            When(x => x.Name.EndsWith(messagePostfix), () =>
            {
                RuleFor(x => x.ContentModel)
                    .NotNull()
                    .When(x => x.Name.EndsWith(messagePostfix))
                    .WithMessage("Expected message type to extend a base-type")
                    .WithErrorCode("MT001");

                RuleFor(x => x.ContentModel.Content)
                    .Must(x => (x as XmlSchemaComplexContentExtension).BaseTypeName.Name.EndsWith(messagePostfix + "Base"))
                    .When(x => x.Name.EndsWith(messagePostfix))
                    .When(x => x.ContentModel != null && x.ContentModel.Content is XmlSchemaComplexContentExtension)
                    .WithMessage("Expected message type to extend {0}Base", messagePostfix)
                    .WithErrorCode("MT002");
            });
        }
    }
}
