using eShop.Web.Service.Base;

namespace eShop.Web.Service.IService
{
    public interface ITokenProvider : IScopedService
    {
        void SetToken(string token);
        string? GetToken();
        void ClearToken();
    }
}
