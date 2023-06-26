using Twilio;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace ship_convenient.Services.SendSmsService
{
    public class SendSmsService : ISendSMSService
    {
        private readonly IConfiguration _configuration;
        
        public SendSmsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Task<MessageResource> SendSmsOtp()
        {
            int min = 111111;
            int max = 999999;

            Random random = new Random();
            // string convertPhone = phone.Remove(0, 1);
            // string phoneAfterConvert = "+84" + convertPhone;
            string otp = random.Next(min, max).ToString();
            
            string accountSid = _configuration["Twilio:SID"];
            string authToken = _configuration["Twilio:AuthToken"];

            var client = new TwilioRestClient(accountSid, authToken);

            // Pass the client into the resource method
            var message = MessageResource.Create(
                to: new PhoneNumber("+84792905834"),
                from: new PhoneNumber("+15075007707"),
                body: "Hello from C#",
                client: client);

            return Task.FromResult(message);
        }
    }
}
