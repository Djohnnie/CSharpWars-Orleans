using System.Collections.Immutable;
using CSharpWars.Common.Extensions;
using CSharpWars.Enums;
using CSharpWars.Orleans.Contracts.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace CSharpWars.Scripting;

public interface IScriptCompiler
{
    Task<ScriptRunner<object?>> CompileForExecution(string script);
    Task<ImmutableArray<Diagnostic>> CompileForDiagnostics(string script);
}

public class ScriptCompiler : IScriptCompiler
{
    private readonly static Assembly MSCoreLib = typeof(object).Assembly;
    private readonly static Assembly SystemCore = typeof(Enumerable).Assembly;
    private readonly static Assembly Dynamic = typeof(DynamicAttribute).Assembly;
    private readonly static Assembly CSharpScript = typeof(BotProperties).Assembly;
    private readonly static Assembly Enums = typeof(Move).Assembly;

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
        
        var scriptOptions = ScriptOptions.Default
            .AddReferences(MSCoreLib, SystemCore, Dynamic, CSharpScript, Enums)
            .WithImports(
                "System", "System.Linq", "System.Collections",
                "System.Collections.Generic", "CSharpWars.Enums",
                "CSharpWars.Orleans.Contracts.Model",
                "System.Runtime.CompilerServices")
            .WithOptimizationLevel(OptimizationLevel.Release);
        var botScript = Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript.Create(decodedScript, scriptOptions, typeof(ScriptGlobals));
        botScript.WithOptions(botScript.Options.AddReferences(MSCoreLib, SystemCore));

        return botScript;
    }
}