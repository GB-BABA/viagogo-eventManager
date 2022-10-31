namespace Viagogo.EventManager.Exceptions;

public class NoEventinCustomerLocationException : Exception
{
    public const string DEFAULT_MESSAGE = "There are no events found in customer location";

    public NoEventinCustomerLocationException() : base(DEFAULT_MESSAGE)
    {

    }
}