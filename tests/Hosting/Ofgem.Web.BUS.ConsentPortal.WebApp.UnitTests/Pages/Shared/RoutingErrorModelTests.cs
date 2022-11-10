using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using Ofgem.Web.BUS.ConsentPortal.WebApp.Pages.Shared;

namespace Ofgem.Web.BUS.ConsentPortal.WebApp.UnitTests.Pages.Dashboard;

[TestFixture]
public class RoutingErrorModelTests
{
    private RoutingErrorModel _systemUnderTest = null!;

    [TestCase(404)]
    [TestCase(500)]
    [TestCase(503)]
    public void Call_RoutingErrorPage_With_Known_Status_Code(int statusCode)
    {
        // Arrange
        _systemUnderTest = new RoutingErrorModel();

        // Act
        _systemUnderTest.OnGet(statusCode);

        // Assert
        using (new AssertionScope())
        {
            _systemUnderTest.ErrorTitle.Should().NotBeNullOrEmpty();
            _systemUnderTest.ErrorPageContent.Should().NotBeNullOrEmpty();
        }
    }

    [TestCase(400)]
    public void Call_RoutingErrorPage_With_StatusCode(int statusCode)
    {
        // Arrange
        _systemUnderTest = new RoutingErrorModel();

        // Act
        _systemUnderTest.OnGet(statusCode);

        // Assert
        using (new AssertionScope())
        {
            _systemUnderTest.ErrorTitle.Should().BeNullOrEmpty();
            _systemUnderTest.ErrorPageContent.Should().BeNullOrEmpty();
        }
    }
}