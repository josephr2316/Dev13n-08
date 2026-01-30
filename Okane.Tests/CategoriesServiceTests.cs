using Okane.Application;

namespace Okane.Tests;

public class CategoriesServiceTests
{
    private readonly CategoriesService _service;
    private readonly ExpensesService _expensesService;

    public CategoriesServiceTests()
    {
        var categoriesRepository = new InMemoryCategoriesRepository();
        var expensesRepository = new InMemoryExpensesRepository();
        _service = new CategoriesService(categoriesRepository, expensesRepository);
        _expensesService = new ExpensesService(expensesRepository, categoriesRepository,
            new ExpenseResponseFactory());
    }
    
    [Fact]
    public void Create_Response()
    {
        var category = _service.Create(new CreateCategoryRequest("Food")).AssertOk();

        Assert.Equal(1, category.Id);
        Assert.Equal("Food", category.Name);
    }
    
    [Fact]
    public void Create_CategoryAlreadyExists()
    {
        _service.Create(new CreateCategoryRequest("Food")).AssertOk();
        
        var error = _service.Create(new CreateCategoryRequest("Food")).AssertError();

        Assert.Equal("Category already exists", error);
    }
    
    [Fact]
    public void Retrieve_ExistingCategory()
    {
        var created = _service.Create(new CreateCategoryRequest("Drinks")).AssertOk();

        var retrieved = _service.Retrieve(created.Id).AssertOk();

        Assert.Equal(1, retrieved.Id);
        Assert.Equal("Drinks", retrieved.Name);
    }
    
    [Fact]
    public void Retrieve_NotFound()
    {
        var message = _service.Retrieve(999).AssertNotFound();
        Assert.Contains("not found", message);
    }
    
    [Fact]
    public void Retrieve_Updated()
    {
        var created = _service.Create(new("Taxes")).AssertOk();

        var updated = _service.Update(created.Id, 
            new UpdateCategoryRequest("Education")).AssertOk();

        var retrieved = _service.Retrieve(updated.Id).AssertOk();

        Assert.Equal("Education", retrieved.Name);
    }
    
    [Fact]
    public void Retrieve_Updated_AlreadyExists()
    {
        _service.Create(new("Taxes")).AssertOk();
        var created = _service.Create(new("Games")).AssertOk();

        var error = _service.Update(created.Id, 
            new UpdateCategoryRequest("Taxes")).AssertError();
        
        Assert.Equal("Category already exists", error);
    }
    
    [Fact]
    public void All()
    {
        _service.Create(new CreateCategoryRequest("Food")).AssertOk();
        _service.Create(new CreateCategoryRequest("Drinks")).AssertOk();

        var all = _service.All().AssertOk().ToArray();

        Assert.Equal(2, all.Length);
        Assert.Contains(all, e => e.Name == "Food");
        Assert.Contains(all, e => e.Name == "Drinks");
    }
    
    [Fact]
    public void Remove()
    {
        var created = _service.Create(new CreateCategoryRequest("Food")).AssertOk();

        _service.Remove(created.Id).AssertOk();

        _service.Retrieve(created.Id).AssertNotFound();
    }
    
    [Fact]
    public void Remove_ExpensesExist()
    {
        var createdCategory = _service.Create(new CreateCategoryRequest("Food")).AssertOk();
        
        _expensesService.Create(new(30, "Food")).AssertOk();
        
        var error = _service.Remove(createdCategory.Id).AssertError();

        Assert.Equal("Can not delete category with existing expenses", error);
    }
}