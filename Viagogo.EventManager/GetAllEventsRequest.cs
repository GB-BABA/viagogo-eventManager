using Viagogo.EventManager.Data.Enums;

namespace Viagogo.EventManager.Data.ViewModels;

public class GetAllEventsRequest
{
    public int PageLimit { get; set; }

    public int PageIndex { get; set; }

    public string? OrderByKey { get; set; }
    public OrderByEnum OrderBy { get; set; } = OrderByEnum.ASC;
}

