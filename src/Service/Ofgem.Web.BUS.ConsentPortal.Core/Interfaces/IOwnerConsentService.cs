using Ofgem.API.BUS.PropertyConsents.Domain.Models.CommsObjects;

namespace Ofgem.Web.BUS.ConsentPortal.Core.Interfaces
{
    /// <summary>
    /// Methods for interacting with the consent API client.
    /// </summary>
    public interface IOwnerConsentService
    {
        /// <summary>
        /// Registers a consent request as received in the applications database.
        /// </summary>
        /// <param name="consentRequestId">The consent request ID.</param>
        /// <returns>A <see cref="RegisterOwnerConsentResult"/> object containing the status of the registration.</returns>
        Task<RegisterOwnerConsentResult> RegisterConsentDeclaration(Guid consentRequestId);

        /// <summary>
        /// Retrieves a summary of the consent request for the user's application.
        /// </summary>
        /// <param name="consentRequestId">The consent request ID.</param>
        /// <returns>A <see cref="ConsentRequestSummary"/> object containing the user's consent request.</returns>
        Task<ConsentRequestSummary?> GetConsentRequestSummary(Guid consentRequestId);

        /// <summary>
        /// Checks that a valid JWT auth token has been provided and has not expired.
        /// </summary>
        /// <param name="token">The token to validate.</param>
        /// <returns>A <see cref="TokenVerificationResult"/> object containing the status of the token.</returns>
        Task<TokenVerificationResult> ValidateEmailToken(string token);

        /// <summary>
        /// Checks if the expiry date on a consent request has passed.
        /// </summary>
        /// <param name="consentRequestSummary">The consent request summary.</param>
        /// <returns><c>true</c> if the request has expired, otherwise <c>false</c>.</returns>
        bool HasConsentRequestExpired(ConsentRequestSummary consentRequestSummary);

        /// <summary>
        /// Sends PO feedback to stored onto the Application database.
        /// </summary>
        /// <param name="feedback"></param>
        /// <returns></returns>
        Task<StoreFeedBackResult> StorePropertyOwnerFeedback(StoreFeedBackRequest feedback);

    }
}
