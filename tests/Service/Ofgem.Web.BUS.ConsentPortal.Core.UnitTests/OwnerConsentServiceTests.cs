using Moq;
using NUnit.Framework;
using Ofgem.API.BUS.PropertyConsents.Client.Interfaces;
using Ofgem.API.BUS.PropertyConsents.Domain.Models.CommsObjects;
using System;
using System.Threading.Tasks;

namespace Ofgem.Web.BUS.ConsentPortal.Core.UnitTests
{
    [TestFixture]
    public class OwnerConsentServiceTests
    {
        private OwnerConsentService _systemUnderTest;
        private Mock<IPropertyConsentAPIClient> _mockPropertyConsentApiClient;

        private readonly Guid _validConsentRequestId = new Guid("EFDBC05E-D16D-45CF-868C-900A8289FDDC");
        private readonly Guid _invalidConsentRequestId = new Guid("5775ff17-c2aa-4042-a206-070e6d421c88");

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

        private TokenVerificationResult _fakeTokenVerificationResult = new TokenVerificationResult();
        private RegisterOwnerConsentResult _registerOwnerConsentResult = new RegisterOwnerConsentResult();

        private string _fakeToken = "5748574857485";

        [OneTimeSetUp]
        public void Setup()
        {
            // TODO: Check invalid setup
            var mockPropertyConsentRequestsClient = new Mock<IPropertyConsentRequestsClient>(MockBehavior.Strict);
            mockPropertyConsentRequestsClient.Setup(client => client.GetConsentRequestSummaryAsync(_validConsentRequestId)).ReturnsAsync(_consentRequestSummary);
            mockPropertyConsentRequestsClient.Setup(client => client.GetConsentRequestSummaryAsync(_invalidConsentRequestId)).ReturnsAsync(new ConsentRequestSummary());

            _mockPropertyConsentApiClient = new Mock<IPropertyConsentAPIClient>(MockBehavior.Strict);
            _mockPropertyConsentApiClient.Setup(client => client.PropertyConsentRequestsClient).Returns(mockPropertyConsentRequestsClient.Object);
        }

        [Test]
        public void Contructor_Throws_ArgumentNullException_If_Api_Client_Is_Null()
        {
            // Act and assert
            Assert.Throws<ArgumentNullException>(() => { _systemUnderTest = new OwnerConsentService(null!); });
        }

        [Test]
        public void Can_Be_Instantiated_With_Valid_Parameters()
        {
            // Act
            _systemUnderTest = new OwnerConsentService(_mockPropertyConsentApiClient.Object);

            // Assert
            Assert.IsNotNull(_systemUnderTest);
        }

        [Test]
        public async Task GetConsentRequestSummary_Returns_Summary_Object_For_Valid_Id()
        {
            // Arrange
            _systemUnderTest = new OwnerConsentService(_mockPropertyConsentApiClient.Object);

            // Act
            var consentRequestSummaryResult = await _systemUnderTest.GetConsentRequestSummary(_validConsentRequestId);

            // Assert
            Assert.NotNull(consentRequestSummaryResult, "Consent request summary result is null");
            Assert.AreEqual("APW12345678", consentRequestSummaryResult.ApplicationReferenceNumber, "Application reference number deserialized incorrectly");
            Assert.AreEqual("Air source heat pump", consentRequestSummaryResult.TechnologyType, "Technology type deserialized incorrectly");
        }

        [Test]
        public async Task GetConsentRequestSummary_Returns_Null_For_Invalid_Id()
        {
            // Arrange
            _systemUnderTest = new OwnerConsentService(_mockPropertyConsentApiClient.Object);

            // Act
            var consentRequestSummaryResult = await _systemUnderTest.GetConsentRequestSummary(_invalidConsentRequestId);

            // Assert
            Assert.IsNull(consentRequestSummaryResult.ApplicationReferenceNumber);
        }

        [Test]
        public void HasConsentRequestExpired_Returns_False_For_Non_Expired_Consent()
        {
            // Arrange
            _systemUnderTest = new OwnerConsentService(_mockPropertyConsentApiClient.Object);
            var consentSummary = new ConsentRequestSummary { ExpiryDate = DateTime.Now.AddMonths(1) };

            // Act
            var consentRequestExpiredResult = _systemUnderTest.HasConsentRequestExpired(consentSummary);

            // Assert
            Assert.IsFalse(consentRequestExpiredResult);
        }

