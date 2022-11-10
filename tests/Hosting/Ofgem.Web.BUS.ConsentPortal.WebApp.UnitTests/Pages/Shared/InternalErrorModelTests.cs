using FluentAssertions.Execution;
using NUnit.Framework;
using Ofgem.Web.BUS.ConsentPortal.WebApp.Pages.Shared;

namespace Ofgem.Web.BUS.ConsentPortal.WebApp.UnitTests.Pages.Dashboard;

[TestFixture]
public class InternalErrorModelTests
{
    private InternalErrorModel _systemUnderTest = null!;

    [Test]
    public void Call_InternalErrorModel_OnGet()
    {
        // Arrange
        _systemUnderTest = new InternalErrorModel();

        // Act
        _systemUnderTest.OnGet();

        // Assert
        using (new AssertionScope())
        {
        }
    }

    [Test]
    public void Call_InternalErrorModel_OnPost()
    {
        // Arrange
        _systemUnderTest = new InternalErrorModel();

        // Act
        _systemUnderTest.OnPost();

        // Assert
        using (new AssertionScope())
        {
        }
    }
}