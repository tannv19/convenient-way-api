using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ship_convenient.Core.CoreModel;
using ship_convenient.Core.IRepository;
using ship_convenient.Core.UnitOfWork;
using ship_convenient.Entities;
using ship_convenient.Model.VnPayModel;
using ship_convenient.Services.AccountService;
using ship_convenient.Services.VnPayService;
using unitofwork_core.Constant.Transaction;

namespace ship_convenient.Controllers
{
    [Route("api/v1.0/[controller]")]
    public class VnPayController : BaseApiController
    {
        private readonly IConfiguration _configuration;
        private readonly IVnPayService _vnPayService;
        private readonly IAccountService _accountService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRepository _accountRepo;
        private readonly ITransactionRepository _transRepo;
        private readonly IDepositRepository _depositRepo;
        public VnPayController(IConfiguration configuration, IVnPayService vnPayService, IAccountService accountService, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _vnPayService = vnPayService;
            _accountService = accountService;
            _unitOfWork = unitOfWork;
            _accountRepo = unitOfWork.Accounts;
            _transRepo = unitOfWork.Transactions;
            _depositRepo = unitOfWork.Deposits;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaymentVnPayModel model)
        {
            string returnUrl = _configuration["VnPay:ReturnPath"];
            string tmnCode = _configuration["VnPay:TmnCode"];

            _vnPayService.AddRequest("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.0.0
            _vnPayService.AddRequest("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            _vnPayService.AddRequest("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            _vnPayService.AddRequest("vnp_Amount", model.Amount + "00"); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            _vnPayService.AddRequest("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            _vnPayService.AddRequest("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            _vnPayService.AddRequest("vnp_IpAddr", model.Ip); //Địa chỉ IP của khách hàng thực hiện giao dịch
            _vnPayService.AddRequest("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            string orderInfo = model.AccountId.ToString();
            if (model.ReturnUrl != string.Empty) {
                orderInfo += $"--{model.ReturnUrl}";
            }
            _vnPayService.AddRequest("vnp_OrderInfo", orderInfo); //Thông tin mô tả nội dung thanh toán
            _vnPayService.AddRequest("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            _vnPayService.AddRequest("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            _vnPayService.AddRequest("vnp_TxnRef", DateTime.Now.Ticks.ToString()); //mã hóa đơn
            _vnPayService.AddRequest("vnp_ExpireDate", DateTime.Now.AddHours(7).AddMinutes(10).ToString("yyyyMMddHHmmss")); //Thời gian kết thúc thanh toán
            ApiResponse<string> paymentUrl = await _vnPayService.CreateRequestUrl(model);

            return SendResponse(paymentUrl);
        }

        [HttpGet("payment-confirm")]
        public async Task<IActionResult> Confirm()
        {
            try
            {
                string returnUrl = _configuration["VnPay:ReturnPathResult"];
                float amount = 0;
                string status = "failed";
                Guid transactionId = Guid.NewGuid();
                if (Request.Query.Count > 0)
                {
                    string vnp_HashSecret = _configuration["VnPay:HashSecret"]; //Secret key
                    var vnpayData = Request.Query;
                    foreach (string s in vnpayData.Keys)
                    {
                        //get all querystring data
                        if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                        {
                            _vnPayService.AddResponse(s, vnpayData[s]);
                        }
                    }
                    //Lay danh sach tham so tra ve tu VNPAY
                    //vnp_TxnRef: Ma don hang merchant gui VNPAY tai command=pay    
                    //vnp_TransactionNo: Ma GD tai he thong VNPAY
                    //vnp_ResponseCode:Response code from VNPAY: 00: Thanh cong, Khac 00: Xem tai lieu
                    //vnp_SecureHash: HmacSHA512 cua du lieu tra ve

                    long orderId = Convert.ToInt64(_vnPayService.GetResponseDataKey("vnp_TxnRef"));
                    float vnp_Amount = Convert.ToInt64(_vnPayService.GetResponseDataKey("vnp_Amount")) / 100;
                    amount = vnp_Amount;
                    string vnpayTranId = _vnPayService.GetResponseDataKey("vnp_TransactionNo");
                    string vnp_ResponseCode = _vnPayService.GetResponseDataKey("vnp_ResponseCode");
                    string vnp_TransactionStatus = _vnPayService.GetResponseDataKey("vnp_TransactionStatus");
                    string vnp_SecureHash = Request.Query["vnp_SecureHash"];
                    bool checkSignature = _vnPayService.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                    string[] vnp_OrderInfo = _vnPayService.GetResponseDataKey("vnp_OrderInfo").Split("--");
                    Guid accountId = Guid.Parse(vnp_OrderInfo[0]);
                    if (vnp_OrderInfo.Length == 2)
                    {
                        returnUrl = vnp_OrderInfo[1];
                    }
                    Account? account = await _accountRepo.GetByIdAsync(accountId, disableTracking: false);
                    //Cap nhat ket qua GD
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00" && account != null)
                    {
                        //Thanh toán thành công
                        account.Balance += (int)Math.Round(vnp_Amount);
                        Deposit deposit = new Deposit();
                        deposit.AccountId = accountId;
                        deposit.TransactionIdPartner = vnpayTranId;
                        deposit.Status = "SUCCESS";
                        deposit.PaymentMethod = "VNPAY";
                        deposit.Amount = (int)Math.Round(vnp_Amount);
                        Transaction transaction = new Transaction
                        {
                            Id = transactionId,
                            Title = TransactionTitle.VNPAY,
                            CoinExchange = (int)Math.Round(vnp_Amount),
                            TransactionType = "RECHARGE",
                            Status = "SUCCESS",
                            Description = "Nạp tiền thành công từ ví điện tử VNPAY",
                            AccountId = accountId,
                            BalanceWallet = account.Balance,
                        };
                        transactionId = transaction.Id;
                        deposit.Transactions.Add(transaction);
                        await _depositRepo.InsertAsync(deposit);
                        status = "success";
                    }
                    else
                    {
                        Deposit deposit = new Deposit();
                        deposit.AccountId = accountId;
                        deposit.TransactionIdPartner = vnpayTranId;
                        deposit.Status = "FAILED";
                        deposit.PaymentMethod = "VNPAY";
                        deposit.Amount = (int)Math.Round(vnp_Amount);
                        await _depositRepo.InsertAsync(deposit);
                    }
                    await _unitOfWork.CompleteAsync();
                    if (status == "failed")
                    {
                        return Redirect(returnUrl + "?status=" + status + "&amount=" + amount);
                    }
                    return Redirect(returnUrl + "?status=" + status + "&amount=" + amount
                            + "&transactionId=" + transactionId + "&createDate=" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                }

                return BadRequest("Có lỗi xảy ra!!");
            }
            catch (Exception e)
            {
                return BadRequest($"Có lỗi xảy ra!! {e.Message}");
            }
        }
    }

}
