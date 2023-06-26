using Microsoft.AspNetCore.Mvc;
using ship_convenient.Core.CoreModel;
using ship_convenient.Model.VnPayModel;

namespace ship_convenient.Services.VnPayService
{
    public interface IVnPayService
    {
        void AddRequest(string key, string value);
        void AddResponse(string key, string value);
        string GetResponseDataKey(string key);
        bool ValidateSignature(string inputHash, string secretKey);
        Task<ApiResponse<string>> CreateRequestUrl(PaymentVnPayModel model);
    }
}
