[![Build Status](https://travis-ci.org/MartinDevillers/CanonicalVerifier.svg)](https://travis-ci.org/MartinDevillers/CanonicalVerifier)
[![Build status](https://ci.appveyor.com/api/projects/status/voj8ownp1wvo1s37?svg=true)](https://ci.appveyor.com/project/MartinDevillers/canonicalverifier)

# Canonical Verifier
The Canonical Verifier is a Command line-tool that verifies XSD-based [Canonical schemas](http://soapatterns.org/design_patterns/canonical_schema). This tool helps enterprise architects, information analysts and software developers to maintain a clean, interopable and consistent model.

The tool can be used as a:
* Manual verification step by an architect or information analyst to check the consistincy of the model
* Post-generation step to provide immediate feedback in tools like Enterprise Architect
* Pre-commit step to block a faulty commit in case the schemas are kept in version control
* Build-step to break a build in case the schemas are used to generate types or contracts

# A Canonical What?
A Canonical Schema lies at the heart of contract-first webservice design in a service oriented architecture. 

![Canonical Schema](http://soapatterns.org/static/images/figures/canonical_schema/fig1.png)

A Canonical Schema consists of two parts:
* One or more message schemas. Each message schema represents a single webservice and consists of one or more request-response message tuples.
* Zero or more domain schemas. Each domain schema represents a logical domain (Customer; Order; etc) and consists of one or more complex types describing types of that domain (Invoice; BankAccount; OrderHistory). Domain schemas are used by message schemas to exchange business documents. Sharing one domain schema across multiple message schemas reduces interfacefriction, promotes the reusability of conceptual models and improves performance caused by avoiding unnecesary type mapping roundtrips.

# Usage
The Canonical Verifier expects all XML schemas to be present in a single directory. The tool will check both the individual schemas and the complete set of schemas for integrity.
```
Devillers.CanonicalVerifier.exe D:/directory-with-XSD/
```
# Configuration
The file [`Devillers.CanonicalVerifier.config`](../blob/master/Devillers.CanonicalVerifier/App.config) contains the configuration of the Canonical Verifier. [Click here](../blob/master/Devillers.CanonicalVerifier/App.config) to see the default configuration file with per setting comment.

# Rules
The Canonical Verifier checks the model using a set of rules. These rules are listed in the table below. Individual rules can be enabled or disabled through the *.config file.

| Error Code | Rule                                                                                         |
|------------|----------------------------------------------------------------------------------------------|
| XML001     | The XML document must be well formed                                                         |
| XML002     | The XML document must be readable by .NET                                                    |
| XSD001     | The set of XSD schemas must valid                                                           |
| XSD002     | The XSD schema must be valid                                                                 |
| CT001      | A complex type should not have a block specifier                                             |
| CT002      | A complex type should not have a final specifier                                             |
| CT003      | A complex type should not be mixed                                                           |
| CT004      | Abstract types should have a name ending with 'Base'                                         |
| CT005      | The name of a complex type should always be Pascal Case                                      |
| EL001      | "Primitive type '{0}' is not supported                                                       |
| EL002      | An element must be either a primitive type or a known schema type.                           |
| EL003      | An element should not have a block specifier                                                 |
| EL004      | An element should not have a default value                                                   |
| EL005      | An element should not have a final specifier                                                 |
| EL006      | An element should not have a fixed value                                                     |
| EL007      | An element should have no form specifier                                                     |
| EL008      | An element should have no id value                                                           |
| EL009      | An element should have no ref value                                                          |
| EL010      | An element with minOccurs=0 should have nillable=true                                        |
| EL011      | An element with minOccurs!=0 should have nillable=false                                      |
| EL012      | Only maxOccurs=0, maxOccurs=1 or maxOccurs=unbounded are allowed on an element               |
| EL013      | The name of an element should always be Pascal Case                                          |
| MT001      | Expected message type to extend a base-type                                                  |
| MT002      | Expected message type to extend {0}Base                                                      |
| SD001      | Schema should have no dependencies on other schemas.                                        |
| DP001      | Schema should have an element for each complex type at the top level (Garden of Eden style). |
| DP002      | Schema should only have complex types at the top level (*NO* Garden of Eden style).          |
| RN001      | Root targetNamespace does not start with expected prefix. Expected: {0}, Actual: {1}         |
| RN002      | "Root targetNamespace does not end with expected postfix. Expected: {0}, Actual: {1}         |

# Sample output
The Canonical Verifier writes any errors to the standard output stream. The verbosity can be increased to provide more insight into the validation process.
```
INFO Started Devillers.CanonicalVerifier for directory: \Data\TestSet3
INFO Parsed 1 schemas
ERROR DP002:S[0].S.XmlSchema.Items: Schema should only have complex types at the top level (*NO* Garden of Eden style). (actual: System.Xml.Schema.XmlSchemaObjectCollection)
ERROR EL003:S[0].S.ComplexType[PrimitiveTypes].Element[String].@.Block: An element should not have a block specifier (actual: Extension)
ERROR EL007:S[0].S.ComplexType[PrimitiveTypes].Element[Boolean].@.Form: An element should have no form specifier (actual: Unqualified)
ERROR EL006:S[0].S.ComplexType[PrimitiveTypes].Element[Integer].@.FixedValue: An element should not have a fixed value (actual: 32)
ERROR EL008:S[0].S.ComplexType[PrimitiveTypes].Element[Double].@.Id: An element should have no id value (actual: test)
ERROR EL004:S[0].S.ComplexType[PrimitiveTypes].Element[NotherDouble].@.DefaultValue: An element should not have a default value (actual: 20)
ERROR EL002:S[0].S.ComplexType[PrimitiveTypes].Element[].@.SchemaTypeName.Namespace: An element must be either a primitive type or a known schema type. (actual: )
ERROR EL009:S[0].S.ComplexType[PrimitiveTypes].Element[].@.RefName.IsEmpty: An element should have no ref value (actual: False)
ERROR EL010:S[0].S.ComplexType[PrimitiveTypes].Element[Long].@.IsNillable: An element with minOccurs=0 should have nillable=true (actual: False)
ERROR EL011:S[0].S.ComplexType[PrimitiveTypes].Element[Date].@.IsNillable: An element with minOccurs!=0 should have nillable=false (actual: True)
ERROR EL012:S[0].S.ComplexType[PrimitiveTypes].Element[DateTime].@.MaxOccursString: Only maxOccurs=0, maxOccurs=1 or maxOccurs=unbounded are allowed on an element (actual: 2)
ERROR EL001:S[0].S.ComplexType[PrimitiveTypes].Element[PositiveInteger].@.SchemaTypeName.Name: Primitive type '{0}' is not supported (actual: positiveInteger)
INFO Found 12 errors and 0 warnings
```
