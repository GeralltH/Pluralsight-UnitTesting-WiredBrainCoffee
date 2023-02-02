using FluentAssertions;
using WiredBrainCoffee.DataProcessor.Data;
using WiredBrainCoffee.DataProcessor.Model;
using WiredBrainCoffee.DataProcessor.Parsing;

namespace WiredBrainCoffee.DataProcessor.Processing;

public class MachineDataProcessorTests : IDisposable
{
    private readonly FakeCoffeeCountStore _coffeeCountStore;
    private readonly MachineDataProcessor _machineDataProcessor;

    public MachineDataProcessorTests()
    {
        _coffeeCountStore = new FakeCoffeeCountStore();
        _machineDataProcessor = new MachineDataProcessor(_coffeeCountStore);
    }

    [Fact]
    public void ProcessItems_RunWithMultipleCoffeeTypes_ShouldSaveCountPerCoffeeType()
    {
        //Arrange
        var items = new[]
        {
            new MachineDataItem("Cappuccino", new DateTime(2022,10,27,8,0,0)),
            new MachineDataItem("Cappuccino", new DateTime(2022,10,27,9,0,0)),
            new MachineDataItem("Espresso", new DateTime(2022,10,27,10,0,0))
        };

        //Act
        _machineDataProcessor.ProcessItems(items);

        //Assert
        _coffeeCountStore.SavedItems.Count.Should().Be(2);

        var item = _coffeeCountStore.SavedItems[0];
        item.CoffeeType.Should().BeEquivalentTo("Cappuccino");
        item.Count.Should().Be(2);

        item = _coffeeCountStore.SavedItems[1];
        item.CoffeeType.Should().BeEquivalentTo("Espresso");
        item.Count.Should().Be(1);
    }

    [Fact]
    public void ProcessItems_RunMultipleTimes_ShouldClearPreviousCoffeeCount()
    {
        //Arrange
        var items = new[]
        {
            new MachineDataItem("Cappuccino", new DateTime(2022,10,27,8,0,0))
        };

        //Act
        _machineDataProcessor.ProcessItems(items);
        _machineDataProcessor.ProcessItems(items);

        //Assert
        _coffeeCountStore.SavedItems.Count.Should().Be(2);

        foreach (var item in _coffeeCountStore.SavedItems)
        {
            item.CoffeeType.Should().BeEquivalentTo("Cappuccino");
            item.Count.Should().Be(1);
        }
    }

    [Fact]
    public void ProcessItems_CoffeeItemWithOlderDateTimes_ShouldBeIgnored()
    {
        //Arrange
        var items = new[]
        {
            new MachineDataItem("Cappuccino", new DateTime(2022,10,27,8,0,0)),
            new MachineDataItem("Cappuccino", new DateTime(2022,10,27,7,0,0)),
            new MachineDataItem("Cappuccino", new DateTime(2022,10,27,9,0,0)),
            new MachineDataItem("Espresso", new DateTime(2022,10,27,10,0,0)),
            new MachineDataItem("Espresso", new DateTime(2022,10,27,10,0,0))
        };

        //Act
        _machineDataProcessor.ProcessItems(items);

        //Assert
        _coffeeCountStore.SavedItems.Count.Should().Be(2);

        var item = _coffeeCountStore.SavedItems[0];
        item.CoffeeType.Should().BeEquivalentTo("Cappuccino");
        item.Count.Should().Be(2);

        item = _coffeeCountStore.SavedItems[1];
        item.CoffeeType.Should().BeEquivalentTo("Espresso");
        item.Count.Should().Be(1);
    }

    public void Dispose()
    {
        //Run after every test
    }
}

public class FakeCoffeeCountStore : ICoffeeCountStore
{
    public List<CoffeeCountItem> SavedItems { get; } = new();
    public void Save(CoffeeCountItem item)
    {
        SavedItems.Add(item);
    }
}
