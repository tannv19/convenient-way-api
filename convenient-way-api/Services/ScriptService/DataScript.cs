using System.Linq;

namespace ship_convenient.Services.ScriptService
{
    public class DataScript
    {
        public static List<string> RandomFirstName()
        {
            List<string> firstNames = new List<string> {
                "Tài", "Thắng", "Hậu", "Huy", "Dung", "Thanh" , "Trâm", "Thảo", "Trân" , "Hoa", "Tú" , "Quất" , "Sơn" , "Hà",
                "Duyên", "Châu", "Vi", "Phương", "Bảo", "Phương"
            };
            return firstNames;
        }

        public static List<string> RandomLastName()
        {
            List<string> firstNames = new List<string> {
                "Nguyễn", "Lê", "Võ", "Lục", "Mạc", "Văn" , "Đinh", "Đoàn", "Lục" };
            return firstNames;
        }

        public static List<string> RandomPhone()
        {
            List<string> phones = new List<string>
            {
                "0354767689", "0396823286", "0865757476", "0388045086", "0367742279", "0372855005",
                "0359022379", "0867956588", "0978674737", "0867574689", "0378065986", "0333988139",
                "0975250439", "0372850979", "0868345574", "0346325779", "0372940990", "0364020486",
                "0966325732", "0387807768", "0344240688", "0328181195"
            };
            return phones;
        }

        public static List<string> RandomLocationsName()
        {
            return RandomLocationsFull().Select(location => location.Name).ToList();
        }

        public static List<Location> RandomLocationsFull()
        {
            List<Location> locations = new List<Location> {
                new Location{ Name = "32 đường Lê Lợi, phường Bến Thành, quận 1, TP. Hồ Chí Minh",
                    Latitude =  10.77446184664321, Longitude = 106.70054199461353},
                new Location{ Name = "Cảng hàng không Quốc tế Tân Sơn Nhất", Latitude = 10.818621161556333, Longitude= 106.65882449461415},
                new Location{ Name = "ÆON MALL Tân Phú Celadon", Latitude = 10.801754970457324, Longitude= 106.61748515043774},
                new Location{ Name = "Trường THPT Tây Thạnh", Latitude = 10.814584463687861, Longitude= 106.62170240810828},
                new Location{ Name = "Điểm Nhấn Group, 84 Nguyễn Háo Vĩnh, Tân Quý, Tân Phú, Thành phố Hồ Chí Minh 700000, Việt Nam",
                    Latitude = 10.793429410315342, Longitude = 106.62492818111954},
                new Location{ Name = "Công viên Văn hóa Lê Thị Riêng", Latitude = 10.784726876133202, Longitude= 106.66465181975546},
                new Location{ Name = "Công viên nước Đầm Sen", Latitude = 10.769010236581662, Longitude= 106.63598283608705},
                new Location{ Name = "Sun Palace Trung Tâm Tiệc Cưới & Hội Nghị", Latitude = 10.748813817037725, Longitude= 106.62576812359941},
                new Location{ Name = "Nhà hàng Chim Rừng, PJX7+8W9, Bình Trị Đông B, Bình Tân, Thành phố Hồ Chí Minh, Việt Nam",
                    Latitude = 10.748497501513569, Longitude =106.61489743878941},
                new Location{ Name = "Trường THPT Nguyễn Văn Linh, Khu dân cư Phú Lợi, phường 7, Quận 8, Thành phố Hồ Chí Minh",
                    Latitude = 10.702345394813634, Longitude=  106.62131981393073},
                new Location{ Name = "Nhà thờ Đức Bà Sài Gòn, 01 Công xã Paris, Bến Nghé, Quận 1, Thành phố Hồ Chí Minh 70000, Việt Nam",
                    Latitude = 10.779922502943554, Longitude= 106.69895452160226},
                new Location{ Name = "Chợ Hạnh Thông Tây",
                    Latitude = 10.835806827564992, Longitude= 106.6586323657791},
                new Location{ Name = "Trường THPT Trường Chinh, 1 DN11, Khu Phố 4, Quận 12, Thành phố Hồ Chí Minh, Việt Nam",
                    Latitude = 10.841540650699322, Longitude = 106.62492586947249},
                new Location{ Name = "Trường Đại học Lao động Xã hội - Cơ sở 2",
                    Latitude = 10.8671558816317, Longitude = 106.61791482345008},
                new Location{ Name = "Gà Hấp Hèm 34, 34 Nguyễn Thị Thử, Xuân Thới Sơn, Hóc Môn, Thành phố Hồ Chí Minh, Việt Nam",
                    Latitude =10.868030044633432, Longitude = 106.5790905216033},
                new Location{ Name = "74/2B Phạm Văn Chiêu, phường 14, Gò Vấp, Thành phố Hồ Chí Minh, Việt Nam",
                    Latitude =10.852002685081368, Longitude = 106.65066313694406},
                new Location{ Name = "ĐÈN LED THỐNG NGUYỄN, 237/21 Phạm Văn Chiêu, Phường 14, Gò Vấp, Thành phố Hồ Chí Minh, Việt Nam",
                    Latitude =10.855329944111109, Longitude = 106.64818080047358},
                new Location{ Name = "The Coffee House - Phan Huy Ích, 403 Phan Huy Ích, Phường 14, Gò Vấp, Thành phố Hồ Chí Minh 700000, Việt Nam",
                    Latitude =10.849597841458253, Longitude = 106.64036577775039},
                new Location{ Name = "HỈN COFFEE, 34F6 DN6, Đông Hưng Thuận, Quận 12, Thành phố Hồ Chí Minh, Việt Nam",
                    Latitude = 10.845442378781687, Longitude = 106.62399700897241},
                new Location{ Name = "Giáo xứ Tân Hưng, 1 QL1A, Tân Thới Hiệp, Quận 12, Thành phố Hồ Chí Minh, Việt Nam",
                    Latitude = 10.856645950385822, Longitude = 106.63854306134574},
                new Location{ Name = "69 Đông Hưng Thuận 19, Đông Hưng Thuận, Quận 12, Thành phố Hồ Chí Minh, Việt Nam",
                    Latitude = 10.848980015245798, Longitude = 106.63657629047829},
            };

            return locations;
        }

        public static Location GetLocationFromName(string name) {
            List<Location> locations = RandomLocationsFull();
            Location location = locations.FirstOrDefault(x => x.Name == name || x.Name + "[script]" == name);
            return location;
        }

        public static List<string> RandomProduct()
        {
            List<string> products = new List<string>  {
                "Bánh mì", "Bánh tráng", "Hoa", "Kẹo dừa", "Bàn", "Ghế", "Chén", "Tô", "Điện thoại", "Loa", "Đèn", "Laptop" };
            return products;
        }


    }
}
