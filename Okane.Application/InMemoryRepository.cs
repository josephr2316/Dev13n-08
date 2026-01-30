namespace Okane.Application;

public abstract class InMemoryRepository<T, TUpdateReq> : IRepository<T, TUpdateReq>
    where T : class, IEntity
{
    private int _lastId;
    protected readonly List<T> Items = [];

    public void Add(T entity)
    {
        if (entity.Id == 0)
            entity.Id = ++_lastId;
        else
            _lastId = Math.Max(_lastId, entity.Id);

        Items.Add(entity);
    }

    public T? ById(int id) =>
        Items.FirstOrDefault(x => x.Id == id);

    public IEnumerable<T> All() => Items;

    public void Remove(int id)
    {
        var existing = Items.First(x => x.Id == id);
        Items.Remove(existing);
    }

    public bool Exists(int id) =>
        Items.Any(x => x.Id == id);

    public abstract T Update(int id, TUpdateReq request);
}

public sealed class InMemoryCategoryRepository
    : InMemoryRepository<Category, UpdateCategoryRequest>, ICategoryRepository
{
    public Category? ByName(string name) =>
        Items.FirstOrDefault(c => c.Name == name);

    public override Category Update(int id, UpdateCategoryRequest request)
    {
        var existing = Items.First(c => c.Id == id);
        existing.Name = request.Name;
        return existing;
    }
}

public sealed class InMemoryExpenseRepository
    : InMemoryRepository<Expense, UpdateExpenseRequest>, IExpenseRepository
{
    public IEnumerable<Expense> ByCategoryId(int categoryId) =>
        Items.Where(e => e.CategoryId == categoryId);

    public override Expense Update(int id, UpdateExpenseRequest request)
    {
        var existing = Items.First(e => e.Id == id);
        existing.Amount = request.Amount;
        existing.CategoryId = request.CategoryId;
        existing.Description = request.Description;
        return existing;
    }
}
