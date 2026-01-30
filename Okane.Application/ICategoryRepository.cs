namespace Okane.Application;

public interface ICategoryRepository : IRepository<Category, UpdateCategoryRequest>
{
    Category? ByName(string name);
}