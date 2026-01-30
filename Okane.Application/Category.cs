namespace Okane.Application;

public class Category : IEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<Expense> Expenses { get; set; } = [];
}
