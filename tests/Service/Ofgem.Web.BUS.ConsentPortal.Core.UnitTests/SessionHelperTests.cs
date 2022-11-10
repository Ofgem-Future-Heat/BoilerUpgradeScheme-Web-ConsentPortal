using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using System;
using System.Text;

namespace Ofgem.Web.BUS.ConsentPortal.Core.UnitTests
{
    [TestFixture]
    public class SessionHelperTests
    {
        private SessionHelper _systemUnderTest;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private string consentId = (Guid.NewGuid()).ToString();

        [OneTimeSetUp]
        public void Setup()
        {
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        }

        [Test]
        public void Contructor_Throws_ArgumentNullException_If_SessionHelper_Is_Null()
        {
            // Act and assert
            Assert.Throws<ArgumentNullException>(() => { _systemUnderTest = new SessionHelper(null); });
        }

        [Test]
        public void Can_Be_Instantiated_With_Valid_Parameters()
        {
            // Act
            _systemUnderTest = new SessionHelper(_mockHttpContextAccessor.Object);

            // Assert
            Assert.IsNotNull(_systemUnderTest);
        }

        [Test]
        public void Sets_Http_Session_Variable()
        {
            // Arrange & Act
            Mock<ISession> mockSession = new Mock<ISession>();
            string key = "ConsentId";
            byte[] value = null;

            mockSession.Setup(_ => _.Set(key, It.IsAny<byte[]>()))
                   .Callback<string, byte[]>((k, v) => value = v);
            //mockSession.Setup(_ => _.TryGetValue(key, out value)).Returns(true);

            _mockHttpContextAccessor.Setup(s => s.HttpContext.Session).Returns(mockSession.Object);
            _systemUnderTest = new SessionHelper(_mockHttpContextAccessor.Object);

            // Act
            _systemUnderTest.Add("ConsentId", consentId);
            var result = Encoding.UTF8.GetString(value);

            // Assert
            Assert.AreEqual(result, consentId);
        }


        [Test]
        public void Returns_Null_When_Http_Session_Variable_When_Key_Doesnt_exists()
        {
            // Arrange
            Mock<ISession> mockSession = new Mock<ISession>();

            _mockHttpContextAccessor.Setup(s => s.HttpContext.Session).Returns(mockSession.Object);
            _systemUnderTest = new SessionHelper(_mockHttpContextAccessor.Object);

            // Act
            var testGetVariable = _systemUnderTest.Get("ConsentId");

            // Assert
            Assert.IsNull(testGetVariable);
        }

        [Test]
        public void Gets_Http_Session_Variable_When_Key_exists()
        {
            // Arrange
            
            Mock<ISession> mockSession = new Mock<ISession>();
            byte[] dummy = Encoding.UTF8.GetBytes(consentId);

            mockSession.Setup(_ => _.TryGetValue(It.IsAny<string>(), out dummy));

            _mockHttpContextAccessor.Setup(s => s.HttpContext.Session).Returns(mockSession.Object);
            _systemUnderTest = new SessionHelper(_mockHttpContextAccessor.Object);

            // Act
            var testGetVariable = _systemUnderTest.Get("ConsentId");

            // Assert
            Assert.IsNotEmpty(testGetVariable);
            Assert.AreEqual(testGetVariable, consentId);
        }

    }
}
