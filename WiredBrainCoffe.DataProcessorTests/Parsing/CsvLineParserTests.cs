using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiredBrainCoffee.DataProcessor.Model;

namespace WiredBrainCoffee.DataProcessor.Parsing;
public class CsvLineParserTests
{
    [Fact]
    public void Parse_RunWithValidData_ShouldParseValidLine()
    {
        //Arrange
        string[] csvLines = new[] { "Espresso;27/10/2022 8:01:16 AM" };

        //Act
        var machineDataItems = CsvLineParser.Parse(csvLines);

        //Assert
        machineDataItems.Should().NotBeNull();
        machineDataItems.Should().ContainSingle();
        machineDataItems[0].CoffeeType.Should().BeEquivalentTo("Espresso");
        machineDataItems[0].CreatedAt.Should().Be(new DateTime(2022,10,27,8,1,16));
    }

    [Fact]
    public void Parse_RunWithEmptyLines_ShouldSkipEmptyLines()
    {
        //Arrange
        string[] csvLines = new[] { "", "  " };

        //Act
        var machineDataItems = CsvLineParser.Parse(csvLines);

        //Assert
        machineDataItems.Should().BeNullOrEmpty();
    }

    [InlineData("Cappuccino", "Invalid Csv Line")]
    [InlineData("Cappuccino;InvalidDateTime", "Invalid DateTime in Csv Line")]
    [Theory]
    public void Parse_RunWithInvaidData_ShouldThrowExceptionForInvalidLine(string csvLine, string expectedMessagePrefix)
    {
        //Arrange
        var csvLines = new[] { csvLine };

        //Act
        Action act = () => CsvLineParser.Parse(csvLines);

        //Assert
        var exception = act.Should().Throw<Exception>().WithMessage($"{expectedMessagePrefix}: {csvLine}");
    }
}
