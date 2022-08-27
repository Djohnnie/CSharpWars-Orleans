using System.Collections.Immutable;
using CSharpWars.Common.Extensions;
using CSharpWars.Enums;
using CSharpWars.Orleans.Contracts.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Runtime.CompilerServices;

namespace CSharpWars.Scripting;

public interface IScriptCompiler
{
    Task<ScriptRunner<object?>> CompileForExecution(string script);
    Task<ImmutableArray<Diagnostic>> CompileForDiagnostics(string script);
}

public class ScriptCompiler : IScriptCompiler
{
    public Task<ScriptRunner<object?>> CompileForExecution(string script)
    {
        return Task.Run(() =>
        {
            var botScript = PrepareScript(script);
            return botScript.CreateDelegate();
        });
    }

    public Task<ImmutableArray<Diagnostic>> CompileForDiagnostics(string script)
    {
        return Task.Run(() =>
        {
            var botScript = PrepareScript(script);
            return botScript.Compile();
        });
    }

    private Script<object?> PrepareScript(string script)
    {
        var decodedScript = script.Base64Decode();
        var mscorlib = typeof(object).Assembly;
        var systemCore = typeof(Enumerable).Assembly;
        var dynamic = typeof(DynamicAttribute).Assembly;
        var csharpScript = typeof(BotProperties).Assembly;
        var enums = typeof(Move).Assembly;
        var scriptOptions = ScriptOptions.Default.AddReferences(mscorlib, systemCore, dynamic, csharpScript, enums);
        scriptOptions = scriptOptions.WithImports(
            "System", "System.Linq", "System.Collections",
            "System.Collections.Generic", "CSharpWars.Enums",
            "CSharpWars.Orleans.Contracts.Model",
            "System.Runtime.CompilerServices");
        scriptOptions = scriptOptions.WithOptimizationLevel(OptimizationLevel.Release);
        var botScript = CSharpScript.Create(decodedScript, scriptOptions, typeof(ScriptGlobals));
        botScript.WithOptions(botScript.Options.AddReferences(mscorlib, systemCore));

        return botScript;
    }
}