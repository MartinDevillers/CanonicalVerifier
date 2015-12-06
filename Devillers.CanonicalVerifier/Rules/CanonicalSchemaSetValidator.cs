using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using FluentValidation;
using FluentValidation.Internal;

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