        [Test]
        public void HasConsentRequestExpired_Returns_True_For_Expired_Consent()
        {
            // Arrange
            _systemUnderTest = new OwnerConsentService(_mockPropertyConsentApiClient.Object);
            var consentSummary = new ConsentRequestSummary { ExpiryDate = DateTime.Now.AddMonths(-1) };

            // Act
            var consentRequestExpiredResult = _systemUnderTest.HasConsentRequestExpired(consentSummary);

            // Assert
            Assert.IsTrue(consentRequestExpiredResult);
        }

        [Test]
        public async Task ValidateEmailToken_Returns_Success()
        {
            _fakeTokenVerificationResult.ConsentRequestId = Guid.NewGuid();
            _fakeTokenVerificationResult.TokenAccepted = true;
            _fakeTokenVerificationResult.TokenExpiryDate = DateTime.UtcNow.AddDays(3);

            var mockPropertyConsentRequestsClient = new Mock<IPropertyConsentRequestsClient>(MockBehavior.Strict);
            mockPropertyConsentRequestsClient.Setup(client => client.VerifyWebToken(_fakeToken)).ReturnsAsync(_fakeTokenVerificationResult);

            _mockPropertyConsentApiClient = new Mock<IPropertyConsentAPIClient>(MockBehavior.Strict);
            _mockPropertyConsentApiClient.Setup(client => client.PropertyConsentRequestsClient).Returns(mockPropertyConsentRequestsClient.Object);

            // Arrange
            _systemUnderTest = new OwnerConsentService(_mockPropertyConsentApiClient.Object);

            // Act
            var verifyTokenRespons = await _systemUnderTest.ValidateEmailToken(_fakeToken);

            // Assert
            Assert.IsNotNull(verifyTokenRespons);
            Assert.AreEqual(verifyTokenRespons.TokenAccepted, true);
        }

        [Test]
        public async Task ValidateEmailToken_Returns_Error()
        {
            _fakeTokenVerificationResult.ConsentRequestId = Guid.NewGuid();
            _fakeTokenVerificationResult.TokenAccepted = false;
            _fakeTokenVerificationResult.TokenExpiryDate = DateTime.UtcNow.AddDays(3);

            var mockPropertyConsentRequestsClient = new Mock<IPropertyConsentRequestsClient>(MockBehavior.Strict);
            mockPropertyConsentRequestsClient.Setup(client => client.VerifyWebToken(_fakeToken)).ReturnsAsync(_fakeTokenVerificationResult);

            _mockPropertyConsentApiClient = new Mock<IPropertyConsentAPIClient>(MockBehavior.Strict);
            _mockPropertyConsentApiClient.Setup(client => client.PropertyConsentRequestsClient).Returns(mockPropertyConsentRequestsClient.Object);

            // Arrange
            _systemUnderTest = new OwnerConsentService(_mockPropertyConsentApiClient.Object);


            // Act
            var verifyTokenRespons = await _systemUnderTest.ValidateEmailToken(_fakeToken);

            // Assert
            Assert.IsNotNull(verifyTokenRespons);
            Assert.AreEqual(verifyTokenRespons.TokenAccepted, false);
        }

        [Test]
        public async Task ValidateEmailToken_Returns_Argument_Null_Exception()
        {
            var mockPropertyConsentRequestsClient = new Mock<IPropertyConsentRequestsClient>(MockBehavior.Strict);
            mockPropertyConsentRequestsClient.Setup(client => client.VerifyWebToken(null))
                .ThrowsAsync(new ArgumentNullException());

            _mockPropertyConsentApiClient = new Mock<IPropertyConsentAPIClient>(MockBehavior.Strict);
            _mockPropertyConsentApiClient.Setup(client => client.PropertyConsentRequestsClient).Returns(mockPropertyConsentRequestsClient.Object);

            // Arrange
            _systemUnderTest = new OwnerConsentService(_mockPropertyConsentApiClient.Object);

            //// Act & Assert
            Assert.That(() => _systemUnderTest.ValidateEmailToken(null), Throws.TypeOf<System.ArgumentNullException>());
        }

