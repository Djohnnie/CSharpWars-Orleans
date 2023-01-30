namespace CSharpWars.Common.Helpers;

public interface IRandomHelper
{
    int Get(int max);
    int Get(int min, int max);
    TEnum Get<TEnum>() where TEnum : struct, Enum;
    TItem GetItem<TItem>(List<TItem> items);
    TItem GetItem<TItem>(TItem[] items);
}

public class RandomHelper : IRandomHelper
{
    private readonly Random _random = Random.Shared;

    public int Get(int max)
    {
        return Get(0, max);
    }

    public int Get(int min, int max)
    {
        return _random.Next(min, max);
    }

    public TEnum Get<TEnum>() where TEnum : struct, Enum
    {
        var values = Enum.GetValues<TEnum>();
        var indexOfValue = _random.Next(values.Length);

        return values[indexOfValue];
    }

    public TItem GetItem<TItem>(List<TItem> items)
    {
        return items[_random.Next(items.Count)];
    }

    public TItem GetItem<TItem>(TItem[] items)
    {
        return items[_random.Next(items.Length)];
    }
}