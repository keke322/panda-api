using Xunit;
using FluentAssertions;
using Panda.Api.Utils; // Adjust to your actual namespace
public class UtilsTests
{
    [Theory]
    [InlineData("EC1A 1BB")]
    [InlineData("W1A 0AX")]
    [InlineData("M1 1AE")]
    [InlineData("B33 8TH")]
    [InlineData("CR2 6XH")]
    [InlineData("DN55 1PT")]
    [InlineData("GIR 0AA")]  
    [InlineData("ec1a 1bb")] 
    [InlineData(" EC1A1BB ")]
    public void IsValidUkPostcode_ValidFormats_ShouldReturnTrue(string postcode)
    {
        var result = Utils.IsValidUkPostcode(postcode);
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    [InlineData("123456")]
    [InlineData("ABCDE")]
    [InlineData("EC1A1BB2")]
    [InlineData("INVALID")]
    public void IsValidUkPostcode_InvalidFormats_ShouldReturnFalse(string postcode)
    {
        var result = Utils.IsValidUkPostcode(postcode);
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("1373645350")]
    [InlineData("9434765919")]
    [InlineData("9876543210")]
    public void BeValidNhsNumber_ValidNumbers_ShouldReturnTrue(string nhsNumber)
    {
        var result = Utils.BeValidNhsNumber(nhsNumber);
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("9434765918")]
    [InlineData("1373645351")]
    [InlineData("abcdefghij")]
    [InlineData("12345")]     
    [InlineData("123456789012")] 
    [InlineData("")]
    [InlineData("        ")]
    [InlineData(null)]
    public void BeValidNhsNumber_InvalidNumbers_ShouldReturnFalse(string nhsNumber)
    {
        var result = Utils.BeValidNhsNumber(nhsNumber);
        result.Should().BeFalse();
    }

    [Fact]
    public void BeValidNhsNumber_Should_IgnoreSpaces()
    {
        var result = Utils.BeValidNhsNumber("137 364 5350"); // Valid with spaces
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("1h", 3600)]
    [InlineData("30m", 1800)]
    [InlineData("1h30m", 5400)]
    [InlineData("2h15m", 8100)]
    [InlineData("0h5m", 300)]
    [InlineData("10h0m", 36000)]
    [InlineData("0h0m", 0)]
    public void ParseDurationToSeconds_ValidInputs_ReturnsExpectedSeconds(string input, int expected)
    {
        var result = Utils.ParseDurationToSeconds(input);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("h30m")]
    [InlineData("1hour")]
    [InlineData("1h30")]
    [InlineData("m")]
    [InlineData("1x30m")]
    public void ParseDurationToSeconds_InvalidInputs_ThrowsFormatException(string input)
    {
        Action act = () => Utils.ParseDurationToSeconds(input);
        act.Should().Throw<FormatException>();
    }
}
