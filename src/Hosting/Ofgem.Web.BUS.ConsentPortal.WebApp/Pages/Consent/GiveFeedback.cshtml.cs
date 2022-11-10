using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.FeatureManagement.Mvc;
using Ofgem.API.BUS.PropertyConsents.Domain.Models.CommsObjects;
using Ofgem.Web.BUS.ConsentPortal.Core.Filters;
using Ofgem.Web.BUS.ConsentPortal.Core.Interfaces;
using Ofgem.Web.BUS.ConsentPortal.Domain.Constants;
using System.ComponentModel.DataAnnotations;

namespace Ofgem.Web.BUS.ConsentPortal.WebApp.Pages.Consent;

/// <summary>
/// Page model for the Details page.
/// </summary>
[ServiceFilter(typeof(SessionTokenAuthorizeAttribute))]
[FeatureGate(FeatureFlags.ConsentFeedback)]
public class GiveFeedbackModel : PageModel
{
    private readonly IOwnerConsentService _ownerConsentService;
    private readonly ISessionHelper _sessionHelper;

    /// <summary>
    /// Value from radio button selection
    /// </summary>
    [BindProperty]
    [Required(ErrorMessage = "Tell us how satisfied you are")]
    public string SurveyOption { get; set; }

    /// <summary>
    /// The consent request to which this feedback is associated.
    /// </summary>
    public string ConsentRequestId { get; set; }

    /// <summary>
    /// The content of the optional text field.
    /// </summary>
    [BindProperty]
    [StringLength(1200, ErrorMessage = "Use 1200 characters or fewer")]
    public string? FeedbackNarrative { get; set; }

    /// <summary>
    /// A list of all errors from client-side validation
    /// </summary>
    public List<string> ErrorMessages { get; set; }


    public GiveFeedbackModel(IOwnerConsentService ownerConsentService, ISessionHelper sessionHelper)
    {
        _ownerConsentService = ownerConsentService ?? throw new ArgumentNullException(nameof(ownerConsentService));
        _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
    }

    /// <summary>
    /// GET: Consent/Details
    /// </summary>
    /// <returns>The page.</returns>
    public async Task<IActionResult> OnGet()
    {
        var consentRequestIdString = _sessionHelper.Get("ConsentId");

        ConsentRequestId = consentRequestIdString;

        return this.Page();
    }

    /// <summary>
    /// POST: Consent/Give-feedback
    /// </summary>
    /// <returns>The current page if validation fails. Redirects to the declaration page if successful.</returns>
    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            ErrorMessages = new();

            var modelStateErrors = ModelState.SelectMany(x => x.Value.Errors);
            ErrorMessages.AddRange(modelStateErrors.Select(x => x.ErrorMessage));

            return await OnGet();
        }
        else
        {
            var consentRequestIdString = _sessionHelper.Get("ConsentId");
            var feedbackData = new StoreFeedBackRequest
            {
                ConsentRequestId = Guid.Parse(consentRequestIdString),
                FeedbackNarratiave = FeedbackNarrative,
                SurveyOption = Int32.Parse(this.SurveyOption)
            };
            await _ownerConsentService.StorePropertyOwnerFeedback(feedbackData);

            return RedirectToPage("./GiveFeedbackComplete");
        }
    }
}

