namespace Viagogo.EventManager.Exceptions;

public class CustomerModelIsRequiredException : Exception
{
    private const string DEFAULT_MESSAGE = "Customer model can not be null";

    public CustomerModelIsRequiredException() : base(DEFAULT_MESSAGE)
    {

    }
}

