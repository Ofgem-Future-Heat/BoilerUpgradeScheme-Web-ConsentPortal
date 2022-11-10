using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Ofgem.Web.BUS.ConsentPortal.Core.Interfaces;

namespace Ofgem.Web.BUS.ConsentPortal.Core.Filters
{
    /// <summary>
    /// Ensures that a request is authorized with a session ID parameter in either the form body or querystring
    /// </summary>
    public class SessionTokenAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly ISessionAuthorizationService _sessionAuthorizationService;
        private readonly ISessionHelper _sessionHelper;

        public SessionTokenAuthorizeAttribute(ISessionAuthorizationService sessionAuthorizationService, ISessionHelper sessionHelper)
        {
            _sessionAuthorizationService = sessionAuthorizationService ?? throw new ArgumentNullException(nameof(sessionAuthorizationService));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {

            string sessionValue = _sessionHelper.Get("SessionId");

            if (!string.IsNullOrEmpty(sessionValue))
            {
                var isTokenValid = _sessionAuthorizationService.ValidateSessionToken(sessionValue);

                if (isTokenValid) return;
            }

            context.Result = new RedirectToPageResult("./SessionExpired");
            context.HttpContext.Response.StatusCode = 401;
        }
    }
}
