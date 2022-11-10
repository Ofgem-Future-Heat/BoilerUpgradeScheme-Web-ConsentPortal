using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ofgem.Web.BUS.ConsentPortal.WebApp.Pages.Shared
{
    /// <summary>
    /// Error page model for errors that don't have a response body redirected through 
    /// UseStatusCodePagesWithReExecute middleware.
    /// </summary>
    public class RoutingErrorModel : PageModel
    {
        /// <summary>
        /// String used to store error title depending on error code
        /// </summary>
        public string ErrorTitle { get; set;}
        /// <summary>
        /// List containing the content of the message displayed to users depending on error code.
        /// </summary>
        public List<string> ErrorPageContent { get; set; } = new List<string>();
        /// <summary>
        /// GET: Error status code for errors that don't have a response body redirected 
        /// through UseStatusCodePagesWithReExecute middleware.
        /// </summary>
        /// <returns>The page and text related to specific error code.</returns>
        public IActionResult OnGet(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ErrorTitle = "Page not found";
                    ErrorPageContent.Add( "If you typed the web address, check it is correct.");
                    ErrorPageContent.Add("If you pasted the web address, check you copied the entire address.");
                    ErrorPageContent.Add("If you continue to have issues, you can contact Ofgem at:");
                    break;
                case 500:
                    ErrorTitle = "Sorry, there is a problem with the service";
                    ErrorPageContent.Add("Try again later.");
                    ErrorPageContent.Add("If you continue to have issues, you can contact Ofgem at:");
                    break;
                case 503:
                    ErrorTitle = "Sorry, the service is unavailable";
                    ErrorPageContent.Add("Try again later.");
                    ErrorPageContent.Add("If you need to contact Ofgem urgently about the Boiler Upgrade Scheme, you can:");
                    break;
            }
            return Page();

        }
    }
}
