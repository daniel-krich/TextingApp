using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Subscriptions;
using HotChocolate.AspNetCore.Subscriptions.Messages;
using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using TextAppApi.Authentication;

namespace TextAppApi.Subscriptions
{
    public class SubscriptionAuthMiddleware : ISocketSessionInterceptor
    {
        public ValueTask<ConnectionStatus> OnConnectAsync(ISocketConnection connection, InitializeConnectionMessage message, CancellationToken cancellationToken)
        {
            try
            {
                //var jwtHeader = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9zaWQiOiI2MjIyOTU2YjBhNGU3ZWMxMDhmMWE2YzIiLCJleHAiOjE2NDcwMzg0NDMsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NDQzMTAiLCJhdWQiOiJodHRwOi8vbG9jYWxob3N0OjMwMDAifQ.WK7zoy8kJlVkgsYM85vOFhJGtecppemkzbWZ4PbZd_w";
                var jwtHeader = message.Payload["Authorization"] as string;

                if (string.IsNullOrEmpty(jwtHeader) || !jwtHeader.StartsWith("Bearer "))
                    return new ValueTask<ConnectionStatus>(ConnectionStatus.Reject("Unauthorized"));

                var token = jwtHeader.Replace("Bearer ", "");

                var tokenService = connection.HttpContext.RequestServices.GetRequiredService<ITokenService>();

                var claims = tokenService.GetPrincipalFromToken(token);
                if (claims == null)
                    return new ValueTask<ConnectionStatus>(ConnectionStatus.Reject("Unauthorized (invalid token)"));

                connection.HttpContext.User = claims;

                return new ValueTask<ConnectionStatus>(ConnectionStatus.Accept());
            }
            catch (Exception ex)
            {
                return new ValueTask<ConnectionStatus>(ConnectionStatus.Reject(ex.Message));
            }
        }

        public ValueTask OnCloseAsync(ISocketConnection connection, CancellationToken cancellationToken)
        {
            return new ValueTask();
        }

        public ValueTask OnRequestAsync(ISocketConnection connection, IQueryRequestBuilder requestBuilder, CancellationToken cancellationToken)
        {
            return new ValueTask();
        }
    }
}
