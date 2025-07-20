using System.Text.RegularExpressions;

namespace Panda.Api.Utils;

public static class Utils
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
    public static int ParseDurationToSeconds(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new FormatException("Input is null or empty.");

        int totalSeconds = 0;
        int currentNumber = 0;
        bool numberSeen = false;

        foreach (char c in input)
        {
            if (char.IsDigit(c))
            {
                currentNumber = currentNumber * 10 + (c - '0');
                numberSeen = true;
            }
            else if (c == 'h')
            {
                if (!numberSeen) throw new FormatException("Missing number before 'h'.");
                totalSeconds += currentNumber * 3600;
                currentNumber = 0;
                numberSeen = false;
            }
            else if (c == 'm')
            {
                if (!numberSeen) throw new FormatException("Missing number before 'm'.");
                totalSeconds += currentNumber * 60;
                currentNumber = 0;
                numberSeen = false;
            }
            else
            {
                throw new FormatException($"Unexpected character '{c}' in duration string.");
            }
        }

        if (numberSeen)
            throw new FormatException("Unfinished duration component at the end.");

        return totalSeconds;
    }
}
