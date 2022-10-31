namespace Viagogo.EventManager.Exceptions;

public class CustomerNotFoundException : Exception
{
    private const string DEFAULT_MESSAGE = "Customer was not found";

    public CustomerNotFoundException() : base(DEFAULT_MESSAGE)
    {

    }
}