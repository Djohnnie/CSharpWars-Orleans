namespace CSharpWars.Common.Helpers;

public interface IRandomHelper
{
    int Get(int max);
    int Get(int min, int max);
    TEnum Get<TEnum>() where TEnum : Enum;
    TItem GetItem<TItem>(List<TItem> items);
}

public class RandomHelper : IRandomHelper
{
    private readonly Random _random = new();

    public int Get(int max)
    {
        return Get(0, max);
    }

    public int Get(int min, int max)
    {
        return _random.Next(min, max);
    }

    public TEnum Get<TEnum>() where TEnum : Enum
    {
        var values = Enum.GetValues(typeof(TEnum));
        var indexOfValue = _random.Next(values.Length);

        return (TEnum)values.GetValue(indexOfValue) ?? default;
    }

    public TItem GetItem<TItem>(List<TItem> items)
    {
        return items[_random.Next(items.Count)];
    }
}