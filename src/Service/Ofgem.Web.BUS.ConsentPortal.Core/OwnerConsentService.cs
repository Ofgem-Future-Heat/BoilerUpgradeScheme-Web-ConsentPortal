using Ofgem.API.BUS.PropertyConsents.Client.Interfaces;
using Ofgem.API.BUS.PropertyConsents.Domain.Models.CommsObjects;
using Ofgem.Web.BUS.ConsentPortal.Core.Interfaces;

namespace Ofgem.Web.BUS.ConsentPortal.Core
{
    /// <summary>
    /// Implements the <see cref="IOwnerConsentService"/>.
    /// </summary>
    public class OwnerConsentService : IOwnerConsentService
    {
        private readonly IPropertyConsentAPIClient _propertyConsentAPIClient;

        public OwnerConsentService(IPropertyConsentAPIClient propertyConsentAPIClient)
        {
            _propertyConsentAPIClient = propertyConsentAPIClient ?? throw new ArgumentNullException(nameof(propertyConsentAPIClient));
        }

        /// <summary>
        /// Retrieves a summary of the consent request for the user's application.
        /// </summary>
        /// <param name="consentRequestId">The consent request ID.</param>
        /// <returns>A <see cref="ConsentRequestSummary"/> object containing the user's consent request.</returns>
        public async Task<ConsentRequestSummary?> GetConsentRequestSummary(Guid consentRequestId)
        {
            var consentRequestSummaryResponse = await _propertyConsentAPIClient.PropertyConsentRequestsClient.GetConsentRequestSummaryAsync(consentRequestId);

            return consentRequestSummaryResponse;
        }

        /// <summary>
        /// Checks that a valid JWT auth token has been provided and has not expired.
        /// </summary>
        /// <param name="token">The token to validate.</param>
        /// <returns>A <see cref="TokenVerificationResult"/> object containing the status of the token.</returns>
        public async Task<TokenVerificationResult> ValidateEmailToken(string token)
        {
            var verifyWebTokenResponse = await _propertyConsentAPIClient.PropertyConsentRequestsClient.VerifyWebToken(token);

            if (!verifyWebTokenResponse.TokenAccepted)
            {
                return new TokenVerificationResult { TokenAccepted = false };
            }

            return verifyWebTokenResponse;
        }

        /// <summary>
        /// Registers a consent request as received.
        /// </summary>
        /// <param name="consentRequestId">The consent request ID.</param>
        /// <returns>A <see cref="RegisterOwnerConsentResult"/> object containing the status of the registration.</returns>
        public async Task<RegisterOwnerConsentResult> RegisterConsentDeclaration(Guid consentRequestId)
        {
            var registerConsentResult = await _propertyConsentAPIClient.PropertyConsentRequestsClient.RegisterConsentAsync(consentRequestId);

            if (registerConsentResult.IsSuccess)
            {
                await _propertyConsentAPIClient.PropertyConsentRequestsClient.SendConsentConfirmEmailAsync(consentRequestId);
            }

            return registerConsentResult;
        }

        /// <summary>
        /// Checks if the expiry date on a consent request has passed.
        /// </summary>
        /// <param name="consentRequestSummary">The consent request summary.</param>
        /// <returns><c>true</c> if the request has expired, otherwise <c>false</c>.</returns>
        public bool HasConsentRequestExpired(ConsentRequestSummary consentRequestSummary)
        {
            var hasExpiryDatePassed = DateTime.UtcNow.CompareTo(consentRequestSummary.ExpiryDate) > 0;

            return hasExpiryDatePassed;
        }

        /// <summary>
        /// Sends PO feedback to the database to be stored
        /// </summary>
        /// <param name="feedback"></param>
        /// <returns></returns>
        public async Task<StoreFeedBackResult> StorePropertyOwnerFeedback(StoreFeedBackRequest feedback)
        {
            var storeFeedBackResult = await _propertyConsentAPIClient.PropertyConsentRequestsClient.StoreFeedbackAsync(feedback);

            return storeFeedBackResult;
        }
    }
}
