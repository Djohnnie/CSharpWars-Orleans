using CSharpWars.WebApi.Exceptions;
using System.Diagnostics;
using System.Linq.Expressions;

namespace CSharpWars.WebApi.Helpers;

public interface IApiHelper<TManager>
{
    Task<IResult> Execute<TResult>(Expression<Func<TManager, Task<TResult>>> managerCall);
    Task<IResult> Execute(Func<TManager, Task> managerCall);
    Task<IResult> Post<TResult>(Func<TManager, Task<TResult>> managerCall);
}

public class ApiHelper<TManager> : IApiHelper<TManager>
{
    private readonly TManager _manager;
    private readonly ILogger<ApiHelper<TManager>> _logger;

    public ApiHelper(TManager manager, ILogger<ApiHelper<TManager>> logger)
    {
        _manager = manager;
        _logger = logger;
    }

    public async Task<IResult> Execute<TResult>(Expression<Func<TManager, Task<TResult>>> managerCall)
    {
        return await Try(async () =>
        {
            var logicCall = managerCall.Compile();

            TResult result = await logicCall(_manager);
            return result != null ? Results.Ok(result) : Results.NotFound();
        });
    }

    public async Task<IResult> Execute(Func<TManager, Task> managerCall)
    {
        return await Try(async () =>
        {
            await managerCall(_manager);
            return Results.Ok();
        });
    }

    public async Task<IResult> Post<TResult>(Func<TManager, Task<TResult>> managerCall)
    {
        return await Try(async () =>
        {
            var result = await managerCall(_manager);
            return result != null ? Results.Created("", result) : Results.NotFound();
        });
    }

    private async Task<IResult> Try(Func<Task<IResult>> action)
    {
        try
        {
            var startingTimestamp = Stopwatch.GetTimestamp();

            var result = await action();

            var elapsedTime = Stopwatch.GetElapsedTime(startingTimestamp);
            _logger.LogTrace("REQUEST: {Milliseconds}ms", elapsedTime.Milliseconds);

            return result;
        }
        catch (ManagerException ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}
