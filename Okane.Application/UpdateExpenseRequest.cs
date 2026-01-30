namespace Okane.Application;

public record UpdateExpenseRequest(int Amount, int CategoryId, string? Description = null);
