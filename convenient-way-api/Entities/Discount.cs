namespace ship_convenient.Entities
{
    public class Discount : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public int Value { get; set; }
        public string Code { get; set; } = string.Empty;

        #region Relationship
        public List<Package> Packages { get; set; }
        #endregion

        public Discount()
        {
            Packages = new List<Package>();
        }
    }
}
