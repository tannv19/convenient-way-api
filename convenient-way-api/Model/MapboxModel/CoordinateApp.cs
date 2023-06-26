namespace ship_convenient.Model.MapboxModel
{
    public class CoordinateApp
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public CoordinateApp()
        {

        }
        public CoordinateApp(double Longitude, double Latitude)
        {
            this.Longitude = Longitude;
            this.Latitude = Latitude;
        }
        
    }
}
