using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Ofgem.API.BUS.PropertyConsents.Domain.Models.CommsObjects;
using Ofgem.Web.BUS.ConsentPortal.Core.Filters;
using Ofgem.Web.BUS.ConsentPortal.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace Ofgem.Web.BUS.ConsentPortal.Core.UnitTests.Filters
{
    [TestFixture]
    public class EmailTokenAuthorizeAttributeTests : FilterTestBase
    {
        private EmailTokenAuthorizeAttribute _systemUnderTest;
        private Mock<IOwnerConsentService> _mockOwnerConsentService;

        private const string ValidToken = "valid";
        private const string InvalidToken = "invalid";
        private const string NullToken = "null";
        private const string TokenQuerystringKey = "token";

        [OneTimeSetUp]
        public void Setup()
        {
            var validTokenResult = new TokenVerificationResult { TokenAccepted = true };
            var invalidTokenResult = new TokenVerificationResult { TokenAccepted = false };
            var nullTokenResult = (TokenVerificationResult)null!;

            _mockOwnerConsentService = new Mock<IOwnerConsentService>(MockBehavior.Strict);
            _mockOwnerConsentService.Setup(service => service.ValidateEmailToken(ValidToken).Result).Returns(validTokenResult);
            _mockOwnerConsentService.Setup(service => service.ValidateEmailToken(InvalidToken).Result).Returns(invalidTokenResult);
            _mockOwnerConsentService.Setup(service => service.ValidateEmailToken(string.Empty).Result).Returns(invalidTokenResult);
            _mockOwnerConsentService.Setup(service => service.ValidateEmailToken(NullToken).Result).Returns(nullTokenResult);
        }

        [Test]
        public void Contructor_Throws_ArgumentNullException_If_OwnerConsentService_Is_Null()
        {
            // Act and assert
            Assert.Throws<ArgumentNullException>(() => { _systemUnderTest = new EmailTokenAuthorizeAttribute(null); });
        }

        [Test]
        public void Can_Be_Instantiated_With_Valid_Parameters()
        {
            // Act
            _systemUnderTest = new EmailTokenAuthorizeAttribute(_mockOwnerConsentService.Object);

            // Assert
            Assert.IsNotNull(_systemUnderTest);
        }

        [TestCase(ValidToken)]
        public async Task OnAuthorizationAsync_Accepts_Valid_Token(string tokenValue)
        {
            // Arrange
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(context => context.Request.Query[TokenQuerystringKey]).Returns(tokenValue);
            mockHttpContext.Setup(context => context.Response).Returns(new Mock<HttpResponse>().Object);

            var filterContext = GetTestFilterContext(mockHttpContext.Object);
            _systemUnderTest = new EmailTokenAuthorizeAttribute(_mockOwnerConsentService.Object);

            // Act
            await _systemUnderTest.OnAuthorizationAsync(filterContext);

            // Assert
            Assert.IsNull(filterContext.Result); //Returns Null when happy path is followed successfully
        }
        
        [TestCase(InvalidToken)]
        [TestCase("")]
        [TestCase(null!)]
        public async Task OnAuthorizationAsync_Denies_Invalid_Token(string tokenValue)
        {
            //Arrange
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(context => context.Request.Query[TokenQuerystringKey]).Returns(tokenValue);
            mockHttpContext.Setup(context => context.Response).Returns(new Mock<HttpResponse>().Object);

            var filterContext = GetTestFilterContext(mockHttpContext.Object);
            _systemUnderTest = new EmailTokenAuthorizeAttribute(_mockOwnerConsentService.Object);

            // Act
            await _systemUnderTest.OnAuthorizationAsync(filterContext);

            // Assert
            Assert.AreEqual(typeof(RedirectToPageResult), filterContext.Result.GetType());
        }

        [Test]
        public async Task OnAuthorizationAsync_Null_Token()
        {
            //Arrange
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(context => context.Request.Query[TokenQuerystringKey]).Returns(NullToken);
            mockHttpContext.Setup(context => context.Response).Returns(new Mock<HttpResponse>().Object);

            _mockOwnerConsentService.Setup(x => x.ValidateEmailToken(NullToken)).ReturnsAsync((TokenVerificationResult)null!);

            var filterContext = GetTestFilterContext(mockHttpContext.Object);
            _systemUnderTest = new EmailTokenAuthorizeAttribute(_mockOwnerConsentService.Object);

            // Act
            await _systemUnderTest.OnAuthorizationAsync(filterContext);

            // Assert
            Assert.AreEqual(typeof(RedirectToPageResult), filterContext.Result.GetType());
        }
    }
}
