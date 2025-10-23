namespace AspNetCoreBoilerplate.Web.Core.Extensions;

public static class StringExtensions
{
    public static string ToTitleCase(this string segment)
    {
        return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(segment.Replace("-", " "));
    }

    public static string FirstLineWithEllipsis(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "";

        int index = text.IndexOf('\n');
        string firstLine = (index >= 0) ? text.Substring(0, index) : text;

        return firstLine + "...";
    }

    public static string TakeWithEllipsis(this string? text, int take = 25)
    {
        if (string.IsNullOrEmpty(text) || take <= 0)
            return "";

        if (text.Length <= take)
            return text;

        return text.Substring(0, take) + "...";
    }

    public static string ToCamelCase(this string stringValue)
    {
        if (string.IsNullOrWhiteSpace(stringValue))
            return stringValue;

        stringValue = stringValue.Trim();

        if (stringValue.Length == 1)
            return stringValue.ToLower();

        return char.ToLowerInvariant(stringValue[0]) + stringValue.Substring(1);
    }
}
