using System.Security.Cryptography;
using System.Text;

namespace ship_convenient.Helper
{
    public class Utils
    {
        public static double GetPercent(string text)
        {
            double percent = double.Parse(text) / 100;
            if (percent < 0 || percent > 1) throw new ArgumentException("Percent have from 0 to 1");
            return percent;
        }

        public static String HmacSHA512(string key, String inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }

        public static bool CompareEqualTime(DateTime time1, DateTime time2)
        {
            bool isEqualYear = time1.Year == time2.Year;
            bool isEqualMonth = time1.Month == time2.Month;
            bool isEqualDay = time1.Day == time2.Day;
            bool isEqualHour = time1.Hour == time2.Hour;
            bool isEqualMinute = time1.Minute == time2.Minute;
            return isEqualYear && isEqualMonth && isEqualDay && isEqualHour && isEqualMinute;
        }

        public static bool CompareEqualTimeDate(DateTime time1, DateTime time2)
        {
            bool isEqualYear = time1.Year == time2.Year;
            bool isEqualMonth = time1.Month == time2.Month;
            bool isEqualDay = time1.Day == time2.Day;
            return isEqualYear && isEqualMonth && isEqualDay;
        }

        public static bool CompareEqualTimeHour(DateTime time1, DateTime time2)
        {
            bool isEqualYear = time1.Year == time2.Year;
            bool isEqualMonth = time1.Month == time2.Month;
            bool isEqualDay = time1.Day == time2.Day;
            bool isEqualHour = time1.Hour == time2.Hour;
            return isEqualYear && isEqualMonth && isEqualDay && isEqualHour;
        }


        public static bool IsTimeToday(DateTime time)
        {
            bool isEqualYear = time.Year == DateTime.UtcNow.Year;
            bool isEqualMonth = time.Month == DateTime.UtcNow.Month;
            bool isEqualDay = time.Day == DateTime.UtcNow.Day;
            return isEqualYear && isEqualMonth && isEqualDay;
        }
    }
}
