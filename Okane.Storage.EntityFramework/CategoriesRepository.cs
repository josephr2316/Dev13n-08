using Okane.Application;

namespace Okane.Storage.EntityFramework
{
    public class CategoriesRepository(OkaneDbContext db) : ICategoriesRepository
    {
        public void Add(Category entity)
        {
            db.Categories.Add(entity);
            db.SaveChanges();
        }

        public Category? ById(int id) =>
            db.Categories
                .FirstOrDefault(x => x.Id == id);

        public IEnumerable<Category> All() =>
            db.Categories;

        public void Remove(int id)
        {
            var category = db.Categories.First(x => x.Id == id);
            db.Categories.Remove(category);
            db.SaveChanges();
        }

        public bool Exists(int id) => 
            db.Categories.Any(x => x.Id == id);

        public Category? ByName(string name) =>
            db.Categories.FirstOrDefault(category => category.Name == name);
    }
}