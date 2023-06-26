namespace ship_convenient.Entities
{
    public class Vehicle : BaseEntity
    {
        public bool IsActive { get; set; }
        public string Name { get; set; } = string.Empty;
        public double MaxVolume { get; set; }
        public double MaxSize { get; set; }
        #region Relationship
        public Guid InfoUserId { get; set; }
        public InfoUser? InfoUser { get; set; }
        #endregion
    }
}
