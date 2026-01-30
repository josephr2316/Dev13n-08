namespace Okane.Application;

public interface IExpenseRepository : IRepository<Expense, UpdateExpenseRequest>
{
    IEnumerable<Expense> ByCategoryId(int categoryId);
}
