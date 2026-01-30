namespace Okane.Application;

public record ExpenseResponse(int Id, int Amount, int CategoryId, string CategoryName, string? Description);
