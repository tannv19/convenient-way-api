namespace ship_convenient.Entities
{
    public class Role : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        #region Relationship
        public List<Account> Accounts { get; set; }
        #endregion
        public Role()
        {
            Accounts = new List<Account>();
        }
    }
}
