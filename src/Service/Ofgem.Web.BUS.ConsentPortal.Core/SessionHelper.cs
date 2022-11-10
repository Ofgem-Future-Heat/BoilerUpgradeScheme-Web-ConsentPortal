using Microsoft.AspNetCore.Http;
using Ofgem.Web.BUS.ConsentPortal.Core.Interfaces;

namespace Ofgem.Web.BUS.ConsentPortal.Core
{
    /// <summary>
    /// Implements the <see cref="ISessionHelper"/>.
    /// </summary>
    public class SessionHelper : ISessionHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }
        /// <summary>
        /// Adds a key value pair to HttpContext.Session
        /// </summary>
        /// <param name="sessionKey">The session token to extend.</param>
        /// <param name="sessionValue">The session token to extend.</param>
        public void Add(string sessionKey, string sessionValue)
        {
            _httpContextAccessor.HttpContext.Session.SetString(sessionKey, sessionValue);
        }
        /// <summary>
        /// Gets a session variable from HttpContext.Session
        /// </summary>
        /// <param name="sessionKey">The session token to extend.</param>
        /// <returns>A string if a session variable with the provided key exists.</returns>
        public string Get(string sessionKey)
        {
            return _httpContextAccessor.HttpContext.Session.GetString(sessionKey);
        }

    }
}
