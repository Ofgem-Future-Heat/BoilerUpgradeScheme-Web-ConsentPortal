using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using NUnit.Framework;
using Ofgem.API.BUS.PropertyConsents.Domain.Models.CommsObjects;
using Ofgem.Web.BUS.ConsentPortal.Core.Interfaces;
using Ofgem.Web.BUS.ConsentPortal.WebApp.Pages.Consent;
using System;
using System.Threading.Tasks;

namespace Ofgem.Web.BUS.ConsentPortal.WebApp.UnitTests.Pages.Consent
{
    [TestFixture]
    public class DetailsModelTests
    {
        private DetailsModel _systemUnderTest;
        private Mock<IOwnerConsentService> _mockOwnerConsentService = new();
        private Mock<ISessionAuthorizationService> _mockSessionAuthorizationService = new();
        private Mock<ISessionHelper> _mockSessionHelper = new();
        private const string ValidToken = "valid";

        private TokenVerificationResult _validTokenResult = new TokenVerificationResult { TokenAccepted = true, ConsentRequestId = new Guid() };

        private ConsentRequestSummary _consentRequestSummary = new()
        {
            ApplicationReferenceNumber = "APW12345678",
            InstallerName = "APW Enterprises",
            TechnologyType = "Air source heat pump",
            InstallationAddressLine1 = "1 Canada Square",
            InstallationAddressLine2 = "Canary Wharf",
            InstallationAddressLine3 = "London",
            InstallationAddressCounty = "",
            InstallationAddressPostcode = "E14 5AA",
            ServiceLevelAgreementDate = DateTime.Now.AddYears(1),
            ExpiryDate = DateTime.Now.AddDays(14),
            QuoteAmount = 11405
        };

        [Test]
        public async Task OnGet_RedirectTo_SessionExpired_If_Token_Invalid()
        {
            //Arrange
            var invalidTokenResult = new TokenVerificationResult { TokenAccepted = false, ConsentRequestId = null, TokenExpiryDate = null };
            string invalidToken = "Invalid.Token";

            _mockOwnerConsentService.Setup(service => service.ValidateEmailToken(ValidToken)).ReturnsAsync(invalidTokenResult);
            _systemUnderTest = new DetailsModel(_mockOwnerConsentService.Object, _mockSessionAuthorizationService.Object, _mockSessionHelper.Object);

            //Act
            var redirect = await _systemUnderTest.OnGet(invalidToken) as RedirectToPageResult;

            //Assert
            Assert.IsNotNull(redirect);
            Assert.AreEqual("./SessionExpired", redirect.PageName);
        }

        [Test]
        public async Task OnGet_RedirectTo_AlreadyGiven_If_Token_Is_Valid_but_Consent_Already_Given()
        {
            //Arrange
            _validTokenResult.TokenExpiryDate = DateTime.UtcNow.AddDays(1);
            _consentRequestSummary.HasConsented = DateTime.UtcNow.AddDays(-1);

            _mockOwnerConsentService.Setup(service => service.ValidateEmailToken(ValidToken)).ReturnsAsync(_validTokenResult);
            _mockOwnerConsentService.Setup(service => service.GetConsentRequestSummary(_validTokenResult.ConsentRequestId.Value)).ReturnsAsync(_consentRequestSummary);

            _mockSessionHelper.Setup(x => x.Get("ConsentId")).Returns(_validTokenResult.ConsentRequestId.Value.ToString());

            _systemUnderTest = new DetailsModel(_mockOwnerConsentService.Object, _mockSessionAuthorizationService.Object, _mockSessionHelper.Object);

            //Act
            var redirect = await _systemUnderTest.OnGet(ValidToken) as RedirectToPageResult;

            //Assert
            Assert.IsNotNull(redirect);
            Assert.AreEqual("./AlreadyGiven", redirect.PageName);
        }

        [Test]
        public async Task OnGet_RedirectTo_LinkExpired_If_Token_Is_Valid_but_Link_Expired()
        {
            //Arrange
            _consentRequestSummary.HasConsented = null;
            _validTokenResult.TokenExpiryDate = DateTime.UtcNow.AddDays(-1);
            _consentRequestSummary.ExpiryDate = DateTime.UtcNow.AddDays(-1);

            _mockOwnerConsentService.Setup(service => service.ValidateEmailToken(ValidToken)).ReturnsAsync(_validTokenResult);
            _mockOwnerConsentService.Setup(service => service.GetConsentRequestSummary(_validTokenResult.ConsentRequestId.Value)).ReturnsAsync(_consentRequestSummary);

            _mockSessionHelper.Setup(x => x.Get("ConsentId")).Returns(_validTokenResult.ConsentRequestId.Value.ToString());

            _systemUnderTest = new DetailsModel(_mockOwnerConsentService.Object, _mockSessionAuthorizationService.Object, _mockSessionHelper.Object);

            //Act
            var redirect = await _systemUnderTest.OnGet(ValidToken) as RedirectToPageResult;

            //Assert
            Assert.IsNotNull(redirect);
            Assert.AreEqual("./LinkExpired", redirect.PageName);
        }

