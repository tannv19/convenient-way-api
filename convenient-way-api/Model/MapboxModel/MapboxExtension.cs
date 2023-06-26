using GeoCoordinatePortable;

namespace ship_convenient.Model.MapboxModel
{
    public static class MapboxExtension
    {
        public static CoordinateApp ToCoordinate(this GeoCoordinate point)
        {
            CoordinateApp coordinateApp = new CoordinateApp();
            coordinateApp.Latitude = point.Latitude;
            coordinateApp.Longitude = point.Longitude;
            return coordinateApp;
        }
    }
}
