using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace eShop.Services.ShoppingCartAPI.Utility
{
    public class BackEndApiAuthenticationHttpClientHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BackEndApiAuthenticationHttpClientHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken)
        {
            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer",token);
            return await base.SendAsync(requestMessage, cancellationToken);
        }
    }
}
