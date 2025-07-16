using System.Text.RegularExpressions;

namespace Panda.Api.Utils;

public static class ValidationUtils
{
    private static readonly Regex UkPostcodeRegex = new Regex(
        @"^(GIR 0AA|[A-Z]{1,2}\d{1,2} ?\d[A-Z]{2}|[A-Z]{1,2}\d[A-Z] ?\d[A-Z]{2}|[A-Z]{1,2}\d{2} ?\d[A-Z]{2})$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static bool IsValidUkPostcode(string postcode)
    {
        if (string.IsNullOrWhiteSpace(postcode))
            return false;

        return UkPostcodeRegex.IsMatch(postcode.Trim());
    }
}
