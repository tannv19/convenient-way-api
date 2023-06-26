namespace ship_convenient.Model.ConfigModel
{
    public class UpdateConfigModel
    {
        public string Name { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public Guid ModifiedBy { get; set; }
    }
}
