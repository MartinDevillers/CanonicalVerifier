using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using FluentValidation.Validators;

namespace Devillers.CanonicalVerifier.Rules
{
    public class XmlSchemaObjectCollectionPropertyRule<TProperty> : PropertyRule where TProperty : XmlSchemaObject
    {
        public XmlSchemaObjectCollectionPropertyRule(MemberInfo member, Func<object, object> propertyFunc, LambdaExpression expression, Func<CascadeMode> cascadeModeThunk, Type typeToValidate, Type containerType) : base(member, propertyFunc, expression, cascadeModeThunk, typeToValidate, containerType)
        {
        }

        protected override IEnumerable<ValidationFailure> InvokePropertyValidator(ValidationContext context, IPropertyValidator validator, string propertyName)
        {
            var propertyContext = new PropertyValidatorContext(context, this, propertyName);
            
            var delegatingValidator = validator as IDelegatingValidator;
            if ((delegatingValidator == null || delegatingValidator.CheckCondition(propertyContext.Instance)))
            {
                var collectionPropertyValue = propertyContext.PropertyValue as XmlSchemaObjectCollection;

                if (collectionPropertyValue != null)
                {
                    return InvokeRecursiveXmlTreeValidator<TProperty>(context, validator, collectionPropertyValue);
                }
            }
            return new List<ValidationFailure>();
        }

        private IEnumerable<ValidationFailure> InvokeRecursiveXmlTreeValidator<T>(ValidationContext context, IPropertyValidator validator, XmlSchemaObjectCollection items)  where T : XmlSchemaObject
        {
            foreach (var item in items)
            {
                var ct = item as XmlSchemaComplexType;
                if (ct != null)
                {
                    var xss = ct.Particle as XmlSchemaSequence;
                    if (xss != null)
                    {
                        var newContext = CloneForChildValidator(context, ct);

                        foreach (var item1 in InvokeRecursiveXmlTreeValidator<T>(newContext, validator, xss.Items))
                        {
                            yield return item1;
                        }
                    }
                    if (ct.ContentModel != null)
                    {
                        var xscce = ct.ContentModel.Content as XmlSchemaComplexContentExtension;
                        if (xscce != null)
                        {
                            var xss2 = xscce.Particle as XmlSchemaSequence;
                            if (xss2 != null)
                            {
                                var newContext = CloneForChildValidator(context, ct);

                                foreach (var item1 in InvokeRecursiveXmlTreeValidator<T>(newContext, validator, xss2.Items))
                                {
                                    yield return item1;
                                }
                            }
                        }
                    }
                }
                var value = item as T;
                if (value != null)
                {
                    var newContext = CloneForChildValidator(context, value);
                    var newPropertyContext = new PropertyValidatorContext(newContext, this, newContext.PropertyChain.ToString(), value);

                    foreach (var result in validator.Validate(newPropertyContext))
                    {
                        yield return result;
                    }
                }
            }
        }

        private static ValidationContext CloneForChildValidator(ValidationContext parent, XmlSchemaObject item)
        {
            var newContext = parent.CloneForChildValidator(parent.InstanceToValidate);
            newContext.PropertyChain.Add(GetObjectName(item));
            return newContext;
        }

        private static string GetObjectName(XmlSchemaObject item)
        {
            var element = item as XmlSchemaElement;
            if (element != null)
            {
                return string.Format("Element[{0}]", element.Name);
            }
            var type = item as XmlSchemaComplexType;
            if (type != null)
            {
                return string.Format("ComplexType[{0}]", type.Name);
            }
            return null;
        }
    }
}
