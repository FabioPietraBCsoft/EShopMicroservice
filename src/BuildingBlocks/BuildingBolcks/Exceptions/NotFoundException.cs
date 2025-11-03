namespace BuildingBolcks.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {

    }

    public NotFoundException(string name, object key) : base($"Entità \"{name}\" ({key}) non è stata trovata")
    {

    }

}

