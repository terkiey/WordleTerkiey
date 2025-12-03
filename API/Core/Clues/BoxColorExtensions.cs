namespace API;

internal static class BoxColorExtensions
{
    private static readonly BoxColor[] _values =
        (BoxColor[])Enum.GetValues(typeof(BoxColor));

    public static BoxColor Next(this BoxColor value)
    {
        int index = Array.IndexOf(_values, value);
        return _values[(index + 1) % _values.Length];
    }
}
