using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Ofgem.Web.BUS.ConsentPortal.Core.Interfaces;

namespace Ofgem.Web.BUS.ConsentPortal.Core.Filters
{
    /// <summary>
    /// Ensures that a request is authorized with a token parameter in the querystring,
    /// </summary>
    public class EmailTokenAuthorizeAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        private readonly IOwnerConsentService _ownerConsentService;

        public EmailTokenAuthorizeAttribute(IOwnerConsentService ownerConsentService)
        {
            _ownerConsentService = ownerConsentService ?? throw new ArgumentNullException(nameof(ownerConsentService));
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            const string tokenKey = "token";

            if (!string.IsNullOrEmpty(context.HttpContext.Request.Query[tokenKey]))
            {
                var tokenVerificationResult = await _ownerConsentService.ValidateEmailToken(context.HttpContext.Request.Query[tokenKey]);

                if (tokenVerificationResult != null && tokenVerificationResult.TokenAccepted) return;
            }

            context.Result = new RedirectToPageResult("./SessionExpired");
            context.HttpContext.Response.StatusCode = 401;
        }
    }
}
