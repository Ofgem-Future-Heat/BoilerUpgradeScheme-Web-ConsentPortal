using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ofgem.Web.BUS.ConsentPortal.Core.Filters;
using Ofgem.Web.BUS.ConsentPortal.Core.Interfaces;

namespace Ofgem.Web.BUS.ConsentPortal.Core
{
    /// <summary>
    /// Extensions to add Core project services to the DI container.
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Adds Core project services to the DI container.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddServiceConfigurations(this IServiceCollection services, IConfiguration config)
        {
            var sessionTokenSecret = config["SessionTokenSecret"];

            services.AddTransient<IOwnerConsentService, OwnerConsentService>();
            services.AddTransient<ISessionHelper, SessionHelper>();

            services.AddTransient<ISessionAuthorizationService>(service => new SessionAuthorizationService(sessionTokenSecret));

            services.AddTransient<EmailTokenAuthorizeAttribute>();
            services.AddTransient<SessionTokenAuthorizeAttribute>();

            return services;
        }
    }
}
