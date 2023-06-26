namespace ship_convenient.Entities
{
    public class ConfigPrice: BaseEntity
    {
        public int Level { get; set; }
        public int Price { get; set; }
        public int MinDistance { get; set; }
        public int MaxDistance { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
