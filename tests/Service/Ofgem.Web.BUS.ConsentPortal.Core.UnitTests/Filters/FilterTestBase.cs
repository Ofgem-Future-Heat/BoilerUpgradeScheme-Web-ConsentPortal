using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;

namespace Ofgem.Web.BUS.ConsentPortal.Core.UnitTests.Filters
{
    public abstract class FilterTestBase
    {
        protected AuthorizationFilterContext GetTestFilterContext(HttpContext httpContext)
        {
            var actionContext = new ActionContext(httpContext,
                                                  new Microsoft.AspNetCore.Routing.RouteData(),
                                                  new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
            var filterContext = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata> { });

            return filterContext;
        }
    }
}
