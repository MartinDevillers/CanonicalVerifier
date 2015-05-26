# Canonical Verifier
Command line-tool that verifies XSD-based Canonical Data Models. The tool can be used as a:
* Manual verification step by an architect or information analyst to check the consistincy of the model
* Post-generation step to provide immediate feedback in tools like Enterprise Architect
* Pre-commit step to block a faulty commit in case the schema's are kept in version control
* Build-step to break a build in case the schema's are used to generate types or contracts

# Command-line interface
The Canonical Verifier expects all XML Schema's to be present in a single directory. The tool will check both the individual schema's and the complete set of schema's for integrity.
```
Devillers.CanonicalVerifier.exe D:/directory-with-XSD/
```

# Sample out
Below is 
```
INFO START: Started Devillers.CanonicalVerifier for directory: D:\GIT\CDM\
INFO Customer.v1.xsd: Started file-level validation
INFO Customer.v1.xsd: This is a domain schema
INFO Customer.v1.xsd: Namespace based on filename: customer:v1
INFO Customer.v1.xsd: Validated 27 elements
INFO Product.v1.xsd: Started file-level validation
INFO Product.v1.xsd: This is a domain schema
INFO Product.v1.xsd: Namespace based on filename: product:v1
INFO Product.v1.xsd: Validated 38 elements
INFO MsgBSCustomer.v1.xsd: Started file-level validation
INFO MsgBSCustomer.v1.xsd: This is a message schema
INFO MsgBSCustomer.v1.xsd: Namespace based on filename: bscustomer:v1
INFO MsgBSCustomer.v1.xsd: Validated 13 elements
INFO MsgISSalesOrder.v1.xsd: Started file-level validation
INFO MsgISSalesOrder.v1.xsd: This is a message schema
INFO MsgISSalesOrder.v1.xsd: Namespace based on filename: issalesorder:v1
INFO MsgPcsOrder.v1.xsd: Started file-level validation
INFO MsgPcsOrder.v1.xsd: This is a message schema
INFO MsgPcsOrder.v1.xsd: Namespace based on filename: pcsorder:v1
ERROR MsgPcsOrder.v1.xsd: 16,3: Type: AddItemToOrderRequest: Expected Request-type to extend RequestBase
ERROR MsgPcsOrder.v1.xsd: 233,3: Type: RemoveItemFromOrderRequest: Expected Request-type to extend RequestBase
INFO MsgPcsOrder.v1.xsd: Validated 50 elements
INFO Order.v1.xsd: Started file-level validation
INFO Order.v1.xsd: This is a domain schema
INFO Order.v1.xsd: Namespace based on filename: order:v1
INFO Order.v1.xsd: Validated 71 elements
INFO Security.v1.xsd: Started file-level validation
INFO Security.v1.xsd: This is a domain schema
INFO Security.v1.xsd: Namespace based on filename: security:v1
INFO Security.v1.xsd: Validated 14 elements
INFO SET: Started set-level validation
INFO SET: Validated set of 18 schemas
INFO STOP: 2 errors detected
```
