using ship_convenient.Entities;
using ship_convenient.Model.RouteModel;

namespace ship_convenient.Model.UserModel
{
    public class ResponseInfoUserModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<ResponseRouteModel> Routes { get; set; } = new();
        public List<ResponseConfigUserModel> Configs { get; set; } = new();
    }
}
