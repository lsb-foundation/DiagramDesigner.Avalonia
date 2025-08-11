namespace DiagramDesigner;

internal static class NumberExtension
{
    public static bool IsBetween(this double value, double min, double max)
        => value >= min && value <= max;
}
