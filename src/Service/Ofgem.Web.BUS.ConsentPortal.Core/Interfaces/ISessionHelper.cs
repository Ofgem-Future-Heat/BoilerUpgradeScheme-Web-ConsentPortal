namespace Ofgem.Web.BUS.ConsentPortal.Core.Interfaces
{
    /// <summary>
    /// Methods for interacting with HttpContext.Session.
    /// </summary>
    public interface ISessionHelper
    {
        /// <summary>
        /// Adds a key value pair to HttpContext.Session
        /// </summary>
        /// <param name="sessionKey">The session token to extend.</param>
        /// <param name="sessionValue">The session token to extend.</param>
        public void Add(string sessionKey, string sessionValue);
        /// <summary>
        /// Gets a session variable from HttpContext.Session
        /// </summary>
        /// <param name="sessionKey">The session token to extend.</param>
        /// <returns>A string if a session variable with the provided key exists.</returns>
        public string Get(string sessionKey);
    }
}
