namespace Okane.Application;

public class CategoriesService(ICategoriesRepository categories)
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

    public Result Remove(int createdId)
    {
        throw new NotImplementedException();
    }

    public Result<CategoryResponse> Update(int id, UpdateCategoryRequest reuqest)
    {
        throw new NotImplementedException();
    }
}