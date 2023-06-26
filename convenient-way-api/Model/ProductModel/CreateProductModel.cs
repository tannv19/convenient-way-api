using System.Diagnostics.Contracts;
using ProductEntity = ship_convenient.Entities.Product;

namespace unitofwork_core.Model.ProductModel
{
    public class CreateProductModel
    {
        public string Name { get; set; } = string.Empty;
        public int Price { get; set; } = 0;
        public string Description { get; set; } = string.Empty;

        public ProductEntity ConvertToEntity()
        {
            ProductEntity pro = new ProductEntity();
            pro.Name = this.Name;
            pro.Price = this.Price;
            pro.Description = this.Description;
            return pro;
        }
    }
}