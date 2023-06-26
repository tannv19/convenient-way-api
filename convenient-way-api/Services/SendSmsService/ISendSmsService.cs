using Twilio.Rest.Api.V2010.Account;

namespace ship_convenient.Services.SendSmsService
{
    public interface ISendSMSService
    {
        public Task<MessageResource> SendSmsOtp();

    }
}
