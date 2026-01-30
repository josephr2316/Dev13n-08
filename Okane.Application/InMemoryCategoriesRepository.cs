namespace Okane.Application;

public class InMemoryCategoriesRepository : InMemoryRepository<Category>, ICategoriesRepository
{
    public Category? ByName(string name) =>
        Entities.FirstOrDefault(category => category.Name == name);

    public Category Update(int id, UpdateCategoryRequest request)
    {
        var existing = Entities.First(c => c.Id == id);
        existing.Name = request.Name;
        return existing;
    }
}