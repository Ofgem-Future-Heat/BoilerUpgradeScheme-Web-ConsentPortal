using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.FeatureManagement.Mvc;
using Ofgem.API.BUS.PropertyConsents.Domain.Models.CommsObjects;
using Ofgem.Web.BUS.ConsentPortal.Core.Filters;
using Ofgem.Web.BUS.ConsentPortal.Core.Interfaces;
using Ofgem.Web.BUS.ConsentPortal.Domain.Constants;

namespace Ofgem.Web.BUS.ConsentPortal.WebApp.Pages.Consent;

/// <summary>
/// Page model for the Confirmation page.
/// Displayed at the end of the happy path journey
/// </summary>
[ServiceFilter(typeof(SessionTokenAuthorizeAttribute))]
[FeatureGate(FeatureFlags.ConsentFinish)]
public class ConfirmationModel : PageModel
{
    private readonly IOwnerConsentService _ownerConsentService;
    private readonly ISessionHelper _sessionHelper;

    public ConfirmationModel(IOwnerConsentService ownerConsentService, ISessionHelper sessionHelper)
    {
        _ownerConsentService = ownerConsentService ?? throw new ArgumentNullException(nameof(ownerConsentService));
        _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
    }

    /// <summary>
    /// Model containing details of the user's consent request
    /// </summary>
    public ConsentRequestSummary ConsentRequestSummary { get; set; }

    /// <summary>
    /// GET Consent/Confirtmation
    /// </summary>
    /// <returns>The page.</returns>
    public async Task<IActionResult> OnGet()
    {
        var consentIdString = _sessionHelper.Get("ConsentId");
        var consentId = Guid.Parse(consentIdString);
        var sessionId = _sessionHelper.Get("SessionId");

        var consentRequestSummary = await _ownerConsentService.GetConsentRequestSummary(consentId);
        ConsentRequestSummary = consentRequestSummary;

        return Page();
    }
}
