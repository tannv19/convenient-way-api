namespace ship_convenient.Model.ConfigUserModel
{
    public class UpdateUserConfigModel
    {
        public Guid AccountId { get; set; }
        public string ConfigName { get; set; } = string.Empty;
        public string ConfigValue { get; set; } = string.Empty;
    }
}
