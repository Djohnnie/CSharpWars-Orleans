using System.Text.Json;

namespace CSharpWars.Common.Extensions;

public static class SerializationExtensions
{
    public static string Serialize<T>(this T objectToSerialize)
    {
        return objectToSerialize == null ? string.Empty : JsonSerializer.Serialize(objectToSerialize);
    }

    public static T? Deserialize<T>(this string objectToDeserialize)
    {
        return string.IsNullOrEmpty(objectToDeserialize) ? default : JsonSerializer.Deserialize<T>(objectToDeserialize);
    }
}