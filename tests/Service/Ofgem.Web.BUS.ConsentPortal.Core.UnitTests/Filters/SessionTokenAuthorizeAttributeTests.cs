using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Ofgem.Web.BUS.ConsentPortal.Core.Filters;
using Ofgem.Web.BUS.ConsentPortal.Core.Interfaces;
using System;

namespace Ofgem.Web.BUS.ConsentPortal.Core.UnitTests.Filters
{
    [TestFixture]
    public class SessionTokenAuthorizeAttributeTests : FilterTestBase
    {

        private SessionTokenAuthorizeAttribute _systemUnderTest;
        private Mock<ISessionAuthorizationService> _mockSessionAuthorizationService;
        private Mock<ISessionHelper> _mockSessionHelper;
        private const string ValidToken = "valid";
        private const string InvalidToken = "invalid";
        private const string TokenFormQuerystringKey = "sessionId";

        [OneTimeSetUp]
        public void Setup()
        {
            //GenerateSessionToken();

            _mockSessionAuthorizationService = new Mock<ISessionAuthorizationService>();
            _mockSessionHelper = new Mock<ISessionHelper>();
            _mockSessionAuthorizationService.Setup(service => service.ValidateSessionToken(ValidToken)).Returns(true);
            _mockSessionAuthorizationService.Setup(service => service.ValidateSessionToken(InvalidToken)).Returns(false);
            _mockSessionAuthorizationService.Setup(service => service.ValidateSessionToken(string.Empty)).Returns(false);
        }

        [Test]
        public void Contructor_Throws_ArgumentNullException_If_SessionAuthorizationService_Is_Null()
        {
            // Act and assert
            Assert.Throws<ArgumentNullException>(() => { _systemUnderTest = new SessionTokenAuthorizeAttribute(null, _mockSessionHelper.Object); });
        }

        [Test]
        public void Contructor_Throws_ArgumentNullException_If_SessionHelper_Is_Null()
        {
            // Act and assert
            Assert.Throws<ArgumentNullException>(() => { _systemUnderTest = new SessionTokenAuthorizeAttribute(_mockSessionAuthorizationService.Object, null); });
        }

        [Test]
        public void Can_Be_Instantiated_With_Valid_Parameters()
        {
            // Act
            _systemUnderTest = new SessionTokenAuthorizeAttribute(_mockSessionAuthorizationService.Object, _mockSessionHelper.Object);

            // Assert
            Assert.IsNotNull(_systemUnderTest);
        }

        [TestCase(ValidToken)]
        [Test]
        public void OnAuthorization_Accepts_Valid_Session_Token(string tokenValue)
        {
            // Arrange
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(context => context.Response).Returns(new Mock<HttpResponse>().Object);

            _mockSessionHelper.Setup(x => x.Get("SessionId")).Returns(tokenValue);

            var filterContext = GetTestFilterContext(mockHttpContext.Object);
            _systemUnderTest = new SessionTokenAuthorizeAttribute(_mockSessionAuthorizationService.Object, _mockSessionHelper.Object);

            // Act
            _systemUnderTest.OnAuthorization(filterContext);

            // Assert
            Assert.IsNull(filterContext.Result); //Returns Null when happy path is followed successfully
        }

        [TestCase(InvalidToken)]
        [TestCase("")]
        public void OnAuthorization_Denies_Invalid_Session_Token(string tokenValue)
        {
            // Arrange
            var mockHttpContext = new Mock<HttpContext>();

            mockHttpContext.Setup(context => context.Response).Returns(new Mock<HttpResponse>().Object);

            _mockSessionHelper.Setup(x => x.Get("SessionId")).Returns(tokenValue);
            //mockHttpContext.Setup(context => context.Session.GetString("SessionId")).Returns(tokenValue);

            var filterContext = GetTestFilterContext(mockHttpContext.Object);
            _systemUnderTest = new SessionTokenAuthorizeAttribute(_mockSessionAuthorizationService.Object, _mockSessionHelper.Object);

            // Act
            _systemUnderTest.OnAuthorization(filterContext);

            // Assert
            Assert.AreEqual(typeof(RedirectToPageResult), filterContext.Result.GetType());
        }
    }
}
