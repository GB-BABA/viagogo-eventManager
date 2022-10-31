namespace Viagogo.EventManager.Data.Models;

public class EventWithProximityRanking : Event
{
    public int ProximityRanking { get; set; }

    public EventWithProximityRanking(string name, string city, int proximityRanking)
    {

    }
}