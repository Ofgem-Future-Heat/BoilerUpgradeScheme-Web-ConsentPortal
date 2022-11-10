using NUnit.Framework;
using System.Threading.Tasks;
using Ofgem.Web.BUS.ConsentPortal.WebApp.Pages.Shared;

namespace Ofgem.Web.BUS.ConsentPortal.WebApp.UnitTests.Pages.Consent
{
    [TestFixture]
    public class RoutingErrorTests
    {
        [Test]
        public async Task OnGet_ReturnPageResult()
        {
            //Arrange
            var _systemUnderTest = new RoutingErrorModel();
            var pageResult = _systemUnderTest.OnGet(404);

            //Act & Assert
            Assert.IsNotNull(pageResult);
        }
    }
}