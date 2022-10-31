using Viagogo.EventManager.Data.Models;

namespace Viagogo.EventManager.Helpers;

public static class EmailSenderHelper
{
    // You do not need to know how these methods work
    internal static void AddToEmail(Customer c, Event e, int? price = null)
    {
        var distance = GeolocationHelper.GetDistance(c.City, e.City);
        Console.Out.WriteLine($"{c.Name}: {e.Name} in {e.City}"
        + (distance > 0 ? $" ({distance} miles away)" : "")
        + (price.HasValue ? $" for ${price}" : ""));
    }
}

