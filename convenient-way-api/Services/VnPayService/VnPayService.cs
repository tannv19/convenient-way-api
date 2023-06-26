using Microsoft.AspNetCore.Mvc;
using ship_convenient.Core.CoreModel;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Helper;
using ship_convenient.Helper.VnPay;
using ship_convenient.Model.VnPayModel;
using ship_convenient.Services.GenericService;
using System.Net;
using System.Text;

namespace ship_convenient.Services.VnPayService
{
    public class VnPayService : GenericService<VnPayService>, IVnPayService
    {
        private readonly IConfiguration _confiuration;
        public const string VERSION = "2.1.0";
        private SortedList<string, string> _requestData =
            new SortedList<string, string>(new VnPayCompare());
        private SortedList<string, string> _responseData =
            new SortedList<string, string>(new VnPayCompare());
        public VnPayService(ILogger<VnPayService> logger, IUnitOfWork unitOfWork, IConfiguration configuration) : base(logger, unitOfWork)
        {
            _confiuration = configuration;
        }

        public void AddRequest(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _requestData.Add(key, value);
            }
        }

        public void AddResponse(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _responseData.Add(key, value);
            }
        }

        public Task<ApiResponse<string>> CreateRequestUrl(PaymentVnPayModel model)
        {
            ApiResponse<string> response = new();
            string baseUrl = _confiuration["VnPay:Url"];
            string vnp_HashSecret = _confiuration["VnPay:HashSecret"];
            StringBuilder data = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in _requestData)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }

            string querystring = data.ToString();

            baseUrl += "?" + querystring;
            string signData = querystring;
            if (signData.Length > 0)
            {
                signData = signData.Remove(data.Length - 1, 1);
            }

            string vnp_SecureHash = Utils.HmacSHA512(vnp_HashSecret, signData);
            baseUrl += "vnp_SecureHash=" + vnp_SecureHash;

            response.ToSuccessResponse(baseUrl, "Tạo thanh toán thành công");
            return Task.FromResult(response);
        }

        public string GetResponseDataKey(string key)
        {
            string? retValue;
            if (_responseData.TryGetValue(key, out retValue))
            {
                return retValue;
            }
            else
            {
                return string.Empty;
            }
        }

    

        private string GetResponseData()
        {
            StringBuilder data = new StringBuilder();
            if (_responseData.ContainsKey("VnpSecureHashType"))
            {
                _responseData.Remove("VnpSecureHashType");
            }

            if (_responseData.ContainsKey("VnpSecureHash"))
            {
                _responseData.Remove("VnpSecureHash");
            }

            foreach (KeyValuePair<string, string> kv in _responseData)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    data.Append(WebUtility.UrlEncode("vnp_" + kv.Key.Substring(3)) + "=" +
                                WebUtility.UrlEncode(kv.Value) + "&");
                }
            }

            //remove last '&'
            if (data.Length > 0)
            {
                data.Remove(data.Length - 1, 1);
            }

            return data.ToString();
        }

        public bool ValidateSignature(string inputHash, string secretKey)
        {
            string rspRaw = GetResponseData();
            string myChecksum = Utils.HmacSHA512(secretKey, rspRaw);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
