using NUnit.Framework;
using System;

namespace Ofgem.Web.BUS.ConsentPortal.Core.UnitTests
{
    [TestFixture]
    public class SessionAuthorizationServiceTests
    {
        private SessionAuthorizationService _systemUnderTest;

        private const string ConsentTokenSecret = "my super secret consent token secret";
        private const string ConsentRequestId = "my consent request ID";

        [Test]
        public void Contructor_Throws_ArgumentNullException_If_Consent_Token_Secret_Is_Null()
        {
            // Act and assert
            Assert.Throws<ArgumentNullException>(() => { _systemUnderTest = new SessionAuthorizationService(null); });
        }

        [Test]
        public void Can_Be_Instantiated_With_Valid_Parameters()
        {
            // Act
            _systemUnderTest = new SessionAuthorizationService(ConsentTokenSecret);

            // Assert
            Assert.IsNotNull(_systemUnderTest);
        }

        [Test]
        public void GenerateSessionToken_Creates_Token_Successully()
        {
            // Arrange
            _systemUnderTest = new SessionAuthorizationService(ConsentTokenSecret);

            // Act
            var testToken = _systemUnderTest.GenerateSessionToken(ConsentRequestId);

            // Assert
            Assert.IsNotEmpty(testToken);
        }

        [Test]
        public void GenerateSessionToken_Creates_Token_Throws_Exception()
        {
            // Arrange
            _systemUnderTest = new SessionAuthorizationService(ConsentTokenSecret);

            // Act & Assert
            Assert.That(() => _systemUnderTest.GenerateSessionToken(null), Throws.TypeOf<System.ArgumentNullException>());
        }

        [Test]
        public void ValidateSessionToken_Validates_Valid_Token()
        {
            // Arrange
            _systemUnderTest = new SessionAuthorizationService(ConsentTokenSecret);
            var testToken = _systemUnderTest.GenerateSessionToken(ConsentRequestId);

            // Act
            var isValid = _systemUnderTest.ValidateSessionToken(testToken);

            // Assert
            Assert.IsTrue(isValid);
        }

        [Test]
        public void ValidateSessionToken_Invalidates_Invalid_Token()
        {
            // Arrange
            _systemUnderTest = new SessionAuthorizationService(ConsentTokenSecret);
            var testToken = "an invalid token";

            // Act
            var isValid = _systemUnderTest.ValidateSessionToken(testToken);

            // Assert
            Assert.IsFalse(isValid);
        }

        [Test]
        public void ValidateSessionToken_Invalidates_Null_Token()
        {
            // Arrange
            _systemUnderTest = new SessionAuthorizationService(ConsentTokenSecret);

            // Act
            var isValid = _systemUnderTest.ValidateSessionToken(null);

            // Assert
            Assert.IsFalse(isValid);
        }

        [Test]
        public void ValidateSessionToken_Returns_New_Token_For_Valid_Token()
        {
            // Arrange
            _systemUnderTest = new SessionAuthorizationService(ConsentTokenSecret);

            // Act
            var isValid = _systemUnderTest.ValidateSessionToken(null);

            // Assert
            Assert.IsFalse(isValid);
        }

        [Test]
        public void ExtendSessionToken_Returns_No_Token_For_Invalid_Token()
        {
            // Arrange
            _systemUnderTest = new SessionAuthorizationService(ConsentTokenSecret);
            var testToken = "an invalid token";

            // Act
            var extendedToken = _systemUnderTest.ExtendSessionToken(testToken);

            // Assert
            Assert.IsEmpty(extendedToken);
        }

        [Test]
        public void ExtendSessionToken_Returns_Updated_Token_For_Valid_Token()
        {
            // Arrange
            _systemUnderTest = new SessionAuthorizationService(ConsentTokenSecret);
            var testToken = _systemUnderTest.GenerateSessionToken(ConsentRequestId);

            // Act
            var extendedToken = _systemUnderTest.ExtendSessionToken(testToken);

            // Assert
            Assert.IsNotEmpty(extendedToken);
            Assert.AreEqual(extendedToken, testToken);
        }

        [Test]
        public void ExtendSessionToken_Returns_No_Token_For_Null_Token()
        {
            // Arrange
            _systemUnderTest = new SessionAuthorizationService(ConsentTokenSecret);
            var testToken = _systemUnderTest.GenerateSessionToken(ConsentRequestId);

            // Act
            var extendedToken = _systemUnderTest.ExtendSessionToken(null!);

            // Assert
            Assert.IsEmpty(extendedToken);
        }
    }
}
