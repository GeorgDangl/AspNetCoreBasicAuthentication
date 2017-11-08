using System;
using Microsoft.AspNetCore.Authentication;

namespace AspNetCoreBasicAuthentication.Authentication
{
    public static class HttpBasicAuthenticationExtensions
    {
        public static AuthenticationBuilder AddHttpBasicAuthentication(this AuthenticationBuilder builder, Action<AuthenticationSchemeOptions> configureOptions = null)
        {
            configureOptions = configureOptions ?? (o => {}); // Empty config action if none is provided
            return builder.AddScheme<AuthenticationSchemeOptions, HttpBasicAuthenticationHandler>("Basic", "Http Basic", configureOptions);
        }
    }
}
