namespace SqModel.Analysis.Extensions;

public static class stringExtension
{
    public static bool AreEqual(this string? source, string? text)
    {
        if (source == null && text == null) return true;
        if (source == null) return false;
        return string.Equals(source, text, StringComparison.CurrentCultureIgnoreCase);
    }

    public static bool AreContains(this string? source, IEnumerable<string?> texts)
    {
        return texts.Where(x => source.AreEqual(x)).Any();
    }

    public static bool IsNumeric(this string source)
    {
        if (string.IsNullOrEmpty(source)) return false;
        return source.First().IsInteger();
    }
}