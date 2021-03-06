﻿using System.Xml.Schema;
using FluentValidation;

namespace Devillers.CanonicalVerifier.Rules
{
    public class XmlSchemaDependencyValidator : AbstractValidator<XmlSchema>
    {
        public XmlSchemaDependencyValidator(string excludingNamespacePrefix)
        {
            RuleFor(x => x.TargetNamespace)
                .Must(x => !x.StartsWith(excludingNamespacePrefix))
                .WithMessage("Schema should have no dependencies on other schema's.")
                .WithErrorCode("SD001");

        }
    }
}
