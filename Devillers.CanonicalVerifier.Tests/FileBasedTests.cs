using System;
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

            var expectedErrors = new[] { "EL001", "EL002", "EL003", "EL004", /*"EL005",*/ "EL006", "EL007", "EL008", "EL009", "EL010", "EL011", "EL012", "EL013" };

            CollectionAssert.AreEquivalent(expectedErrors, result.Errors.Select(x => x.ErrorCode).ToList());
        }
    }
}
