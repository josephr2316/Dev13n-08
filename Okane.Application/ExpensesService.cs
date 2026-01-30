namespace Okane.Application;

public class ExpensesService(IExpenseRepository expenses, ICategoryRepository categories)
{
    public Result<ExpenseResponse> Create(CreateExpenseRequest request)
    {
        if (request.Amount < 1)
            return new ErrorResult<ExpenseResponse>(
                $"{nameof(request.Amount)} must be greater than 1.");

        var category = categories.ById(request.CategoryId);
        if (category is null)
            return new NotFoundResult<ExpenseResponse>(
                $"Category with id {request.CategoryId} was not found.");

        var expense = new Expense
        {
            Amount = request.Amount,
            CategoryId = category.Id,
            Category = category,
            Description = request.Description
        };
        expenses.Add(expense);

        var response = new ExpenseResponse(
            expense.Id,
            expense.Amount,
            category.Id,
            category.Name,
            expense.Description);
        return new OkResult<ExpenseResponse>(response);
    }

    public Result<ExpenseResponse> Retrieve(int expenseId)
    {
        var expense = expenses.ById(expenseId);

        if (expense is null)
            return new NotFoundResult<ExpenseResponse>(
                $"Expense with id {expenseId} was not found.");

        var categoryName = expense.Category?.Name ?? string.Empty;

        var response = new ExpenseResponse(
            expense.Id,
            expense.Amount,
            expense.CategoryId,
            categoryName,
            expense.Description);

        return new OkResult<ExpenseResponse>(response);
    }

    public Result<IEnumerable<ExpenseResponse>> All()
    {
        var response = expenses.All()
            .Select(expense => new ExpenseResponse(
                expense.Id,
                expense.Amount,
                expense.CategoryId,
                expense.Category?.Name ?? string.Empty,
                expense.Description));

        return new OkResult<IEnumerable<ExpenseResponse>>(response);
    }

    public Result<ExpenseResponse> Update(int id, UpdateExpenseRequest request)
    {
        if (request.Amount < 1)
            return new ErrorResult<ExpenseResponse>(
                $"{nameof(request.Amount)} must be greater than 1.");

        if (!expenses.Exists(id))
            return new NotFoundResult<ExpenseResponse>(
                $"Expense with id {id} was not found.");

        var category = categories.ById(request.CategoryId);
        if (category is null)
            return new NotFoundResult<ExpenseResponse>(
                $"Category with id {request.CategoryId} was not found.");

        var updated = expenses.Update(id, request);
        updated.Category = category;

        var response = new ExpenseResponse(
            updated.Id,
            updated.Amount,
            updated.CategoryId,
            category.Name,
            updated.Description);
        return new OkResult<ExpenseResponse>(response);
    }

    public Result Delete(int id)
    {
        if (!expenses.Exists(id))
            return new NotFoundResult(
                $"Expense with id {id} was not found.");

        expenses.Remove(id);

        return new OkResult();
    }
}
