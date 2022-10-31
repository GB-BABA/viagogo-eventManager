namespace Viagogo.EventManager.Helpers;

internal static class GeolocationHelper
{
    internal static int GetDistance(string fromCity, string toCity)
    {
        return AlphebiticalDistance(fromCity, toCity);
    }

    internal static int AlphebiticalDistance(string s, string t)
    {
        try
        {

            if (s.Equals(t, StringComparison.InvariantCultureIgnoreCase))
                return 0;

            var result = 0;
            int i;
            for (i = 0; i < Math.Min(s.Length, t.Length); i++)
            {
                // Console.Out.WriteLine($"loop 1 i={i} {s.Length} {t.Length}");
                result += Math.Abs(s[i] - t[i]);
            }
            for (; i < Math.Max(s.Length, t.Length); i++)
            {
                // Console.Out.WriteLine($"loop 2 i={i} {s.Length} {t.Length}");
                result += s.Length > t.Length ? s[i] : t[i];
            }
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return -1; //Returns negative number when distance can not be calculated
        }
    }
}