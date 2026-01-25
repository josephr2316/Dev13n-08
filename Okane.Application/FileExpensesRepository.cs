namespace Okane.Application;

public class FileExpensesRepository : IRepository<Expense>
{
    private readonly string _filePath = "expenses.txt";
    
    public void Add(Expense entity)
    {
        var lines = File.Exists(_filePath) ? File.ReadAllLines(_filePath).ToList() : new List<string>();
        var lastId = lines.Count > 0 ? int.Parse(lines[^1].Split(',')[0]) : 0;
        // new List<string>() // Array.Empty<string>()
        /*var maxId = lines
            .Select(line => int.Parse(line.Split(',')[0]))
            .Max();*/
        entity.Id = lastId + 1;

        var newLine = $"{entity.Id},{entity.Amount},{entity.CategoryName}\n";
        File.AppendAllText(_filePath, newLine + Environment.NewLine);
    }


    public Expense? ById(int id)
    {
        if (!File.Exists(_filePath))
            return null;

        foreach (var line in File.ReadLines(_filePath)){
            var parts = line.Split(',');
            if (parts.Length > 0 && parts[0] == id.ToString()){
                return new Expense { Id = int.Parse(parts[0]), Amount = int.Parse(parts[1]), CategoryName = parts[2] };
            }
        }

        return !File.Exists(_filePath) ? null : File.ReadLines(_filePath).
            Select(line =>
                {
                    var parts = line.Split(',');
                    return new Expense
                    {
                        Id = int.Parse(parts[0]), Amount = int.Parse(parts[1]), CategoryName = parts[2]
                    };
                }
            ).FirstOrDefault(e => e.Id == id);
    }

    public IEnumerable<Expense> All()
    {
        if (!File.Exists(_filePath))
            return [];

        return File.ReadLines(_filePath).Select(line => {
            var parts = line.Split(',');
            return new Expense { Id = int.Parse(parts[0]), Amount = int.Parse(parts[1]), CategoryName = parts[2] };
        });

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