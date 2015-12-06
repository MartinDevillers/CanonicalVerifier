using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using FluentValidation;

namespace Devillers.CanonicalVerifier.Rules
{
    public class XmlSchemaElementValidator : AbstractValidator<XmlSchemaElement>
    {
        public XmlSchemaElementValidator(string domainPrefix)
        {
            RuleFor(x => x.SchemaTypeName.Name)
                .Matches("string|boolean|int|double|decimal|long|date|dateTime|base64Binary")
                .When(x => x.SchemaTypeName.Namespace == XmlSchema.Namespace)
                .WithMessage("Primitive type '{0}' is not supported")
                .WithErrorCode("EL001");

            RuleFor(x => x.SchemaTypeName.Namespace)
                .Must(x => x.StartsWith(domainPrefix))
                .When(x => x.SchemaTypeName.Namespace != XmlSchema.Namespace)
                .WithMessage("An element must be either a primitive type or a known schema type.")
                .WithErrorCode("EL002");

            RuleFor(x => x.Block)
                .Equal(XmlSchemaDerivationMethod.None)
                .WithMessage("An element should not have a block specifier")
                .WithErrorCode("EL003");

            RuleFor(x => x.DefaultValue)
                .Null()
                .WithMessage("An element should not have a default value")
                .WithErrorCode("EL004");

            RuleFor(x => x.Final)
                .Equal(XmlSchemaDerivationMethod.None)
                .WithMessage("An element should not have a final specifier")
                .WithErrorCode("EL005");

            RuleFor(x => x.FixedValue)
                .Null()
                .WithMessage("An element should not have a fixed value")
                .WithErrorCode("EL006");

            RuleFor(x => x.Form)
                .Equal(XmlSchemaForm.None)
                .WithMessage("An element should have no form specifier")
                .WithErrorCode("EL007");

            RuleFor(x => x.Id)
                .Null()
                .WithMessage("An element should have no id value")
                .WithErrorCode("EL008");

            RuleFor(x => x.RefName.IsEmpty)
                .NotEmpty()
                .WithMessage("An element should have no ref value")
                .WithErrorCode("EL009");

            RuleFor(x => x.IsNillable)
                .NotEmpty()
                .When(x => x.MinOccurs == 0)
                .WithMessage("An element with minOccurs=0 should have nillable=true")
                .WithErrorCode("EL010");

            RuleFor(x => x.IsNillable)
                .Equal(false)
                .When(x => x.MinOccurs != 0)
                .WithMessage("An element with minOccurs!=0 should have nillable=false")
                .WithErrorCode("EL011");

            RuleFor(x => x.MaxOccursString)
                .Matches("0|1|unbounded")
                .WithMessage("Only maxOccurs=0, maxOccurs=1 or maxOccurs=unbounded are allowed on an element")
                .WithErrorCode("EL012");

            RuleFor(x => x.Name)
                .Matches("^[A-Z][a-z]+(?:[A-Z][a-z]+)*$")
                .WithMessage("The name of an element should always be Pascal Case")
                .WithErrorCode("EL013");
        }
    }
}
