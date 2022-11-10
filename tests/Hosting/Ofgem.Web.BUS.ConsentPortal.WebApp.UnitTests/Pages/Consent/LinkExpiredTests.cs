using FluentAssertions;
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
    public class LinkExpiredModelTests
    {
        private LinkExpiredModel _systemUnderTest;
        private Mock<IOwnerConsentService> _mockOwnerConsentService = new();
        private Mock<ISessionHelper> _mockSessionHelper = new();

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
            QuoteAmount = 11405
        };

        [Test]
        public void Constructor_Should_Throw_ArgumentNullException_If_OwnerConsentService_Null()
        {
            // Arrange and act
            var action = () => new LinkExpiredModel(null!, _mockSessionHelper.Object);

            // Assert
            action.Should().Throw<ArgumentNullException>().WithParameterName("ownerConsentService");
        }

        [Test]
        public void Constructor_Should_Throw_ArgumentNullException_If_SessionHelper_Null()
        {
            // Arrange and act
            var action = () => new LinkExpiredModel(_mockOwnerConsentService.Object, null!);

            // Assert
            action.Should().Throw<ArgumentNullException>().WithParameterName("sessionHelper");
        }

        [Test]
        public async Task OnGet_ReturnPageResult()
        {
            //Arrange
            var consentId = Guid.NewGuid();

            _mockSessionHelper.Setup(x => x.Get("ConsentId")).Returns(consentId.ToString());
            _mockOwnerConsentService.Setup(x => x.GetConsentRequestSummary(consentId))
                .ReturnsAsync(_consentRequestSummary);

            _systemUnderTest = new LinkExpiredModel(_mockOwnerConsentService.Object, _mockSessionHelper.Object);

            //Act
            var page = await _systemUnderTest.OnGet() as PageResult;

            //Assert
            Assert.IsNotNull(page);
        }

        [Test]
        public async Task OnGet_Return_Argument_Null_Exception()
        {
            //Arrange
            _mockSessionHelper.Setup(x => x.Get("ConsentId")).Throws<ArgumentNullException>();

            _systemUnderTest = new LinkExpiredModel(_mockOwnerConsentService.Object, _mockSessionHelper.Object);

            //// Act & Assert
            Assert.That(() => _systemUnderTest.OnGet(), Throws.TypeOf<System.ArgumentNullException>());
        }
    }
}
