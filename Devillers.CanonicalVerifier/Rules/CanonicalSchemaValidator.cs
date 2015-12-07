using System;
using System.Linq.Expressions;
using System.Xml.Schema;
using FluentValidation;
using FluentValidation.Internal;

namespace Devillers.CanonicalVerifier.Rules
{
    public class CanonicalSchemaValidator : AbstractValidator<CanonicalSchema>
    {
        public CanonicalSchemaValidator()
        {
            RuleFor(x => x.XmlException)
                .Empty()
                .WithMessage("{0},{1}: {2}", (x, y) => y.LineNumber, (x, y) => y.LinePosition, (x, y) => y.Message)
                .WithErrorCode("XML001");

            RuleForEach(x => x.XmlSchemaValidationErrors)
                .Empty()
                .WithMessage("{0}: {1}", (x, y) => y.Severity, (x, y) => y.Message)
                .WithErrorCode("XSD002");

            RuleFor(x => x.XmlSchema)
                .NotEmpty()
                .WithErrorCode("XML002");
                ;

            When(XmlSchemaPresent, () =>
            {
                RuleFor(x => x.XmlSchema)
                    .SetValidator(
                        x => new XmlSchemaRootNamespaceValidator(Settings.MessageNamespacePrefix, x.NamespaceSuffix))
                    .When(x => x.IsMessageSchema);

                RuleForEach<XmlSchema>(x => x.XmlSchema.Includes)
                    .SetValidator(new XmlSchemaDependencyValidator(Settings.MessageNamespacePrefix))
                    .When(x => x.IsMessageSchema);

                RuleFor(x => x.XmlSchema)
                    .SetValidator(new XmlSchemaDesignPatternValidator(true))
                    .When(x => x.IsMessageSchema);

                RuleForEach<XmlSchemaComplexType>(x => x.XmlSchema.Items)
                    .SetValidator(new XmlSchemaMessageTypeValidator("Request"))
                    .When(x => x.IsMessageSchema)
                    .OverridePropertyName("@");

                RuleForEach<XmlSchemaComplexType>(x => x.XmlSchema.Items)
                    .SetValidator(new XmlSchemaMessageTypeValidator("Response"))
                    .When(x => x.IsMessageSchema)
                    .OverridePropertyName("@");

                RuleFor(x => x.XmlSchema)
                    .SetValidator(x => new XmlSchemaRootNamespaceValidator(Settings.DomainNamespacePrefix, x.NamespaceSuffix))
                    .Unless(x => x.IsMessageSchema);

                RuleForEach<XmlSchema>(x => x.XmlSchema.Includes)
                    .SetValidator(new XmlSchemaDependencyValidator(string.Empty))
                    .Unless(x => x.IsMessageSchema);

                RuleFor(x => x.XmlSchema)
                    .SetValidator(new XmlSchemaDesignPatternValidator(false))
                    .Unless(x => x.IsMessageSchema);

                RuleForEach<XmlSchemaElement>(x => x.XmlSchema.Items)
                    .SetValidator(new XmlSchemaElementValidator(Settings.DomainNamespacePrefix))
                    .OverridePropertyName("@");

                RuleForEach<XmlSchemaComplexType>(x => x.XmlSchema.Items)
                    .SetValidator(new XmlSchemaComplexTypeValidator())
                    .OverridePropertyName("@");
            });
        }

        public bool XmlSchemaPresent(CanonicalSchema schema)
        {
            return schema.XmlSchema != null;
        }

        public IRuleBuilderInitial<CanonicalSchema, TXmlSchemaObject> RuleForEach<TXmlSchemaObject>(Expression<Func<CanonicalSchema, XmlSchemaObjectCollection>> expression) where TXmlSchemaObject : XmlSchemaObject
        {
            var member = expression.GetMember();
            var compiled = expression.Compile();
            var rule = new XmlSchemaObjectCollectionPropertyRule<TXmlSchemaObject>(member, compiled.CoerceToNonGeneric(), expression, () => CascadeMode, typeof(TXmlSchemaObject), typeof(XmlSchema));
            AddRule(rule);
            return new RuleBuilder<CanonicalSchema, TXmlSchemaObject>(rule);
        } 
    }
}
