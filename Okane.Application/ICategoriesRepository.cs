namespace Okane.Application;

public interface ICategoriesRepository : IRepository<Category>
{
    Category? ByName(string name);
    Category Update(int id, UpdateCategoryRequest request);
}