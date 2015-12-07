using System.Linq;
using Devillers.CanonicalVerifier.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Devillers.CanonicalVerifier.Tests
{
    [TestClass]
    public class FileBasedTests
    {
        [TestMethod]
        [DeploymentItem("TestSet1", "TestSet1")]
        public void Given_TestSet1_Should_BeValid()
        {
            var validator = new CanonicalSchemaSetValidator();

            var schemaSet = new CanonicalSchemaSet("TestSet1");

            var result = validator.Validate(schemaSet);

            Assert.IsTrue(result.IsValid, string.Join("; ", result.Errors.Select(x => x.ErrorMessage)));
        }

        [TestMethod]
        [DeploymentItem("TestSet2", "TestSet2")]
        public void Given_TestSet2_Should_TriggerXmlSchemaComplexTypeValidator()
        {
            var validator = new CanonicalSchemaSetValidator();

            var schemaSet = new CanonicalSchemaSet("TestSet2");

            var result = validator.Validate(schemaSet);

            var expectedErrors = new[] {"CT001", "CT002", "CT003", "CT004", "CT005"};

            CollectionAssert.AreEquivalent(expectedErrors, result.Errors.Select(x => x.ErrorCode).ToList());
        }

        [TestMethod]
        [DeploymentItem("TestSet3", "TestSet3")]
        public void Given_TestSet3_Should_TriggerXmlSchemaElementValidator()
        {
            var validator = new CanonicalSchemaSetValidator();

            var schemaSet = new CanonicalSchemaSet("TestSet3");

            var result = validator.Validate(schemaSet);

            var expectedErrors = new[] { "EL001", "EL002", "EL003", "EL004", /*"EL005",*/ "EL006", "EL007", "EL008", "EL009", "EL010", "EL011", "EL012", "EL013", "DP002" };

            CollectionAssert.AreEquivalent(expectedErrors, result.Errors.Select(x => x.ErrorCode).ToList());
        }

        [TestMethod]
        [DeploymentItem("TestSet4", "TestSet4")]
        public void Given_TestSet4_Should_TriggerCanonicalSchemaValidator()
        {
            var validator = new CanonicalSchemaSetValidator();

            var schemaSet = new CanonicalSchemaSet("TestSet4");

            var result = validator.Validate(schemaSet);

            var expectedErrors = new[] { "XML001", "XML002", "XSD001" };

            CollectionAssert.AreEquivalent(expectedErrors, result.Errors.Select(x => x.ErrorCode).ToList());
        }

        [TestMethod]
        [DeploymentItem("TestSet5", "TestSet5")]
        public void Given_TestSet5_Should_TriggerXmlSchemaDesignPatternValidator()
        {
            var validator = new CanonicalSchemaSetValidator();

            var schemaSet = new CanonicalSchemaSet("TestSet5");

            var result = validator.Validate(schemaSet);

            var expectedErrors = new[] { "DP001", "DP002" };

            CollectionAssert.AreEquivalent(expectedErrors, result.Errors.Select(x => x.ErrorCode).ToList());
        }
    }
}
