using Viagogo.EventManager.Data.Models;

namespace Viagogo.EventManager.Data.Mock;

public static class EventMockData
{
    public static List<Event> Events = new() {
        new Event{ Name = "Phantom of the Opera", City = "New York"},
        new Event{ Name = "Metallica", City = "Los Angeles"},
        new Event { Name = "Metallica", City = "New York" },
        new Event { Name = "Metallica", City = "Boston" },
        new Event { Name = "LadyGaGa", City = "New York" },
        new Event { Name = "LadyGaGa", City = "Boston" },
        new Event { Name = "LadyGaGa", City = "Chicago" },
        new Event { Name = "LadyGaGa", City = "San Francisco" },
        new Event { Name = "LadyGaGa", City = "Washington" }
    };

    public static Customer Customer = new() { Name = "Mr. Fake", City = "New York" };

    public static List<Customer> Customers = new() {
        new Customer{ Name = "Nathan", City = "New York"},
        new Customer{ Name = "Bob", City = "Boston"},
        new Customer{ Name = "Cindy", City = "Chicago"},
        new Customer{ Name = "Lisa", City = "Los Angeles"}
    };
}

