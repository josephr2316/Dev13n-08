namespace Okane.Application;

public class FileExpensesRepository : IRepository<Expense>
{
    private readonly string _filePath;

    public FileExpensesRepository(string filePath = "expenses.txt")
    {
        _filePath = filePath;
    }
    
    public void Add(Expense entity)
    {
        var lines = File.Exists(_filePath) 
            ? File.ReadAllLines(_filePath).Where(line => !string.IsNullOrWhiteSpace(line)).ToList() 
            : new List<string>();
        var lastId = 0;
        if (lines.Count > 0)
        {
            var parts = lines[^1].Split(',');
            lastId = int.Parse(parts[0]);
        }
        entity.Id = lastId + 1;

        var newLine = $"{entity.Id},{entity.Amount},{entity.CategoryName}";
        File.AppendAllText(_filePath, newLine + Environment.NewLine);
    }


    public Expense? ById(int id)
    {
        if (!File.Exists(_filePath))
            return null;
        
        foreach (var line in File.ReadLines(_filePath))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;
                
            var parts = line.Split(',');
            if (parts.Length >= 3)
            {
                var lineId = int.Parse(parts[0]);
                if (lineId == id)
                {
                    return new Expense 
                    { 
                        Id = lineId, 
                        Amount = int.Parse(parts[1]), 
                        CategoryName = parts[2] 
                    };
                }
            }
        }
        return null;
    }

    public IEnumerable<Expense> All()
    {
        if (!File.Exists(_filePath))
            return [];

        return File.ReadLines(_filePath)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line => {
                var parts = line.Split(',');
                return new Expense { Id = int.Parse(parts[0]), Amount = int.Parse(parts[1]), CategoryName = parts[2] };
            });
    }

    public void Remove(int id)
    {
        var lines = File.ReadAllLines(_filePath)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToList();
        var index = lines.FindIndex(line => int.Parse(line.Split(',')[0]) == id);
        lines.RemoveAt(index);
        File.WriteAllLines(_filePath, lines);
    }

    public bool Exists(int id)
    {
        if (!File.Exists(_filePath))
            return false;

        return File.ReadLines(_filePath)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Any(line =>
            {
                var parts = line.Split(',');
                return parts.Length > 0 && int.Parse(parts[0]) == id;
            });
    }

    public Expense Update(int id, UpdateExpenseRequest request)
    {
        var lines = File.ReadAllLines(_filePath)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToList();
        var index = lines.FindIndex(line => int.Parse(line.Split(',')[0]) == id);
        lines[index] = $"{id},{request.Amount},{request.CategoryName}";
        
        File.WriteAllLines(_filePath, lines);

        return new Expense
        {
            Id = id,
            Amount = request.Amount,
            CategoryName = request.CategoryName
        };
    }
}