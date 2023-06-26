namespace ship_convenient.Helper
{
    public class ParseHelper
    {
        public static int RoundedToInt(double input) {
            return Convert.ToInt32(Math.Round(input));
        }

        public static int RoundedToInt(decimal input)
        {
            return Convert.ToInt32(Math.Round(input));
        }
    }
}
