using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.FeatureManagement.Mvc;
using Ofgem.API.BUS.PropertyConsents.Domain.Models.CommsObjects;
using Ofgem.Web.BUS.ConsentPortal.Core.Filters;
using Ofgem.Web.BUS.ConsentPortal.Core.Interfaces;
using Ofgem.Web.BUS.ConsentPortal.Domain.Constants;

namespace Ofgem.Web.BUS.ConsentPortal.WebApp.Pages.Consent;

/// <summary>
/// Page model for the Details page.
/// </summary>
[ServiceFilter(typeof(EmailTokenAuthorizeAttribute))]
[FeatureGate(FeatureFlags.ConsentAllow)]
public class DetailsModel : PageModel
{
    private readonly IOwnerConsentService _ownerConsentService;
    private readonly ISessionAuthorizationService _sessionAuthorizationService;
    private readonly ISessionHelper _sessionHelper;

    /// <summary>
    /// Model containing details of the user's consent request
    /// </summary>
    public ConsentRequestSummary ConsentRequestSummary { get; set; }

    /// <summary>
    /// Flag to show an error message on the front end.
    /// </summary>
    public bool DisplayError { get; set; }

    /// <summary>
    /// Tickbox value indicating that the user has confirmed consent.
    /// </summary>
    [BindProperty]
    public bool ConsentAgreed { get; set; }

    /// <summary>
    /// Tickbox value indicating that the user has confirmed they are the named owner.
    /// </summary>
    [BindProperty]
    public bool UserConfirmsIsNamedOwner { get; set; }

    public DetailsModel(IOwnerConsentService ownerConsentService, ISessionAuthorizationService sessionAuthorizationService, ISessionHelper sessionHelper)
    {
        _ownerConsentService = ownerConsentService ?? throw new ArgumentNullException(nameof(ownerConsentService));
        _sessionAuthorizationService = sessionAuthorizationService ?? throw new ArgumentNullException(nameof(sessionAuthorizationService));
        _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
    }

    /// <summary>
    /// GET: Consent/Details
    /// </summary>
    /// <returns>The page.</returns>
    public async Task<IActionResult> OnGet(string token)
    {
        TokenVerificationResult? tokenValidation = await _ownerConsentService.ValidateEmailToken(token).ConfigureAwait(false);

        if (tokenValidation == null || !tokenValidation.TokenAccepted || tokenValidation.ConsentRequestId == null)
        {
            return RedirectToPage("./SessionExpired");
        }

        //Unhappy path uses consentRequestIdString so session needs to be setup before checks so we can still use the same Auth.
        Guid consentRequestIdGuid = tokenValidation.ConsentRequestId.Value;
        string consentRequestIdString = consentRequestIdGuid.ToString();

        ConsentRequestSummary? consentRequestSummary = await _ownerConsentService.GetConsentRequestSummary(consentRequestIdGuid);
        ConsentRequestSummary = consentRequestSummary!;
        var sessionToken = _sessionAuthorizationService.GenerateSessionToken(consentRequestIdString);

        _sessionHelper.Add("Token", token);
        _sessionHelper.Add("SessionId", sessionToken);
        _sessionHelper.Add("ConsentId", consentRequestIdString!);

        if (consentRequestSummary.HasConsented != null)
        {
            return RedirectToPage("./AlreadyGiven");
        }
        else if (consentRequestSummary.ExpiryDate <= DateTime.UtcNow)
        {
            return RedirectToPage("./LinkExpired");
        }

        return Page();
    }

    /// <summary>
    /// POST: Consent/Details
    /// </summary>
    /// <returns>The current page if validation fails. Redirects to the declaration page if successful.</returns>
    public async Task<IActionResult> OnPost()
    {
        if (!ConsentAgreed || !UserConfirmsIsNamedOwner)
        {
            DisplayError = true;

            return await OnGet(_sessionHelper.Get("Token"));
        }
        
        return RedirectToPage("./EpcConfirmation");
    }
}

