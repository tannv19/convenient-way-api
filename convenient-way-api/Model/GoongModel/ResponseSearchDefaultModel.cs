namespace ship_convenient.Model.GoongModel
{
    public class ResponseSearchDefaultModel
    {
        public string Name { get; set; } = string.Empty;
        public string PlaceId { get; set; } = string.Empty;

        public ResponseSearchDefaultModel()
        {

        }

        public ResponseSearchDefaultModel(ResponseSearchModel responseSearchModel)
        {
            this.Name = responseSearchModel.Name;
            this.PlaceId = responseSearchModel.PlaceId;
        }

        public ResponseSearchDefaultModel(string placeId, string name)
        {
            this.Name = name;
            this.PlaceId = placeId;
        }
    }
}
