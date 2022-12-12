namespace CSharpWars.Orleans.Contracts.Exceptions;

[GenerateSerializer]
public class CSharpWarsException : Exception
{
    public CSharpWarsException(string message) : base(message)
    {

    }
}