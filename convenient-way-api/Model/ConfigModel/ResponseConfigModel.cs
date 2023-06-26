using ship_convenient.Constants.ConfigConstant;

namespace ship_convenient.Model.ConfigModel
{
    public class ResponseConfigModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public string Type { get; set; } = TypeOfConfig.DEFAULT;
        public List<ResponseConfigModel> configsChildren { get; set; } = new List<ResponseConfigModel>();
        public Guid ModifiedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
