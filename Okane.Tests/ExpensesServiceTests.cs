using Okane.Application;

namespace Okane.Tests;

public class ExpensesServiceTests
{
    private readonly ExpensesService _service;
    private List<Expense> _expenses;

    public ExpensesServiceTests()
    {
        _expenses = new List<Expense>();
        _service = new ExpensesService(_expenses);
    }

    [Fact]
    public void Create_Response()
    {
        var expense = _service.Create(new(10, "Food"));

        Assert.Equal(10, expense.Amount);
        Assert.Equal("Food", expense.CategoryName);
    }
    
    [Fact]
    public void Retrieve_NotFound()
    {
        var retrieved = _service.Retrieve(1);
        
        Assert.Null(retrieved);
    }
    
    [Fact]
    public void Retrieve_Updated()
    {
        var createResponse = _service.Create(
            new(10, "Food"));
        
        var updated = _service.Update(
            createResponse.Id, new(20, "Drinks"));
        
        var retrieved = _service.Retrieve(createResponse.Id);
        Assert.NotNull(retrieved);
        
        Assert.Equal(20, retrieved.Amount);
        Assert.Equal("Drinks", retrieved.CategoryName);
    }
    
    [Fact]
    public void Retrieve_OneExpense()
    {
        _expenses.Clear();
        
        var expense = _service.Create(new(10, "Food"));

        var retrieved = _service.Retrieve(expense.Id);
        
        Assert.NotNull(retrieved);
        Assert.Equal(10, retrieved.Amount);
        Assert.Equal("Food", retrieved.CategoryName);
    }
    
    [Fact]
    public void Retrieve_Deleted()
    {
        var createResponse = _service.Create(
            new(10, "Food"));

        var deleteResponse = _service.Delete(createResponse.Id);
        Assert.True(deleteResponse);
        
        var retrieved = _service.Retrieve(createResponse.Id);
        
        Assert.Null(retrieved);
    }

    [Fact]
    public void All()
    {
        _service.Create(new(10, "Food"));
        _service.Create(new(20, "Drinks"));
        
        var response = _service.All().ToArray();
        
        Assert.Equal(2, response.Count());
        
        var firstExpense = response.First();
        Assert.Equal(10, firstExpense.Amount);
        Assert.Equal("Food", firstExpense.CategoryName);
    }

    [Fact]
    public void Update_Response()
    {
        var createResponse = _service.Create(
            new(10, "Food"));
        
        var updated = _service.Update(
            createResponse.Id, new(20, "Drinks"));
        
        Assert.NotNull(updated);
        
        Assert.Equal(20, updated.Amount);
        Assert.Equal("Drinks", updated.CategoryName);
    }
    
    [Fact]
    public void Update_NotFound()
    {
        var updated = _service.Update(
            999, new(20, "Drinks"));
        
        Assert.Null(updated);
    }

    [Fact]
    public void Delete_Response()
    {
        var createResponse = _service.Create(
            new(10, "Food"));

        var response = _service.Delete(createResponse.Id);
        
        Assert.True(response);
    }
}
