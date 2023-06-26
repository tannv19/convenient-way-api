using ship_convenient.Entities;

namespace ship_convenient.Model.ConfigModel
{
    public class CreateConfigPriceModel
    {
        public int Level { get; set; }
        public int Price { get; set; }
        public int MinDistance { get; set; }
        public int MaxDistance { get; set; }

        public ConfigPrice ToEntity() {
            ConfigPrice entity = new();
            entity.Level = this.Level;
            entity.Price = this.Price;
            entity.MinDistance = this.MinDistance;
            entity.MaxDistance = this.MaxDistance;
            return entity;
        }
    }
}
