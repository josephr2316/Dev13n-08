namespace Okane.Application;

public class CategoriesService(
    ICategoriesRepository categories,
    IExpensesRepository expenses)
{
    public Result<CategoryResponse> Create(CreateCategoryRequest request)
    {
        var existing = categories.ByName(request.Name);
        if (existing != null)
            return new ErrorResult<CategoryResponse>("Category already exists");

        var category = new Category
        {
            Name = request.Name
        };
        categories.Add(category);

        return new OkResult<CategoryResponse>(new CategoryResponse(category.Id, category.Name));
    }

    public Result<CategoryResponse> Retrieve(int id)
    {
        var category = categories.ById(id);

        if (category == null)
            return new NotFoundResult<CategoryResponse>($"{nameof(Category)} with id {id} was not found.");

        return new OkResult<CategoryResponse>(new CategoryResponse(category.Id, category.Name));
    }

    public Result<IEnumerable<CategoryResponse>> All()
    {
        var response = categories
            .All()
            .Select(c => new CategoryResponse(c.Id, c.Name));

        return new OkResult<IEnumerable<CategoryResponse>>(response);
    }

    public Result Remove(int id)
    {
        if (!categories.Exists(id))
            return new NotFoundResult($"Category with id {id} was not found.");

        var hasExpenses = expenses.All().Any(e => e.CategoryId == id);
        if (hasExpenses)
            return new ErrorResult("Can not delete category with existing expenses");

        categories.Remove(id);
        return new OkResult();
    }

    public Result<CategoryResponse> Update(int id, UpdateCategoryRequest request)
    {
        var category = categories.ById(id);
        if (category == null)
            return new NotFoundResult<CategoryResponse>($"Category with id {id} was not found.");

        var existingWithName = categories.ByName(request.Name);
        if (existingWithName != null && existingWithName.Id != id)
            return new ErrorResult<CategoryResponse>("Category already exists");

        var updated = categories.Update(id, request);
        var response = new CategoryResponse(updated.Id, updated.Name);
        return new OkResult<CategoryResponse>(response);
    }
}
