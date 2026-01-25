namespace Okane.Application;

public class FileExpensesRepository : IRepository<Expense>
{
    public void Add(Expense entity)
    {
        throw new NotImplementedException();
    }

    public Expense? ById(int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Expense> All()
    {
        throw new NotImplementedException();
    }

    public void Remove(int id)
    {
        throw new NotImplementedException();
    }

    public bool Exists(int id)
    {
        throw new NotImplementedException();
    }

    public Expense Update(int id, UpdateExpenseRequest request)
    {
        throw new NotImplementedException();
    }
}