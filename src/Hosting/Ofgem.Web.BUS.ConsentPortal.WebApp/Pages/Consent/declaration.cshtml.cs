using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ofgem.API.BUS.PropertyConsents.Domain.Models.CommsObjects;
using Ofgem.Web.BUS.ConsentPortal.Core.Filters;
using Ofgem.Web.BUS.ConsentPortal.Core.Interfaces;

namespace Ofgem.Web.BUS.ConsentPortal.WebApp.Pages.Consent
{
    /// <summary>
    /// Page model for the Declaration page
    /// </summary>
    [ServiceFilter(typeof(SessionTokenAuthorizeAttribute))]
    public class DeclarationModel : PageModel
    {
        private readonly ISessionAuthorizationService _sessionAuthorizationService;
        private readonly ISessionHelper _sessionHelper;
        private readonly IOwnerConsentService _ownerConsentService;

        public DeclarationModel(IOwnerConsentService ownerConsentService, ISessionAuthorizationService sessionAuthorizationService, ISessionHelper sessionHelper)
        {
            _ownerConsentService = ownerConsentService ?? throw new ArgumentNullException(nameof(ownerConsentService));
            _sessionAuthorizationService = sessionAuthorizationService ?? throw new ArgumentNullException(nameof(sessionAuthorizationService));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
        }

        /// <summary>
        /// Model containing details of the user's consent request
        /// </summary>
        public ConsentRequestSummary ConsentRequestSummary { get; set; }

        /// <summary>
        /// Flag to show an error message on the front end.
        /// </summary>
        public bool DisplayError { get; set; }

        /// <summary>
        /// Tickbox value indicating that the user is the owner of the system.
        /// </summary>
        [BindProperty]
        public bool OwnerAgreed { get; set; }
        
        /// <summary>
        /// Tickbox value indicating that the user agrees to funding.
        /// </summary>
        [BindProperty]
        public bool FundingAgreed { get; set; }
        
        /// <summary>
        /// Tickbox value indicating that the user's property is not used for social housing.
        /// </summary>
        [BindProperty]
        public bool SocialHousingAgreed { get; set; }

        /// <summary>
        /// Tickbox indicating that the boiler isnt funded by the ECO.
        /// </summary>
        [BindProperty]
        public bool NotFundedByEnergyCompanyObligationAgreed { get; set; }

        /// <summary>
        /// Tickbox indicating that the owner has acknowledged that their case may be subject to audit.
        /// </summary>
        [BindProperty]
        public bool SubjectToAuditAgreed { get; set; }


        /// <summary>
        /// GET: Consent/Declaration
        /// </summary>
        /// <returns>The page.</returns>
        public async Task<IActionResult> OnGet()
        {
            var consentIdString = _sessionHelper.Get("ConsentId");
            var consentId = Guid.Parse(consentIdString);
            var sessionId = _sessionHelper.Get("SessionId");

            var consentRequestSummary = await _ownerConsentService.GetConsentRequestSummary(consentId);
            ConsentRequestSummary = consentRequestSummary;

            var sessionToken = _sessionAuthorizationService.ExtendSessionToken(sessionId);

            return Page();
        }

        /// <summary>
        /// POST: Consent/Declaration
        /// </summary>
        /// <returns>The current page if validation fails. Redirect to the confirmation page if successful.</returns>
        public async Task<IActionResult> OnPost()
        {
            if (!OwnerAgreed || !FundingAgreed || !SocialHousingAgreed || !NotFundedByEnergyCompanyObligationAgreed || !SubjectToAuditAgreed)
            {
                DisplayError = true;

                return await OnGet();
            }

            var consentIdString = _sessionHelper.Get("ConsentId");
            var consentId = Guid.Parse(consentIdString);

            var registerConsentResult = await _ownerConsentService.RegisterConsentDeclaration(consentId);

            if (registerConsentResult.IsSuccess)
            {
                return RedirectToPage("./confirmation");
            }

            return RedirectToPage("./AlreadyGiven");
        }
    }
}
