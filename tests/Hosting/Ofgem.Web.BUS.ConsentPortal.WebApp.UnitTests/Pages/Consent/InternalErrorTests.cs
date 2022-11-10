using NUnit.Framework;
using System.Threading.Tasks;
using Ofgem.Web.BUS.ConsentPortal.WebApp.Pages.Shared;

namespace Ofgem.Web.BUS.ConsentPortal.WebApp.UnitTests.Pages.Consent
{
    [TestFixture]
    public class InternalErrorTests
    {
        [Test]
        public async Task OnGet_ReturnPageResult()
        {
            //Arrange
            var _systemUnderTest = new InternalErrorModel();

            //Act & Assert
            Assert.DoesNotThrow(_systemUnderTest.OnGet);
        }

        [Test]
        public async Task OnPost_ReturnPageResult()
        {
            //Arrange
            var _systemUnderTest = new InternalErrorModel();

            //Act & Assert
            Assert.DoesNotThrow(_systemUnderTest.OnPost);
        }
    }
}
