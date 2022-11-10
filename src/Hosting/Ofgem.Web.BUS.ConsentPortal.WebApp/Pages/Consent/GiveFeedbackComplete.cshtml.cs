using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.FeatureManagement.Mvc;
using Ofgem.Web.BUS.ConsentPortal.Domain.Constants;

namespace Ofgem.Web.BUS.ConsentPortal.WebApp.Pages.Consent;

/// <summary>
/// Page model for the Confirmation page.
/// Displayed at the end of the happy path journey
/// </summary>
//[ServiceFilter(typeof(SessionTokenAuthorizeAttribute))]
[FeatureGate(FeatureFlags.ConsentFinish)]
public class GiveFeedbackCompleteModel : PageModel
{
    /// <summary>
    /// GET Consent/Confirtmation
    /// </summary>
    /// <returns>The page.</returns>
    public async Task<IActionResult> OnGet()
    {
        return Page();
    }
}
