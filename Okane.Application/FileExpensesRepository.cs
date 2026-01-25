namespace Okane.Application;

public class FileExpensesRepository : IRepository<Expense>
{
    private readonly string _filePath = "expenses.txt";


    public void Add(Expense entity)
    {
        var lines = File.Exists(_filePath) ? File.ReadAllLines(_filePath).toList() : [];
        var lastId = lines.Length > 0 ? int.Parse(lines[^1].Split(',')[0]) : 0;
        // new List<string>() // Array.Empty<string>()
        / * var maxId = lines
            .Select(line => int.Parse(line.Split(',')[0]))
            .Max();*/
        entity.Id = lastId + 1;

        var newLine = $"{entity.Id},{entity.Amount},{entity.CategoryName}\n";
        File.AppendAllText(_filePath, newLine + Environment.NewLine);
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