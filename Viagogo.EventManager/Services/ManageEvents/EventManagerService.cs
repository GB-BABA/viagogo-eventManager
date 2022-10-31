using Viagogo.EventManager.Data.Mock;
using Viagogo.EventManager.Data.Models;
using Viagogo.EventManager.Data.ViewModels;
using Viagogo.EventManager.Exceptions;
using Viagogo.EventManager.Helpers;

namespace Viagogo.EventManager.Services.ManageEvents;

public static class EventManagerService
{
    /// <summary>
    /// List events
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static IEnumerable<Event> GetAllEvents(GetAllEventsRequest? request = default)
    {
        List<Event> events = EventMockData.Events;

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
                query = (request.OrderBy == Data.Enums.OrderByEnum.ASC) ?
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
    /// Sends events happening in customer's city
    /// </summary>
    /// <param name="customers"></param>
    public static void SendEventsInCustomerCity(params Customer[] customers)
    {
        try
        {
            List<Event> events = EventMockData.Events;
            //Q1. Find out all events that are in cities of customer then add to email.
            /*
             FIlter events by customer's city using a LAMBDA expresions
             */

            List<Event>? currentCustomerEvents = default;

            if (customers is null || !customers.Any())
                throw new NoEventinCustomerLocationException();

            foreach (var customer in customers)
            {
                currentCustomerEvents = events.AsParallel().Where(@event => @event.City.Equals(customer.City, StringComparison.InvariantCultureIgnoreCase)).ToList();

                if (currentCustomerEvents is null || !currentCustomerEvents.Any())
                {
                    Console.WriteLine(NoEventinCustomerLocationException.DEFAULT_MESSAGE);
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
    /// Sends an email of top events closest to customer's city
    /// </summary>
    /// <param name="events"></param>
    /// <param name="customers"></param>
    /// <param name="limit"></param>
    public static void SendEventsClosestToCustomerCity(int limit = 5, params Customer[] customers)
    {
        List<Event> events = EventMockData.Events;
        var customerEvents = FindClosestEventsToCustomer(limit, customers, events);

        var customersGroupedByName = customers.ToDictionary(customer => customer.Name, customer => customer);

        foreach (var customerEvent in customerEvents)
        {
            if (!customersGroupedByName.TryGetValue(customerEvent.Key, out Customer? customer))
                continue;

            SendCustomerEventNotificationEmail(customer, customerEvent.Value);
        }
    }

    private static Dictionary<string, IEnumerable<Event>> FindClosestEventsToCustomer(int limit, Customer[] customers, List<Event> events)
    {
        Dictionary<string, IEnumerable<Event>> customerEvents = new Dictionary<string, IEnumerable<Event>>();
        List<Event>? currentClosestEvents = default;

        Dictionary<string, List<Event>> eventsGroupedByCity = events.AsParallel().Select(evt => new Event
        {
            Name = evt.Name,
            City = evt.City,
            Price = GetPrice(evt)
        })
            .GroupBy(@event => @event.City).ToDictionary(@event => @event.Key, @event => @event.ToList());

        foreach (var customer in customers)
        {
            currentClosestEvents = eventsGroupedByCity
                .Select(@event => new
                {
                    ProximityRanking = GeolocationHelper.GetDistance(customer.City, @event.Key),
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
            EmailSenderHelper.AddToEmail(customer, @event);
    }

    static int GetPrice(Event e)
    {
        return (GeolocationHelper.AlphebiticalDistance(e.City, "") + GeolocationHelper.AlphebiticalDistance(e.Name, "")) / 10;
    }
}