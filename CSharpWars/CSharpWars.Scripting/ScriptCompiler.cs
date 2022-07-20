using System.Runtime.CompilerServices;
using CSharpWars.Common.Extensions;
using CSharpWars.Enums;
using CSharpWars.Scripting.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace CSharpWars.Scripting;

public interface IScriptCompiler
{
    Task<ScriptRunner<object>> Compile(string script);
}

public class ScriptCompiler : IScriptCompiler
{
    public Task<ScriptRunner<object>> Compile(string script)
    {
        return Task.Run(() =>
        {
            var decodedScript = script.Base64Decode();
            var mscorlib = typeof(object).Assembly;
            var systemCore = typeof(Enumerable).Assembly;
            var dynamic = typeof(DynamicAttribute).Assembly;
            var csharpScript = typeof(BotProperties).Assembly;
            var enums = typeof(Move).Assembly;
            var scriptOptions = ScriptOptions.Default.AddReferences(mscorlib, systemCore, dynamic, csharpScript, enums);
            scriptOptions = scriptOptions.WithImports("System", "System.Linq", "System.Collections",
                "System.Collections.Generic", "CSharpWars.Enums", "CSharpWars.Scripting", "CSharpWars.Scripting.Model",
                "System.Runtime.CompilerServices");
            scriptOptions = scriptOptions.WithOptimizationLevel(OptimizationLevel.Release);
            var botScript = CSharpScript.Create(decodedScript, scriptOptions, typeof(ScriptGlobals));
            botScript.WithOptions(botScript.Options.AddReferences(mscorlib, systemCore));

            return botScript.CreateDelegate();
        });
    }
}