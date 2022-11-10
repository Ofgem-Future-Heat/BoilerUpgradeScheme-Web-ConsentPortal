using Microsoft.AspNetCore.Mvc.RazorPages;
using NUnit.Framework;
using Ofgem.Web.BUS.ConsentPortal.WebApp.Pages.Consent;
using System.Threading.Tasks;

namespace Ofgem.Web.BUS.ConsentPortal.WebApp.UnitTests.Pages.Consent
{
    [TestFixture]
    public class GiveFeedbackCompleteModelTests
    {
        private GiveFeedbackCompleteModel _systemUnderTest;

        [Test]
        public async Task OnGet_ReturnPageResult()
        {
            // Arrange
            _systemUnderTest = new GiveFeedbackCompleteModel();

            //Act
            var page = await _systemUnderTest.OnGet() as PageResult;

            //Assert
            Assert.IsNotNull(page);
        }
    }
}
