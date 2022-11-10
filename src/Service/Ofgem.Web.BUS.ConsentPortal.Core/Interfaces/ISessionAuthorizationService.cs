namespace Ofgem.Web.BUS.ConsentPortal.Core.Interfaces
{
    /// <summary>
    /// Methods for working with session tokens.
    /// </summary>
    public interface ISessionAuthorizationService
    {
        /// <summary>
        /// Creates a new session token with a 30 minute expiry for the user's session.
        /// </summary>
        /// <param name="consentRequestId">The consent request ID for the user's application.</param>
        /// <returns>A session token.</returns>
        string GenerateSessionToken(string consentRequestId);

        /// <summary>
        /// Checks that the provided session token is valid.
        /// </summary>
        /// <param name="token">The session token to verify.</param>
        /// <returns><c>true</c> if the token is valid, otherwise <c>false</c></returns>
        bool ValidateSessionToken(string token);

        /// <summary>
        /// Extends the expiry time of the session token by 30 minutes.
        /// </summary>
        /// <param name="token">The session token to extend.</param>
        /// <returns>An updated session token.</returns>
        string ExtendSessionToken(string token);
    }
}
