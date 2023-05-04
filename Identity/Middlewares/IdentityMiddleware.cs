﻿using Identity.Constants;
using Identity.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Identity.Middlewares
{
    public class IdentityMiddleware
    {
        private readonly IdentityOptions _options;
        private readonly IdentityFeatures _features;
        private readonly IdentityRoutes _routes;

        private readonly RequestDelegate _next;

        public IdentityMiddleware(RequestDelegate next, IOptions<IdentityOptions> options)
        {
            _next = next;
            _options = options.Value;
            _routes = options.Value.Routes;
            _features = options.Value.Features;
            SetupDefaultRoutesIfNeeded();
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (_features.IsAvailableRefreshToken && httpContext.Request.Path == _routes.RefreshRoute)
            {
                httpContext.Response.StatusCode = 200;
                await httpContext.Response.WriteAsync("Success");
            }
            else
            {
                await _next.Invoke(httpContext);
            }
        }


        private void SetupDefaultRoutesIfNeeded()
        {
            _routes.RefreshRoute = _routes.RefreshRoute ?? EndpointsConstants.RefreshEndpoint;
        }
    }
}
