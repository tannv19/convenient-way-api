using ship_convenient.Model.UserModel;

namespace ship_convenient.Model.AuthorizeModel
{
    public class ResponseLoginModel
    {
        public string? Token { get; set; }
        public ResponseAccountModel? Account { get; set; }
    }
}
