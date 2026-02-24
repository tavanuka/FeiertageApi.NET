using FeiertageApi.Extensions;
using FeiertageApi.Models;

namespace FeiertageApi.Tests.Extensions;

public class GermanStateExtensionsTests
{
    [Fact]
    public void ToStateCode_ShouldReturnCorrectCode_ForAllStates()
    {
        // Arrange & Act & Assert
        Assert.Equal("bw", GermanState.BadenWuerttemberg.ToStateCode());
        Assert.Equal("by", GermanState.Bavaria.ToStateCode());
        Assert.Equal("be", GermanState.Berlin.ToStateCode());
        Assert.Equal("bb", GermanState.Brandenburg.ToStateCode());
        Assert.Equal("hb", GermanState.Bremen.ToStateCode());
        Assert.Equal("hh", GermanState.Hamburg.ToStateCode());
        Assert.Equal("he", GermanState.Hesse.ToStateCode());
        Assert.Equal("mv", GermanState.MecklenburgVorpommern.ToStateCode());
        Assert.Equal("ni", GermanState.LowerSaxony.ToStateCode());
        Assert.Equal("nw", GermanState.NorthRhineWestphalia.ToStateCode());
        Assert.Equal("rp", GermanState.RhinelandPalatinate.ToStateCode());
        Assert.Equal("sl", GermanState.Saarland.ToStateCode());
        Assert.Equal("sn", GermanState.Saxony.ToStateCode());
        Assert.Equal("st", GermanState.SaxonyAnhalt.ToStateCode());
        Assert.Equal("sh", GermanState.SchleswigHolstein.ToStateCode());
        Assert.Equal("th", GermanState.Thuringia.ToStateCode());
    }

    [Fact]
    public void ToStateCodes_ShouldConvertMultipleStates()
    {
        // Arrange
        var states = new[] { GermanState.Bavaria, GermanState.Berlin, GermanState.Hamburg };

        // Act
        var codes = states.ToStateCodes();

        // Assert
        Assert.Equal(new[] { "by", "be", "hh" }, codes);
    }

    [Theory]
    [InlineData("by", GermanState.Bavaria)]
    [InlineData("BY", GermanState.Bavaria)] // Case insensitive
    [InlineData("be", GermanState.Berlin)]
    [InlineData("hh", GermanState.Hamburg)]
    [InlineData("bw", GermanState.BadenWuerttemberg)]
    public void FromStateCode_ShouldParseCorrectly(string code, GermanState expected)
    {
        // Act
        var result = GermanStateExtensions.FromStateCode(code);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void FromStateCode_ShouldThrow_WhenCodeIsNullOrEmpty(string? code)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(
            () => GermanStateExtensions.FromStateCode(code!));
        Assert.Equal("stateCode", ex.ParamName);
    }

    [Fact]
    public void FromStateCode_ShouldThrow_WhenCodeIsInvalid()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(
            () => GermanStateExtensions.FromStateCode("invalid"));
        Assert.StartsWith("Invalid state code", ex.Message);
        Assert.Equal("stateCode", ex.ParamName);
    }

    [Theory]
    [InlineData("by", GermanState.Bavaria, true)]
    [InlineData("BE", GermanState.Berlin, true)]
    [InlineData("invalid", default(GermanState), false)]
    [InlineData("", default(GermanState), false)]
    [InlineData(null, default(GermanState), false)]
    public void TryParseStateCode_ShouldReturnExpectedResult(string? code, GermanState expectedState, bool expectedResult)
    {
        // Act
        var result = GermanStateExtensions.TryParseStateCode(code!, out var state);

        // Assert
        Assert.Equal(expectedResult, result);
        if (expectedResult)
        {
            Assert.Equal(expectedState, state);
        }
    }

    [Fact]
    public void GetAllStates_ShouldReturn16States()
    {
        // Act
        var states = GermanStateExtensions.GetAllStates();

        // Assert
        Assert.Equal(16, states.Count());
        Assert.Contains(GermanState.Bavaria, states);
        Assert.Contains(GermanState.Berlin, states);
    }
}