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
        [DeploymentItem("TestSet1")]
        public void Given_TestSet1_Should_BeValid()
        {
            var validator = new CanonicalSchemaSetValidator();

            var schemaSet = new CanonicalSchemaSet(".");

            var result = validator.Validate(schemaSet);

            Assert.IsTrue(result.IsValid, string.Join("; ", result.Errors.Select(x => x.ErrorMessage)));
        }
    }
}
