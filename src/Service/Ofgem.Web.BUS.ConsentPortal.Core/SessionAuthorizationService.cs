using Microsoft.IdentityModel.Tokens;
using Ofgem.Web.BUS.ConsentPortal.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ofgem.Web.BUS.ConsentPortal.Core
{
    /// <summary>
    /// Implements the <see cref="ISessionAuthorizationService"/>.
    /// </summary>
    public class SessionAuthorizationService : ISessionAuthorizationService
    {
        private readonly string _consentTokenSecret;

        const int SessionTokenExpiryMinutes = 30;
        const string ConsentRequestIdClaimName = "ConsentRequestId";
        const string ConsentRequestExpiryDateClaimName = "ConsentRequestExpiryDate";

        public SessionAuthorizationService(string consentTokenSecret)
        {
            _consentTokenSecret = consentTokenSecret ?? throw new ArgumentNullException(nameof(consentTokenSecret));
        }

        /// <summary>
        /// Extends the expiry time of the session token by 30 minutes.
        /// </summary>
        /// <param name="token">The session token to extend.</param>
        /// <returns>An updated session token.</returns>
        public string ExtendSessionToken(string token)
        {
            var sessionToken = GetSessionToken(token);

            if (sessionToken != null && sessionToken is JwtSecurityToken jwtSessionToken)
            {
                var consentIdClaim = jwtSessionToken.Claims.FirstOrDefault(claim => claim.Type == ConsentRequestIdClaimName);

                if (consentIdClaim != null)
                {
                    var updatedToken = GenerateSessionToken(consentIdClaim.Value);

                    return updatedToken;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Creates a new session token with a 30 minute expiry for the user's session.
        /// </summary>
        /// <param name="consentRequestId">The consent request ID for the user's application.</param>
        /// <returns>A session token.</returns>
        public string GenerateSessionToken(string consentRequestId)
        {
            var tokenExpiryDateTime = DateTime.UtcNow.AddMinutes(SessionTokenExpiryMinutes);
            
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_consentTokenSecret));
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ConsentRequestIdClaimName, consentRequestId),
                    new Claim(ConsentRequestExpiryDateClaimName, tokenExpiryDateTime.ToString())
                }),
                Expires = tokenExpiryDateTime,
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Checks that the provided session token is valid.
        /// </summary>
        /// <param name="token">The session token to verify.</param>
        /// <returns><c>true</c> if the token is valid, otherwise <c>false</c></returns>
        public bool ValidateSessionToken(string token)
        {
            var sessionToken = GetSessionToken(token);

            if (sessionToken == null)
            {
                return false;
            }

            var isTokenValid = sessionToken.ValidTo.CompareTo(DateTime.UtcNow) >= 0;

            return isTokenValid;
        }

        private SecurityToken? GetSessionToken(string token)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_consentTokenSecret));
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,
                    ValidateAudience = false,
                    ValidateIssuer = false
                }, out SecurityToken validatedToken);

                return validatedToken;
            }
            catch
            {
                // Swallow the exception if the token is invalid
                return null;
            }
        }
    }
}
