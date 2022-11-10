using NUnit.Framework;
using System.Threading.Tasks;
using Ofgem.Web.BUS.ConsentPortal.WebApp.Pages.Consent;

namespace Ofgem.Web.BUS.ConsentPortal.WebApp.UnitTests.Pages.Consent
{
    [TestFixture]
    public class SessionExpiredTests
    {
        [Test]
        public async Task OnGet_ReturnPageResult()
        {
            //Arrange
            var _systemUnderTest = new SessionExpiredModel();

            //Act & Assert
            Assert.DoesNotThrow(_systemUnderTest.OnGet);
        }
    }
}