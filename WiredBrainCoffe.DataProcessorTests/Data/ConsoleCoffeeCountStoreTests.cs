using FluentAssertions;
using WiredBrainCoffee.DataProcessor.Model;

namespace WiredBrainCoffee.DataProcessor.Data;

public class ConsoleCoffeeCountStoreTests
{
    [Fact]
    public void Save_PassInCoffeeItems_ShouldWriteOutputToConsole()
    {
        //Arrange
        var item = new CoffeeCountItem("Cappuccino", 5);
        var stringWriter = new StringWriter();
        var consoleCoffeeCountStore = new ConsoleCoffeeCountStore(stringWriter);

        //Act
        consoleCoffeeCountStore.Save(item);

        //Assert
        var result = stringWriter.ToString();

        result.Should().BeEquivalentTo($"{item.CoffeeType}: {item.Count}{Environment.NewLine}");
    }
}
