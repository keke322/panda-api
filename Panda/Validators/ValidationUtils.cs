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

    public static bool BeValidNhsNumber(string nhsNumber)
    {
        if (string.IsNullOrWhiteSpace(nhsNumber))
            return false;

        // Remove spaces
        nhsNumber = nhsNumber.Replace(" ", "");

        if (!Regex.IsMatch(nhsNumber, @"^\d{10}$"))
            return false;

        // MOD11 checksum validation
        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            sum += (10 - i) * (nhsNumber[i] - '0');
        }
        int remainder = sum % 11;
        int checkDigit = 11 - remainder;
        if (checkDigit == 11) checkDigit = 0;
        if (checkDigit == 10) return false;

        return checkDigit == (nhsNumber[9] - '0');
    }
}
