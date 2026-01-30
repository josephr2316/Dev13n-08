using Okane.Application;

namespace Okane.Tests;

public class ExpensesServiceTests
{
    private readonly ExpensesService _service;
    private readonly InMemoryCategoryRepository _categories;

    public ExpensesServiceTests()
    {
        _categories = new InMemoryCategoryRepository();
        var expenses = new InMemoryExpenseRepository();
        _service = new ExpensesService(expenses, _categories);
    }

    [Fact]
    public void Create_Response()
    {
        var category = CreateCategory("Food");
        var expense = _service.Create(new(10, category.Id)).AssertOk();

        Assert.Equal(10, expense.Amount);
        Assert.Equal(category.Id, expense.CategoryId);
        Assert.Equal("Food", expense.CategoryName);
    }
    
    [Fact]
    public void Create_WithDescription_Response()
    {
        var category = CreateCategory("Food");
        var expense = _service.Create(new(
            Amount: 10, 
            CategoryId: category.Id, 
            Description: "McDonalds")
        ).AssertOk();

        Assert.Equal(10, expense.Amount);
        Assert.Equal(category.Id, expense.CategoryId);
        Assert.Equal("Food", expense.CategoryName);
        Assert.Equal("McDonalds", expense.Description);
    }

    [Fact]
    public void Create_InvalidAmount()
    {
        var category = CreateCategory("Food");
        var message = _service.Create(new(-1, category.Id)).AssertError();
        Assert.Equal("Amount must be greater than 1.", message);
    }

    [Fact]
    public void Retrieve_ExistingExpense()
    {
        var category = CreateCategory("Food");
        var created = _service.Create(new(
            Amount: 10, 
            CategoryId: category.Id, 
            Description: "McDonalds")
        ).AssertOk();

        var retrieved = _service.Retrieve(created.Id).AssertOk();

        Assert.Equal(10, retrieved.Amount);
        Assert.Equal(category.Id, retrieved.CategoryId);
        Assert.Equal("Food", retrieved.CategoryName);
        Assert.Equal("McDonalds", retrieved.Description);
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
        var food = CreateCategory("Food");
        var drinks = CreateCategory("Drinks");

        _service.Create(new(10, food.Id)).AssertOk();
        _service.Create(new(20, drinks.Id)).AssertOk();

        var all = _service.All().AssertOk().ToArray();

        Assert.Equal(2, all.Length);
        Assert.Contains(all, e => e.Amount == 10 && e.CategoryName == "Food" && e.CategoryId == food.Id);
        Assert.Contains(all, e => e.Amount == 20 && e.CategoryName == "Drinks" && e.CategoryId == drinks.Id);
    }

    [Fact]
    public void Update_ExistingExpense()
    {
        var food = CreateCategory("Food");
        var drinks = CreateCategory("Drinks");
        var created = _service.Create(new(10, food.Id)).AssertOk();

        var updated = _service.Update(created.Id, new(20, drinks.Id)).AssertOk();

        Assert.Equal(20, updated.Amount);
        Assert.Equal(drinks.Id, updated.CategoryId);
        Assert.Equal("Drinks", updated.CategoryName);
    }

    [Fact]
    public void Update_InvalidAmount()
    {
        var category = CreateCategory("Food");
        var message = _service.Update(1, new(-5, category.Id)).AssertError();
        Assert.Equal("Amount must be greater than 1.", message);
    }

    [Fact]
    public void Update_NotFound()
    {
        var category = CreateCategory("Drinks");
        var message = _service.Update(999, new(20, category.Id)).AssertNotFound();
        Assert.Contains("not found", message);
    }

    [Fact]
    public void Delete_ExistingExpense()
    {
        var category = CreateCategory("Food");
        var created = _service.Create(new(10, category.Id)).AssertOk();

        _service.Delete(created.Id).AssertOk();
    }

    [Fact]
    public void Delete_NotFound()
    {
        var message = _service.Delete(999).AssertNotFound();
        Assert.Contains("not found", message);
    }

    private Category CreateCategory(string name)
    {
        var category = new Category { Name = name };
        _categories.Add(category);
        return category;
    }
}
