using Microsoft.EntityFrameworkCore;
using Okane.Application;

namespace Okane.Storage.EntityFramework;

public class CategoriesRepository(OkaneDbContext db) : ICategoryRepository
{
    public void Add(Category entity)
    {
        db.Categories.Add(entity);
        db.SaveChanges();
    }

    public Category? ById(int id) =>
        db.Categories
            .Include(category => category.Expenses)
            .FirstOrDefault(category => category.Id == id);

    public IEnumerable<Category> All() =>
        db.Categories
            .Include(category => category.Expenses);

    public void Remove(int id)
    {
        var category = db.Categories.First(c => c.Id == id);
        db.Categories.Remove(category);
        db.SaveChanges();
    }

    public bool Exists(int id) =>
        db.Categories.Any(c => c.Id == id);

    public Category Update(int id, UpdateCategoryRequest request)
    {
        var existing = db.Categories.First(c => c.Id == id);
        existing.Name = request.Name;
        db.SaveChanges();
        return existing;
    }

    public Category? ByName(string name) =>
        db.Categories.FirstOrDefault(c => c.Name == name);
}
