using CSharpWars.Orleans.Contracts.Arena;
using CSharpWars.Orleans.Contracts.Bot;
using CSharpWars.Scripting;
using CSharpWars.Scripting.Model;
using Microsoft.CodeAnalysis.Scripting;
using Orleans;
using Orleans.Runtime;

namespace CSharpWars.Orleans.Grains;

public class ScriptState
{
    public bool Exists { get; set; }
    public string Script { get; set; }
}

public interface IScriptGrain : IGrainWithGuidKey
{
    Task SetScript(string script);
    Task RunScript();
    Task DeleteScript();
}

public class ScriptGrain : Grain, IScriptGrain
{
    private readonly IScriptCompiler _scriptCompiler;
    private readonly IPersistentState<ScriptState> _state;

    private ScriptRunner<object>? _compiledScript;

    public ScriptGrain(
        IScriptCompiler scriptCompiler,
        [PersistentState("script", "scriptStore")] IPersistentState<ScriptState> state)
    {
        _scriptCompiler = scriptCompiler;
        _state = state;
    }

    public override async Task OnActivateAsync()
    {
        if (_state.State.Exists)
        {
            _compiledScript = await _scriptCompiler.Compile(_state.State.Script);
        }

        await base.OnActivateAsync();
    }

    public async Task SetScript(string script)
    {
        _state.State.Script = script;
        _state.State.Exists = true;

        _compiledScript = await _scriptCompiler.Compile(script);

        await _state.WriteStateAsync();
    }

    public async Task RunScript()
    {
        if (_state.State.Exists && _compiledScript != null)
        {
            var botProperties = BotProperties.Build(
                new BotDto(Guid.NewGuid(), "", 100, 100),
                new ArenaDto("", 10, 10),
                new List<BotDto>());
            var scriptGlobals = ScriptGlobals.Build(botProperties);
            _ = await _compiledScript.Invoke(scriptGlobals);
        }
    }

    public async Task DeleteScript()
    {
        if (_state.State.Exists)
        {
            await _state.ClearStateAsync();
        }

        DeactivateOnIdle();
    }
}