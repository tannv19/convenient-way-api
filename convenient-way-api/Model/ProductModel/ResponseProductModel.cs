
namespace unitofwork_core.Model.ProductModel
{
    public class ResponseProductModel
    {
        public int Price { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid PackageId { get; set; }
    }
}