        [Test]
        public async Task OnGet_ReturnPage_If_Token_Is_Valid_And_Active_Plus_Consent_Not_Given()
        {
            //Arrange
            _consentRequestSummary.HasConsented = null;
            _validTokenResult.TokenExpiryDate = DateTime.UtcNow.AddDays(1);
            _consentRequestSummary.ExpiryDate = DateTime.UtcNow.AddDays(1);


            _mockOwnerConsentService.Setup(service => service.ValidateEmailToken(ValidToken)).ReturnsAsync(_validTokenResult);
            _mockOwnerConsentService.Setup(service => service.GetConsentRequestSummary(_validTokenResult.ConsentRequestId.Value)).ReturnsAsync(_consentRequestSummary);

            _mockSessionHelper.Setup(x => x.Get("ConsentId")).Returns(_validTokenResult.ConsentRequestId.Value.ToString());

            _systemUnderTest = new DetailsModel(_mockOwnerConsentService.Object, _mockSessionAuthorizationService.Object, _mockSessionHelper.Object);

            //Act
            var page = await _systemUnderTest.OnGet(ValidToken) as PageResult;

            //Assert
            Assert.IsNotNull(page);
        }

        [Test]
        public async Task OnPost_NotConsented_ReturnPageResult()
        {
            //Arrange
            _consentRequestSummary.HasConsented = null;
            _validTokenResult.TokenExpiryDate = DateTime.UtcNow.AddDays(1);
            _consentRequestSummary.ExpiryDate = DateTime.UtcNow.AddDays(1);

            _mockOwnerConsentService.Setup(service => service.ValidateEmailToken(ValidToken)).ReturnsAsync(_validTokenResult);
            _mockOwnerConsentService.Setup(service => service.GetConsentRequestSummary(_validTokenResult.ConsentRequestId.Value)).ReturnsAsync(_consentRequestSummary);
            _mockSessionHelper.Setup(x => x.Get("Token")).Returns(ValidToken);

            _systemUnderTest = new DetailsModel(_mockOwnerConsentService.Object, _mockSessionAuthorizationService.Object, _mockSessionHelper.Object);

            //Act
            var redirect = await _systemUnderTest.OnPost() as RedirectToPageResult;

            //Assert
            Assert.IsNull(redirect);
            Assert.AreEqual(_systemUnderTest.DisplayError, true);
        }

        //Test for making sure model returned is correct

        [Test]
        public async Task OnPost_HasConsented_And_ConfirmedName_ReturnRedirectToPageResult()
        {
            //Arrange
            _systemUnderTest = new DetailsModel(_mockOwnerConsentService.Object, _mockSessionAuthorizationService.Object, _mockSessionHelper.Object);

            _systemUnderTest.ConsentAgreed = true;
            _systemUnderTest.UserConfirmsIsNamedOwner = true;

            //Act
            var redirect = await _systemUnderTest.OnPost() as RedirectToPageResult;

            //Assert
            Assert.IsNotNull(redirect);
            Assert.AreEqual("./EpcConfirmation",redirect.PageName);
        }

        [Test]
        public void Constructor_Should_Throw_ArgumentNullException_If_OwnerConsentService_Null()
        {
            // Arrange and act
            var action = () => new DetailsModel(null!, _mockSessionAuthorizationService.Object, _mockSessionHelper.Object);

            // Assert
            action.Should().Throw<ArgumentNullException>().WithParameterName("ownerConsentService");
        }

        [Test]
        public void Constructor_Should_Throw_ArgumentNullException_If_SessionAuthorizationService_Null()
        {
            // Arrange and act
            var action = () => new DetailsModel(_mockOwnerConsentService.Object, null!, _mockSessionHelper.Object);

            // Assert
            action.Should().Throw<ArgumentNullException>().WithParameterName("sessionAuthorizationService");
        }

        [Test]
        public void Constructor_Should_Throw_ArgumentNullException_If_SessionHelper_Null()
        {
            // Arrange and act
            var action = () => new DetailsModel(_mockOwnerConsentService.Object, _mockSessionAuthorizationService.Object, null!);

            // Assert
            action.Should().Throw<ArgumentNullException>().WithParameterName("sessionHelper");
        }
    }
}
