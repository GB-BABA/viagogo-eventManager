using Viagogo.EventManager.Data.Mock;
using Viagogo.EventManager.Services.ManageEvents;

var customer = EventMockData.Customer;

#region Get events
EventManagerService.GetAllEvents(); //Return all events

//EventManagerService.GetAllEvents(request: new GetAllEventsRequest { OrderBy = OrderByEnum.DESC, OrderByKey = "price" }); //Return all events (order by price)

//EventManagerService.GetAllEvents(request: new GetAllEventsRequest { OrderBy = OrderByEnum.ASC, OrderByKey = "price" }); //Return all events (order by price)
#endregion

////Send events in customer city
EventManagerService.SendEventsInCustomerCity(customer);

////Sends customer 5 events close to their city 
EventManagerService.SendEventsClosestToCustomerCity(limit: 5, customer);
