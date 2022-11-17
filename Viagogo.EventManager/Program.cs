using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Viagogo;

namespace Viagogo;

public class Program
{
    static List<Event> events = new()
    {
        new Event { Name = "Phantom of the Opera", City = "New York" },
        new Event { Name = "Metallica", City = "Los Angeles" },
        new Event { Name = "Metallica", City = "New York" },
        new Event { Name = "Metallica", City = "Boston" },
        new Event { Name = "LadyGaGa", City = "New York" },
        new Event { Name = "LadyGaGa", City = "Boston" },
        new Event { Name = "LadyGaGa", City = "Chicago" },
        new Event { Name = "LadyGaGa", City = "San Francisco" },
        new Event { Name = "LadyGaGa", City = "Washington" }
    };

    static List<Customer> customers = new List<Customer>{
    new Customer{ Name = "Nathan", City = "New York"},
    new Customer{ Name = "Bob", City = "Boston"},
    new Customer{ Name = "Cindy", City = "Chicago"},
    new Customer{ Name = "Lisa", City = "Los Angeles"}};

    static void Main(string[] args)
    {
        try
        {
            //Q1. Find out all events that are in cities of customer then add to email.
            /*
             FIlter events by customer's city using a LAMBDA expresions
             */

            List<Event>? currentCustomerEvents = default;

            if (customers is null || !customers.Any())
                throw new Exception("Model is required");

            foreach (var customer in customers)
            {
                currentCustomerEvents = events.AsParallel().Where(@event => @event.City.Equals(customer.City, StringComparison.InvariantCultureIgnoreCase)).ToList();

                if (currentCustomerEvents is null || !currentCustomerEvents.Any())
                {
                    Console.WriteLine("There are no events in your area");
                    continue;
                }

                SendCustomerEventNotificationEmail(customer, currentCustomerEvents);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// List events
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    static IEnumerable<Event> GetAllEvents(GetAllEventsRequest? request = default)
    {

        var query = events.AsParallel().Select(@event => new Event
        {
            Name = @event.Name,
            City = @event.City,
            Price = GetPrice(@event)
        });

        if (request is not null && !string.IsNullOrEmpty(request.OrderByKey))
        {
            //Add price filter option
            if (request.OrderByKey == "price")
            {
                query = (request.OrderBy == OrderByEnum.ASC) ?
                    query.OrderBy(evt => evt.Price) :
                    query.OrderByDescending(evt => evt.Price);
            }
        }

        var data = query.ToList();

        foreach (var d in data)
            Console.WriteLine($"Name: {d.Name}, City: {d.City}, Price: {d.Price}");

        return data;
    }


    /// <summary>
    /// Sends an email of top events closest to customer's city
    /// </summary>
    /// <param name="events"></param>
    /// <param name="customers"></param>
    /// <param name="limit"></param>
    static void SendEventsClosestToCustomerCity(int limit = 5, params Customer[] customers)
    {
        var customerEvents = FindClosestEventsToCustomer(limit, customers, events);

        var customersGroupedByName = customers.ToDictionary(customer => customer.Name, customer => customer);

        foreach (var customerEvent in customerEvents)
        {
            if (!customersGroupedByName.TryGetValue(customerEvent.Key, out Customer? customer))
                continue;

            SendCustomerEventNotificationEmail(customer, customerEvent.Value);
        }
    }

    // You do not need to know how these methods work
    static void AddToEmail(Customer c, Event e, int? price = null)
    {
        var distance = GetDistance(c.City, e.City);
        Console.Out.WriteLine($"{c.Name}: {e.Name} in {e.City}"
        + (distance > 0 ? $" ({distance} miles away)" : "")
        + (price.HasValue ? $" for ${price}" : ""));
    }

    static int GetDistance(string fromCity, string toCity)
    {
        return AlphebiticalDistance(fromCity, toCity);
    }

    private static int AlphebiticalDistance(string s, string t)
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

    static Dictionary<string, IEnumerable<Event>> FindClosestEventsToCustomer(int limit, Customer[] customers, List<Event> events)
    {
        Dictionary<string, IEnumerable<Event>> customerEvents = new Dictionary<string, IEnumerable<Event>>();
        List<Event>? currentClosestEvents = default;

        Dictionary<string, List<Event>> eventsGroupedByCity = events.AsParallel().Select(evt => new Event
        {
            Name = evt.Name,
            City = evt.City,
            Price = GetPrice(evt)
        }).GroupBy(@event => @event.City).ToDictionary(@event => @event.Key, @event => @event.ToList());

        foreach (var customer in customers)
        {
            currentClosestEvents = eventsGroupedByCity
                .Select(@event => new
                {
                    ProximityRanking = GetDistance(customer.City, @event.Key),
                    Events = @event.Value
                })
                ?.Where(@event => @event.ProximityRanking > -1) //prevents events with failed distance look-up from showing up in result
                ?.OrderBy(@event => @event.ProximityRanking)
                ?.SelectMany(@event => @event.Events)
                ?.Take(limit)
                .ToList();

            customerEvents.TryAdd(customer.Name, currentClosestEvents ?? new List<Event>());
        }

        return customerEvents;
    }

    /// <summary>
    /// Sends customer event notification happening in customer's city
    /// </summary>
    /// <param name="customer"></param>
    /// <param name="events"></param>
    private static void SendCustomerEventNotificationEmail(Customer customer, IEnumerable<Event> events)
    {
        // 1. TASK
        foreach (var @event in events)
            AddToEmail(customer, @event);
    }

    static int GetPrice(Event e)
    {
        return (AlphebiticalDistance(e.City, "") + AlphebiticalDistance(e.Name, "")) / 10;
    }

    public class EventWithProximityRanking : Event
    {
        public int ProximityRanking { get; set; }

        public EventWithProximityRanking(string name, string city, int proximityRanking)
        {

        }
    }

    public class Event
    {
        public string Name { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public int Price { get; set; }
    }

    public class Customer
    {
        public string Name { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;
    }

    public class GetAllEventsRequest
    {
        public int PageLimit { get; set; }

        public int PageIndex { get; set; }

        public string? OrderByKey { get; set; }
        public OrderByEnum OrderBy { get; set; } = OrderByEnum.ASC;
    }

    public enum OrderByEnum
    {
        ASC,
        DESC
    }
}