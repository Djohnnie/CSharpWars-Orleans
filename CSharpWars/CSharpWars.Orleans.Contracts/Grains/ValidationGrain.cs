namespace CSharpWars.Orleans.Contracts.Grains;

public interface IValidationGrain : IGrainWithGuidKey
{
    Task<ValidatedScriptDto> Validate(ScriptToValidateDto scriptToValidate);
}