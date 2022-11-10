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
    public class DeclarationModelTests
    {
        private DeclarationModel _systemUnderTest;
        private Mock<ISessionAuthorizationService> _mockSessionAuthorizationService = new();
        private Mock<IOwnerConsentService> _mockOwnerConsentService = new();
        private Mock<ISessionHelper> _mockSessionHelper = new();

        [Test]
        public async Task OnGet_ReturnPageResult()
        {
            //Arrange
            var consentId = Guid.NewGuid();
            _mockSessionHelper.Setup(x => x.Get("ConsentId")).Returns(consentId.ToString());

            _systemUnderTest = new DeclarationModel(_mockOwnerConsentService.Object, _mockSessionAuthorizationService.Object, _mockSessionHelper.Object);

            //Act
            var page = await _systemUnderTest.OnGet() as PageResult;

            //Assert
            Assert.IsNotNull(page);

        }

        [Test]
        public async Task OnPost_NotConsented_ReturnPageResult()
        {
            //Arrange
            var consentId = Guid.NewGuid();
            _mockSessionHelper.Setup(x => x.Get("ConsentId")).Returns(consentId.ToString());

            _systemUnderTest = new DeclarationModel(_mockOwnerConsentService.Object, _mockSessionAuthorizationService.Object, _mockSessionHelper.Object);
            _systemUnderTest.PageContext = new PageContext();


            //Act
            var page = await _systemUnderTest.OnPost() as PageResult;

            //Assert
            Assert.IsNotNull(page);
        }


        [Test]
        public async Task OnPost_HasConsented_RegisterConsentReceivedPassed_ReturnRedirectToPageResult()
        {
            //Arrange
            var registerConsentResult = new RegisterOwnerConsentResult
            {
                IsSuccess = true
            };
            _mockOwnerConsentService.Setup(m => m.RegisterConsentDeclaration(It.IsAny<Guid>()).Result).Returns(registerConsentResult);

            var consentId = Guid.NewGuid();
            _mockSessionHelper.Setup(x => x.Get("ConsentId")).Returns(consentId.ToString());

            _systemUnderTest = new DeclarationModel(_mockOwnerConsentService.Object, _mockSessionAuthorizationService.Object, _mockSessionHelper.Object);
            _systemUnderTest.OwnerAgreed = true;
            _systemUnderTest.FundingAgreed = true;
            _systemUnderTest.SocialHousingAgreed = true;
            _systemUnderTest.NotFundedByEnergyCompanyObligationAgreed = true;
            _systemUnderTest.SubjectToAuditAgreed = true;   
            _systemUnderTest.PageContext = new PageContext();

            //Act
            var redirect = await _systemUnderTest.OnPost() as RedirectToPageResult;

            //Assert
            Assert.IsNotNull(redirect);
            Assert.AreEqual(redirect.PageName, "./confirmation");
        }

        [Test]
        public async Task OnPost_HasConsented_RegisterConsentReceivedFailed_ReturnRedirectToPageResult()
        {
            //Arrange
            var registerConsentResult = new RegisterOwnerConsentResult
            {
                IsSuccess = false,
                IsIneligible = false
            };
            _mockOwnerConsentService.Setup(m => m.RegisterConsentDeclaration(It.IsAny<Guid>()).Result).Returns(registerConsentResult);

            var consentId = Guid.NewGuid();
            _mockSessionHelper.Setup(x => x.Get("ConsentId")).Returns(consentId.ToString());

            _systemUnderTest = new DeclarationModel(_mockOwnerConsentService.Object, _mockSessionAuthorizationService.Object, _mockSessionHelper.Object);
            _systemUnderTest.OwnerAgreed = true;
            _systemUnderTest.FundingAgreed = true;
            _systemUnderTest.SocialHousingAgreed = true;
            _systemUnderTest.NotFundedByEnergyCompanyObligationAgreed = true;
            _systemUnderTest.SubjectToAuditAgreed = true;
            _systemUnderTest.PageContext = new PageContext();

            //Act
            var redirect = await _systemUnderTest.OnPost() as RedirectToPageResult;

            //Assert
            Assert.IsNotNull(redirect);
            Assert.AreEqual(redirect.PageName, "./AlreadyGiven");
        }

        [Test]
        public void Constructor_Should_Throw_ArgumentNullException_If_OwnerConsentService_Null()
        {
            // Arrange and act
            var action = () => new DeclarationModel(null!, _mockSessionAuthorizationService.Object, _mockSessionHelper.Object);

            // Assert
            action.Should().Throw<ArgumentNullException>().WithParameterName("ownerConsentService");
        }

        [Test]
        public void Constructor_Should_Throw_ArgumentNullException_If_SessionAuthorizationService_Null()
        {
            // Arrange and act
            var action = () => new DeclarationModel(_mockOwnerConsentService.Object, null!, _mockSessionHelper.Object);

            // Assert
            action.Should().Throw<ArgumentNullException>().WithParameterName("sessionAuthorizationService");
        }

        [Test]
        public void Constructor_Should_Throw_ArgumentNullException_If_SessionHelper_Null()
        {
            // Arrange and act
            var action = () => new DeclarationModel(_mockOwnerConsentService.Object, _mockSessionAuthorizationService.Object, null!);

            // Assert
            action.Should().Throw<ArgumentNullException>().WithParameterName("sessionHelper");
        }
    }
}
