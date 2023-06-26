using ship_convenient.Entities;

namespace ship_convenient.Helper.SuggestPackageHelper
{
    public class DistancePackageModel
    {
        public double Distance { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public PointType PointType { get; set; }
        public Package? Package { get; set; }

        public DistancePackageModel()
        {

        }
        public DistancePackageModel(double distance,PointType pointType ,double latitude,double longitude,Package? package)
        {
            Distance = distance;
            Latitude = latitude;
            Longitude = longitude;
            PointType = pointType;
            Package = package;
        }

        public DistancePackageModel(double distance, PointType pointType, double latitude, double longitude,string locationName, Package? package)
        {
            Distance = distance;
            Latitude = latitude;
            Longitude = longitude;
            LocationName = locationName;
            PointType = pointType;
            Package = package;
        }
    }

    public enum PointType {
        Start,
        Destination,
        Deliver
    }
}
