using FluentAssertions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using NUnit.Framework;
using Ofgem.Web.BUS.ConsentPortal.Core.Interfaces;
using Ofgem.Web.BUS.ConsentPortal.WebApp.Pages.Consent;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.AspNetCore.Mvc;
using Ofgem.API.BUS.PropertyConsents.Domain.Models.CommsObjects;

namespace Ofgem.Web.BUS.ConsentPortal.WebApp.UnitTests.Pages.Consent
{
    [TestFixture]
    public class GiveFeedbackModelTests
    {
        private GiveFeedbackModel _systemUnderTest;
        private Mock<IOwnerConsentService> _mockOwnerConsentService = new();
        private Mock<ISessionHelper> _mockSessionHelper = new();

        [Test]
        public void Constructor_Should_Throw_ArgumentNullException_If_OwnerConsentService_Null()
        {
            // Arrange and act
            var action = () => new GiveFeedbackModel(null!, _mockSessionHelper.Object);

            // Assert
            action.Should().Throw<ArgumentNullException>().WithParameterName("ownerConsentService");
        }

        [Test]
        public void Constructor_Should_Throw_ArgumentNullException_If_SessionHelper_Null()
        {
            // Arrange and act
            var action = () => new GiveFeedbackModel(_mockOwnerConsentService.Object, null!);

            // Assert
            action.Should().Throw<ArgumentNullException>().WithParameterName("sessionHelper");
        }

        [Test]
        public async Task OnGet_ReturnPageResult()
        {
            //Arrange
            var consentId = Guid.NewGuid().ToString();

            _mockSessionHelper.Setup(x => x.Get("ConsentId")).Returns(consentId);

            _systemUnderTest = new GiveFeedbackModel(_mockOwnerConsentService.Object, _mockSessionHelper.Object);

            //Act
            var page = await _systemUnderTest.OnGet() as PageResult;

            //Assert
            Assert.IsNotNull(page);
            Assert.AreEqual(_systemUnderTest.ConsentRequestId, consentId);
        }

        [Test]
        public async Task OnGet_Return_Argument_Null_Exception()
        {
            //Arrange
            _mockSessionHelper.Setup(x => x.Get("ConsentId")).Throws<ArgumentNullException>();

            _systemUnderTest = new GiveFeedbackModel(_mockOwnerConsentService.Object, _mockSessionHelper.Object);

            //// Act & Assert
            Assert.That(() => _systemUnderTest.OnGet(), Throws.TypeOf<System.ArgumentNullException>());
        }

        [Test]
        public async Task OnPost_Has_ErrorMessages()
        {
            //Arrange
            _systemUnderTest = new GiveFeedbackModel(_mockOwnerConsentService.Object, _mockSessionHelper.Object);

            _systemUnderTest.SurveyOption = null;
            _systemUnderTest.FeedbackNarrative = "ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                                                 "dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                                                 "dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                                                 "dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                                                 "dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                                                 "ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                                                 "dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                                                 "dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                                                 "ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                                                 "ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                                                 "ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                                                 "dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                                                 "ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd";
            
            _systemUnderTest.ModelState.AddModelError("SurveyOption", "Tell us how satisfied you are");
            _systemUnderTest.ModelState.AddModelError("FeedbackNarrative", "Use 1200 characters or fewer");

            //Act
            var onGet = await _systemUnderTest.OnPost();

            //Assert
            Assert.IsNotNull(onGet);
            Assert.AreEqual(_systemUnderTest.ErrorMessages.Count, 2);
        }

        [Test]
        public async Task OnPost_SuccessfulRedirect()
        {
            //Arrange
            var consentId = Guid.NewGuid().ToString();

            _mockSessionHelper.Setup(x => x.Get("ConsentId")).Returns(consentId);
            _mockOwnerConsentService.Setup(x => x.StorePropertyOwnerFeedback(It.IsAny<StoreFeedBackRequest>())).ReturnsAsync(new StoreFeedBackResult());

            _systemUnderTest = new GiveFeedbackModel(_mockOwnerConsentService.Object, _mockSessionHelper.Object);

            _systemUnderTest.SurveyOption = "1";
            _systemUnderTest.FeedbackNarrative = "SomeFeedback";

            //Act
            var redirect = await _systemUnderTest.OnPost() as RedirectToPageResult;

            //Assert
            Assert.IsNotNull(redirect);
            Assert.AreEqual("./GiveFeedbackComplete", redirect.PageName);
        }
    }
}
