using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using NUnit.Framework;
using Ofgem.Web.BUS.ConsentPortal.Core.Interfaces;
using Ofgem.Web.BUS.ConsentPortal.WebApp.Pages.consent;

namespace Ofgem.Web.BUS.ConsentPortal.WebApp.UnitTests.Pages.Consent
{
    [TestFixture]
    public class EpcConfirmationModelTests
    {
        private EpcConfirmationModel _systemUnderTest;
        private Mock<ISessionAuthorizationService> _mockSessionAuthorizationService = new();
        private Mock<ISessionHelper> _mockSessionHelper = new();

        private const string SessionId = "my super secret consent token secret";

        [Test]
        public async Task OnGet_ReturnPageResult()
        {
            //Arrange

            _mockSessionHelper.Setup(x => x.Get("SessionId")).Returns(SessionId);

            _systemUnderTest = new EpcConfirmationModel(_mockSessionAuthorizationService.Object, _mockSessionHelper.Object);

            //Act
            var page = _systemUnderTest.OnGet() as PageResult;

            //Assert
            Assert.IsNotNull(page);

        }

        [Test]
        public async Task OnPost_Niether_Checkbox_Ticked_ReturnPageResult()
        {
            //Arrange
            _systemUnderTest = new EpcConfirmationModel(_mockSessionAuthorizationService.Object, _mockSessionHelper.Object);
            _systemUnderTest.PageContext = new PageContext();

            //Act
            var page = await _systemUnderTest.OnPost() as PageResult;

            //Assert
            Assert.AreEqual(_systemUnderTest.EPCMeetsEligibilityError, true);
            Assert.AreEqual(_systemUnderTest.EpcDeclarationError, true);
            Assert.IsNotNull(page);
        }

        [Test]
        public async Task OnPost_Only_EPCMeetsEligibility_Checked_ReturnRedirectToPageResult()
        {
            //Arrange
            _systemUnderTest = new EpcConfirmationModel(_mockSessionAuthorizationService.Object, _mockSessionHelper.Object);
            _systemUnderTest.EPCMeetsEligibility = true;
            _systemUnderTest.EpcDeclaration = false;
            _systemUnderTest.PageContext = new PageContext();

            //Act
            var page = await _systemUnderTest.OnPost() as PageResult;

            //Assert
            Assert.AreEqual(_systemUnderTest.EPCMeetsEligibilityError, false);
            Assert.AreEqual(_systemUnderTest.EpcDeclarationError, true);
            Assert.IsNotNull(page);
        }

        [Test]
        public async Task OnPost_Only_EpcDeclaration_Checked_ReturnRedirectToPageResult()
        {
            //Arrange
            _systemUnderTest = new EpcConfirmationModel(_mockSessionAuthorizationService.Object, _mockSessionHelper.Object);
            _systemUnderTest.EPCMeetsEligibility = false;
            _systemUnderTest.EpcDeclaration = true;
            _systemUnderTest.PageContext = new PageContext();

            //Act
            var page = await _systemUnderTest.OnPost() as PageResult;

            //Assert
            Assert.AreEqual(_systemUnderTest.EPCMeetsEligibilityError, true);
            Assert.AreEqual(_systemUnderTest.EpcDeclarationError, false);
            Assert.IsNotNull(page);
        }


        [Test]
        public async Task OnPost_HasConsented_ReturnRedirectToPageResult()
        {
            //Arrange
            _systemUnderTest = new EpcConfirmationModel(_mockSessionAuthorizationService.Object, _mockSessionHelper.Object);
            _systemUnderTest.EPCMeetsEligibility = true;
            _systemUnderTest.EpcDeclaration = true;
            _systemUnderTest.PageContext = new PageContext();

            //Act
            var redirect = await _systemUnderTest.OnPost() as RedirectToPageResult;

            //Assert
            Assert.IsNotNull(redirect);
            Assert.AreEqual(redirect.PageName, "./declaration");
        }

        [Test]
        public void Constructor_Should_Throw_ArgumentNullException_If_SessionAuthorizationService_Null()
        {
            // Arrange and act
            var action = () => new EpcConfirmationModel(null!, _mockSessionHelper.Object);

            // Assert
            action.Should().Throw<ArgumentNullException>().WithParameterName("sessionAuthorizationService");
        }

        [Test]
        public void Constructor_Should_Throw_ArgumentNullException_If_SessionHelper_Null()
        {
            // Arrange and act
            var action = () => new EpcConfirmationModel(_mockSessionAuthorizationService.Object, null!);

            // Assert
            action.Should().Throw<ArgumentNullException>().WithParameterName("sessionHelper");
        }
    }
}