        [Test]
        public async Task RegisterConsentDeclaration_Returns_Success()
        {
            _registerOwnerConsentResult.IsSuccess = true;
            _registerOwnerConsentResult.IsIneligible = false;
            var fakeConsentId = Guid.NewGuid();

            var mockPropertyConsentRequestsClient = new Mock<IPropertyConsentRequestsClient>(MockBehavior.Strict);
            mockPropertyConsentRequestsClient.Setup(client => client.RegisterConsentAsync(fakeConsentId)).ReturnsAsync(_registerOwnerConsentResult);

            mockPropertyConsentRequestsClient.Setup(client => client.SendConsentConfirmEmailAsync(fakeConsentId))
                .ReturnsAsync(new SendConsentConfirmationEmailResult());

            _mockPropertyConsentApiClient = new Mock<IPropertyConsentAPIClient>(MockBehavior.Strict);
            _mockPropertyConsentApiClient.Setup(client => client.PropertyConsentRequestsClient).Returns(mockPropertyConsentRequestsClient.Object);

            // Arrange
            _systemUnderTest = new OwnerConsentService(_mockPropertyConsentApiClient.Object);

            // Act
            var verifyConsentResponse = await _systemUnderTest.RegisterConsentDeclaration(fakeConsentId);

            // Assert
            Assert.IsNotNull(verifyConsentResponse);
            Assert.AreEqual(verifyConsentResponse.IsSuccess, true);
        }

        [Test]
        public async Task RegisterConsentDeclaration_Returns_Error()
        {
            _registerOwnerConsentResult.IsSuccess = false;
            _registerOwnerConsentResult.IsIneligible = false;
            var fakeConsentId = Guid.NewGuid();

            var mockPropertyConsentRequestsClient = new Mock<IPropertyConsentRequestsClient>(MockBehavior.Strict);
            mockPropertyConsentRequestsClient.Setup(client => client.RegisterConsentAsync(fakeConsentId)).ReturnsAsync(_registerOwnerConsentResult);

            _mockPropertyConsentApiClient = new Mock<IPropertyConsentAPIClient>(MockBehavior.Strict);
            _mockPropertyConsentApiClient.Setup(client => client.PropertyConsentRequestsClient).Returns(mockPropertyConsentRequestsClient.Object);

            // Arrange
            _systemUnderTest = new OwnerConsentService(_mockPropertyConsentApiClient.Object);

            // Act
            var verifyConsentResponse = await _systemUnderTest.RegisterConsentDeclaration(fakeConsentId);

            // Assert
            Assert.IsNotNull(verifyConsentResponse);
            Assert.AreEqual(verifyConsentResponse.IsSuccess, false);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task StorePropertyOwnerFeedback(bool feedbackResultSuccess)
        {
            // Arrange
            _mockPropertyConsentApiClient = new Mock<IPropertyConsentAPIClient>();
            _mockPropertyConsentApiClient.Setup(x =>
                    x.PropertyConsentRequestsClient.StoreFeedbackAsync(It.IsAny<StoreFeedBackRequest>()))
                .ReturnsAsync(new StoreFeedBackResult { IsSuccess = feedbackResultSuccess });

            // Act
            _systemUnderTest = new OwnerConsentService(_mockPropertyConsentApiClient.Object);
                
            var result = await _systemUnderTest.StorePropertyOwnerFeedback(It.IsAny<StoreFeedBackRequest>());

            // Assert
            Assert.AreEqual(result.IsSuccess, feedbackResultSuccess);
        }

        [Test]
        public async Task StorePropertyOwnerFeedback_Null()
        {
            // Arrange
            _mockPropertyConsentApiClient = new Mock<IPropertyConsentAPIClient>();
            _mockPropertyConsentApiClient.Setup(x =>
                    x.PropertyConsentRequestsClient.StoreFeedbackAsync(It.IsAny<StoreFeedBackRequest>()))
                .ReturnsAsync((StoreFeedBackResult)null!);

            // Act
            _systemUnderTest = new OwnerConsentService(_mockPropertyConsentApiClient.Object);

            var result = await _systemUnderTest.StorePropertyOwnerFeedback(It.IsAny<StoreFeedBackRequest>());

            // Assert
            Assert.IsNull(result);
        }
    }
}
