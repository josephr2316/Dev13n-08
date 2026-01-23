namespace Okane.Application;

public class ExpensesService(List<Expense> expenses)
{
    private static int _lastId = 1;

    public Expense Create(CreateExpenseRequest request)
    {
        var (amount, categoryName) = request;
        var id = _lastId++;
        var expense = new Expense(id, amount, categoryName);
        expenses.Add(expense);
        return expense;
    }

    public Expense? Retrieve(int expenseId)
    {
        var expense = expenses.FirstOrDefault(e => e.Id == expenseId);
        return expense;
    }

    public IEnumerable<Expense> All() => expenses;

    public Expense? Update(int expenseId, UpdateExpenseRequest request)
    {
        var expense = expenses.FirstOrDefault(e => e.Id == expenseId);
        if (expense == null)
            return null;

        var (amount, categoryName) = request;
        var updatedExpense = new Expense(expenseId, amount, categoryName);
        
        var index = expenses.FindIndex(e => e.Id == expenseId);
        if (index >= 0)
        {
            expenses[index] = updatedExpense;
        }
        
        return updatedExpense;
    }

    public bool Delete(int expenseId)
    {
        var expense = expenses.FirstOrDefault(e => e.Id == expenseId);
        if (expense == null)
            return false;

        expenses.Remove(expense);
        return true;
    }
}