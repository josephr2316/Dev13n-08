namespace Okane.Application;

public record CreateExpenseRequest(int Amount, int CategoryId, string? Description = null);
