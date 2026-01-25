using Okane.Application;

namespace Okane.Tests;

public class ExpensesServiceTests_WithFile : IDisposable
{
    private readonly ExpensesService _service;

    // Before each test
    public ExpensesServiceTests_WithFile()
    {
        // TODO: Create file
        
        _service = new ExpensesService(new FileExpensesRepository());
    }
    
    // After each test
    public void Dispose()
    {
        // TODO: Delete file
    }

    [Fact]
    public void Create_Response()
    {
        var expense = _service.Create(new(10, "Food")).AssertOk();

        Assert.Equal(10, expense.Amount);
        Assert.Equal("Food", expense.CategoryName);
    }

    [Fact]
    public void Create_InvalidAmount()
    {
        var message = _service.Create(new(-1, "Food")).AssertError();
        Assert.Equal("Amount must be greater than 1.", message);
    }

    [Fact]
    public void Retrieve_ExistingExpense()
    {
        var created = _service.Create(new(10, "Food")).AssertOk();

        var retrieved = _service.Retrieve(created.Id).AssertOk();

        Assert.Equal(10, retrieved.Amount);
        Assert.Equal("Food", retrieved.CategoryName);
    }

    [Fact]
    public void Retrieve_NotFound()
    {
        var message = _service.Retrieve(999).AssertNotFound();
        Assert.Contains("not found", message);
    }

    [Fact]
    public void All_ReturnsAllExpenses()
    {
        _service.Create(new(10, "Food")).AssertOk();
        _service.Create(new(20, "Drinks")).AssertOk();

        var all = _service.All().AssertOk().ToArray();

        Assert.Equal(2, all.Length);
        Assert.Contains(all, e => e.Amount == 10 && e.CategoryName == "Food");
        Assert.Contains(all, e => e.Amount == 20 && e.CategoryName == "Drinks");
    }

    [Fact]
    public void Update_ExistingExpense()
    {
        var created = _service.Create(new(10, "Food")).AssertOk();

        var updated = _service.Update(created.Id, new(20, "Drinks")).AssertOk();

        Assert.Equal(20, updated.Amount);
        Assert.Equal("Drinks", updated.CategoryName);
    }

    [Fact]
    public void Update_InvalidAmount()
    {
        var message = _service.Update(1, new(-5, "Food")).AssertError();
        Assert.Equal("Amount must be greater than 1.", message);
    }

    [Fact]
    public void Update_NotFound()
    {
        var message = _service.Update(999, new(20, "Drinks")).AssertNotFound();
        Assert.Contains("not found", message);
    }

    [Fact]
    public void Delete_ExistingExpense()
    {
        var created = _service.Create(new(10, "Food")).AssertOk();

        _service.Delete(created.Id).AssertOk();
    }

    [Fact]
    public void Delete_NotFound()
    {
        var message = _service.Delete(999).AssertNotFound();
        Assert.Contains("not found", message);
    }
}