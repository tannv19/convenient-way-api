using unitofwork_core.Model.ProductModel;

namespace ship_convenient.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public int Price { get; set; }
        public string Description { get; set; } = string.Empty;
        #region Relationship
        public Guid PackageId { get; set; }
        public Package? Package { get; set; }
        #endregion

        public ResponseProductModel ToResponseModel()
        {
            ResponseProductModel model = new ResponseProductModel();
            model.Name = this.Name;
            model.Price = this.Price;
            model.Description = this.Description;
            return model;
        }
    }
}
