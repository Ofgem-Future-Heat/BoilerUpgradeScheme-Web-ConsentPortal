using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ofgem.Web.BUS.ConsentPortal.Core.Filters;
using Ofgem.Web.BUS.ConsentPortal.Core.Interfaces;

namespace Ofgem.Web.BUS.ConsentPortal.WebApp.Pages.consent
{
    /// <summary>
    /// Page model for the EpcConfirmation page
    /// </summary>
    [ServiceFilter(typeof(SessionTokenAuthorizeAttribute))]
    public class EpcConfirmationModel : PageModel
    {
        private readonly ISessionAuthorizationService _sessionAuthorizationService;
        private readonly ISessionHelper _sessionHelper;

        public EpcConfirmationModel(ISessionAuthorizationService sessionAuthorizationService, ISessionHelper sessionHelper)
        {
            _sessionAuthorizationService = sessionAuthorizationService ?? throw new ArgumentNullException(nameof(sessionAuthorizationService));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
        }
        /// <summary>
        /// Flag to show an error message for EpcDeclarationError on the front end.
        /// </summary>
        public bool EpcDeclarationError { get; set; }

        /// <summary>
        /// Flag to show an error message for EPCMeetsEligibilityError on the front end.
        /// </summary>
        public bool EPCMeetsEligibilityError { get; set; }

        /// <summary>
        /// Tickbox value indicating that the user meets the epc requirements.
        /// </summary>
        [BindProperty]
        public bool EPCMeetsEligibility { get; set; }

        /// <summary>
        /// Tickbox value indicating that the user agrees that they understand the implications of agreeing.
        /// </summary>
        [BindProperty]
        public bool EpcDeclaration { get; set; }

        /// <summary>
        /// GET: Consent/Session Details
        /// </summary>
        /// <returns>The page.</returns>
        public IActionResult OnGet()
        {
            var sessionId = _sessionHelper.Get("SessionId");

            _sessionAuthorizationService.ExtendSessionToken(sessionId);
            return Page();
        }

        /// <summary>
        /// POST: Consent/Declaration
        /// </summary>
        /// <returns>The current page if validation fails. Redirect to the declaration page if successful.</returns>
        public async Task<IActionResult> OnPost()
        {
            if (!EpcDeclaration || !EPCMeetsEligibility)
            {
                if (!EpcDeclaration && !EPCMeetsEligibility)
                {
                    EPCMeetsEligibilityError = true;
                    EpcDeclarationError = true;
                }
                else if (!EPCMeetsEligibility)
                {
                    EPCMeetsEligibilityError = true;
                    EpcDeclarationError = false;

                }
                else
                {
                    EPCMeetsEligibilityError = false;
                    EpcDeclarationError = true;
                }

                return Page();

            }

            return RedirectToPage("./declaration");
        }
    }
}
