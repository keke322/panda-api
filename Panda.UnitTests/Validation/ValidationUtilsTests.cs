using Xunit;
using FluentAssertions;
using Panda.Api.Utils; // Adjust to your actual namespace
public class ValidationUtilsTests
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
        var result = ValidationUtils.IsValidUkPostcode(postcode);
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
        var result = ValidationUtils.IsValidUkPostcode(postcode);
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("1373645350")]
    [InlineData("9434765919")]
    [InlineData("9876543210")]
    public void BeValidNhsNumber_ValidNumbers_ShouldReturnTrue(string nhsNumber)
    {
        var result = ValidationUtils.BeValidNhsNumber(nhsNumber);
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
        var result = ValidationUtils.BeValidNhsNumber(nhsNumber);
        result.Should().BeFalse();
    }

    [Fact]
    public void BeValidNhsNumber_Should_IgnoreSpaces()
    {
        var result = ValidationUtils.BeValidNhsNumber("137 364 5350"); // Valid with spaces
        result.Should().BeTrue();
    }
}
