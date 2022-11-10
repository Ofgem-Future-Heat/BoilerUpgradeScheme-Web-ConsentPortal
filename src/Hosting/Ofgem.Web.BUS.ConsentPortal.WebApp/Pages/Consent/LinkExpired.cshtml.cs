using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ofgem.API.BUS.PropertyConsents.Domain.Models.CommsObjects;
using Ofgem.Web.BUS.ConsentPortal.Core.Filters;
using Ofgem.Web.BUS.ConsentPortal.Core.Interfaces;

namespace Ofgem.Web.BUS.ConsentPortal.WebApp.Pages.Consent
{
    /// <summary>
    /// Page model for the Link Expired page.
    /// Displayed when the user's email token has expired
    /// </summary>
    [ServiceFilter(typeof(SessionTokenAuthorizeAttribute))]
    public class LinkExpiredModel : PageModel
    {
        private readonly IOwnerConsentService _ownerConsentService;
        private readonly ISessionHelper _sessionHelper;

        public LinkExpiredModel(IOwnerConsentService ownerConsentService, ISessionHelper sessionHelper)
        {
            _ownerConsentService = ownerConsentService ?? throw new ArgumentNullException(nameof(ownerConsentService));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
        }

        /// <summary>
        /// Model containing details of the user's consent request
        /// </summary>
        public ConsentRequestSummary ConsentRequestSummary { get; set; }

        /// <summary>
        /// GET /Consent/LinkExpired
        /// </summary>
        /// <returns>The page.</returns>
        public async Task<IActionResult> OnGet()
        {
            var consentIdString = _sessionHelper.Get("ConsentId");
            var consentId = Guid.Parse(consentIdString);

            var consentRequestSummary = await _ownerConsentService.GetConsentRequestSummary(consentId);
            ConsentRequestSummary = consentRequestSummary;

            return Page();
        }
    }
}
