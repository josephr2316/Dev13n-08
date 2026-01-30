using Microsoft.EntityFrameworkCore;
using Okane.Application;

namespace Okane.Storage.EntityFramework;

public class ExpensesRepository(OkaneDbContext db) : IExpenseRepository
{
    public void Add(Expense entity)
    {
        db.Expenses.Add(entity);
        db.SaveChanges();
    }

    public Expense? ById(int id) =>
        db.Expenses
            .Include(expense => expense.Category)
            .FirstOrDefault(expense => expense.Id == id);

    public IEnumerable<Expense> All() =>
        db.Expenses
            .Include(expense => expense.Category);

    public void Remove(int id)
    {
        var expense = db.Expenses.First(x => x.Id == id);
        db.Expenses.Remove(expense);
        db.SaveChanges();
    }

    public bool Exists(int id) =>
        db.Expenses.Any(x => x.Id == id);

    public Expense Update(int id, UpdateExpenseRequest request)
    {
        var existing = db.Expenses.First(expense => expense.Id == id);

        existing.Amount = request.Amount;
        existing.CategoryId = request.CategoryId;
        existing.Description = request.Description;

        db.SaveChanges();
        return existing;
    }

    public IEnumerable<Expense> ByCategoryId(int categoryId) =>
        db.Expenses
            .Include(expense => expense.Category)
            .Where(expense => expense.CategoryId == categoryId);
}
