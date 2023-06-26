using ship_convenient.Constants.ConfigConstant;
using ship_convenient.Model.ConfigModel;

namespace ship_convenient.Entities
{
    public class ConfigApp: BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public string Type { get; set; } = TypeOfConfig.DEFAULT;
        public string Description { get; set; } = string.Empty;
        public Guid ModifiedBy { get; set; }
        public DateTime ModifiedAt { get; set; }

        public ResponseConfigModel ToResponseModel() {
            return new ResponseConfigModel
            {
                Id = Id,
                Name = Name,
                Note = Note,
                Type = Type,
                ModifiedBy = ModifiedBy,
                ModifiedAt = ModifiedAt
            };
        }
    }
}
