namespace Okane.Application;

public class Expense : IEntity
{
    public int Id { get; set; }
    public int Amount { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public string? Description { get; set; }
}
