using Orleans;

namespace CSharpWars.Orleans.Contracts;

[GenerateSerializer]
public class StatusDto
{
    [Id(0)]
    public string Message { get; set; }
}