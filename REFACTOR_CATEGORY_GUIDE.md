# Guía: Refactor de CategoryName → tabla Category y Expense.CategoryId

Sigue estos pasos en orden. Cada sección muestra el código que debes añadir o modificar.

---

## Paso 1: Entidad Category (Okane.Application)

**Crear** `Okane.Application/Category.cs`:

```csharp
namespace Okane.Application;

public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }
}
```

---

## Paso 2: Modificar Expense (Okane.Application)

En `Expense.cs`:
- Quitar la propiedad `CategoryName`.
- Añadir `CategoryId` (int) y la propiedad de navegación `Category`.

```csharp
namespace Okane.Application;

public class Expense
{
    public int Id { get; set; }
    public int Amount { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }  // navegación
    public string? Description { get; set; }
}
```

---

## Paso 3: Interfaz ICategoryRepository (Okane.Application)

**Crear** `Okane.Application/ICategoryRepository.cs`:

```csharp
namespace Okane.Application;

public interface ICategoryRepository
{
    Category? ById(int id);
    Category? GetByName(string name);
    void Add(Category category);
}
```

La API puede seguir recibiendo el nombre de categoría; el servicio/repositorio se encargará de buscar o crear la categoría y asignar `CategoryId`.

---

## Paso 4: Implementación en memoria (tests) – Okane.Application

**Crear** `Okane.Application/InMemoryCategoryRepository.cs`:

```csharp
namespace Okane.Application;

public class InMemoryCategoryRepository : ICategoryRepository
{
    private int _lastId;
    private readonly List<Category> _categories = [];

    public Category? ById(int id) =>
        _categories.FirstOrDefault(c => c.Id == id);

    public Category? GetByName(string name) =>
        _categories.FirstOrDefault(c => c.Name == name);

    public void Add(Category category)
    {
        category.Id = ++_lastId;
        _categories.Add(category);
    }
}
```

---

## Paso 5: InMemoryRepository (Okane.Application)

En `InMemoryRepository.cs`:
- El constructor debe recibir `ICategoryRepository`.
- En `Update`: en vez de asignar `CategoryName`, buscar o crear la categoría por nombre y asignar `existing.CategoryId = category.Id`.

Ejemplo de constructor y `Update`:

```csharp
private readonly ICategoryRepository _categories;

public InMemoryRepository(ICategoryRepository categories)
{
    _categories = categories;
}

// En Update():
var category = _categories.GetByName(request.CategoryName);
if (category is null)
{
    category = new Category { Name = request.CategoryName };
    _categories.Add(category);
}
existing.Amount = request.Amount;
existing.CategoryId = category.Id;
```

---

## Paso 6: ExpensesService (Okane.Application)

En `ExpensesService`:
- Inyectar también `ICategoryRepository` (constructor con `expenses` y `categories`).
- En **Create**: obtener o crear la categoría por `request.CategoryName`, asignar `expense.CategoryId = category.Id`, y construir la respuesta con `category.Name` como nombre para el DTO.
- En **Retrieve**, **All** y **Update**: el `ExpenseResponse` sigue teniendo `CategoryName`; rellenarlo desde `expense.Category?.Name` si cargas la relación, o desde `categories.ById(expense.CategoryId)?.Name` si no (p. ej. en tests en memoria).

Ejemplo para obtener el nombre de categoría al construir la respuesta:

```csharp
// Necesitas una forma de obtener el nombre: o bien expense.Category está cargado (Include),
// o bien llamas a categories.ById(expense.CategoryId)?.Name
private string CategoryNameFrom(Expense expense) =>
    expense.Category?.Name ?? _categories.ById(expense.CategoryId)?.Name ?? "";
```

Usa `CategoryNameFrom(expense)` donde antes usabas `expense.CategoryName` al crear `ExpenseResponse`.

En **Create**, antes de `expenses.Add(expense)`:

```csharp
var category = _categories.GetByName(request.CategoryName);
if (category is null)
{
    category = new Category { Name = request.CategoryName };
    _categories.Add(category);
}
var expense = new Expense
{
    Amount = request.Amount,
    CategoryId = category.Id,
    Description = request.Description
};
_expenses.Add(expense);
// Para la respuesta usa category.Name (ya lo tienes)
```

No modifiques los DTOs: `CreateExpenseRequest`, `UpdateExpenseRequest` y `ExpenseResponse` pueden seguir usando `CategoryName`; solo cambia de dónde sale ese valor (de la entidad Category).

---

## Paso 7: DbContext y relación (Okane.Storage.EntityFramework)

En `OkaneDbContext.cs`:
- Añadir `DbSet<Category> Categories`.
- En `OnModelCreating`, configurar la relación uno a muchos: un `Expense` tiene una `Category`, muchas `Expense` pueden apuntar a la misma `Category`.

Ejemplo:

```csharp
public DbSet<Category> Categories => Set<Category>();

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Expense>()
        .HasOne(e => e.Category)
        .WithMany()
        .HasForeignKey(e => e.CategoryId)
        .OnDelete(DeleteBehavior.Restrict);

    modelBuilder.ApplyConfigurationsFromAssembly(typeof(OkaneDbContext).Assembly);
}
```

`WithMany()` sin colección en `Category` es suficiente si no navegas de Category a Expenses.

---

## Paso 8: CategoryRepository (Okane.Storage.EntityFramework)

**Crear** `Okane.Storage.EntityFramework/CategoryRepository.cs`:

```csharp
using Okane.Application;

namespace Okane.Storage.EntityFramework;

public class CategoryRepository(OkaneDbContext db) : ICategoryRepository
{
    public Category? ById(int id) =>
        db.Categories.Find(id);

    public Category? GetByName(string name) =>
        db.Categories.FirstOrDefault(c => c.Name == name);

    public void Add(Category category)
    {
        db.Categories.Add(category);
        db.SaveChanges();
    }
}
```

---

## Paso 9: ExpensesRepository (Okane.Storage.EntityFramework)

En `ExpensesRepository.cs`:
- En `ById` y `All`: usar `.Include(e => e.Category)` para que al mapear a `ExpenseResponse` tengas `expense.Category.Name`.
- En `Update`: en vez de `existing.CategoryName = request.CategoryName`, buscar o crear la categoría por `request.CategoryName` en `db.Categories` y asignar `existing.CategoryId = category.Id`.

Ejemplo para `ById` y `All`:

```csharp
public Expense? ById(int id) =>
    db.Expenses.Include(e => e.Category).FirstOrDefault(x => x.Id == id);

public IEnumerable<Expense> All() =>
    db.Expenses.Include(e => e.Category);
```

Ejemplo para `Update`:

```csharp
var category = db.Categories.FirstOrDefault(c => c.Name == request.CategoryName);
if (category is null)
{
    category = new Category { Name = request.CategoryName };
    db.Categories.Add(category);
    db.SaveChanges();
}
existing.Amount = request.Amount;
existing.CategoryId = category.Id;
```

---

## Paso 10: Registro en DI (Okane.WebApi)

En `Program.cs`, registrar el repositorio de categorías:

```csharp
.AddTransient<ICategoryRepository, CategoryRepository>()
```

(Justo antes o después de `AddTransient<IRepository<Expense>, ExpensesRepository>()`.)

---

## Paso 11: Tests (Okane.Tests)

En el constructor de `ExpensesServiceTests`, el servicio necesita también un `ICategoryRepository`. Crea un `InMemoryCategoryRepository` y pásalo tanto al `InMemoryRepository<Expense>` como al `ExpensesService`:

```csharp
var categories = new InMemoryCategoryRepository();
_service = new ExpensesService(new InMemoryRepository<Expense>(categories), categories);
```

---

## Paso 12: Migración (sin perder datos)

Orden recomendado en la migración:

1. Crear la tabla **Categories** (Id, Name).
2. Añadir a **Expenses** la columna **CategoryId** (nullable al principio).
3. Rellenar **Categories**: `INSERT INTO "Categories" ("Name") SELECT DISTINCT "CategoryName" FROM "Expenses"`.
4. Asignar **CategoryId**: `UPDATE "Expenses" SET "CategoryId" = (SELECT "Id" FROM "Categories" WHERE "Categories"."Name" = "Expenses"."CategoryName")`.
5. Hacer **CategoryId** NOT NULL y crear la FK de `Expenses.CategoryId` → `Categories.Id`.
6. Eliminar la columna **CategoryName** de **Expenses**.

Puedes generar la migración con:

```bash
dotnet ef migrations add AddCategoriesAndExpenseCategoryId --project Okane.Storage.EntityFramework --startup-project Okane.WebApi
```

y luego **editar manualmente** el `Up` de la migración generada para incluir los pasos 3 y 4 (los `INSERT` y `UPDATE` con `migrationBuilder.Sql(...)`), y asegurarte de que primero añades `CategoryId` nullable, migras datos, y después la pasas a NOT NULL y añades la FK antes de borrar `CategoryName`.

Si prefieres una migración simple (y aceptas perder los datos actuales de categoría), basta con crear la tabla Categories, añadir CategoryId y la FK; en ese caso no hace falta el `INSERT`/`UPDATE` ni conservar `CategoryName`.

---

## Resumen de archivos

| Acción   | Archivo / Ubicación |
|----------|----------------------|
| Crear    | `Okane.Application/Category.cs` |
| Crear    | `Okane.Application/ICategoryRepository.cs` |
| Crear    | `Okane.Application/InMemoryCategoryRepository.cs` |
| Crear    | `Okane.Storage.EntityFramework/CategoryRepository.cs` |
| Modificar| `Okane.Application/Expense.cs` (CategoryId + Category, quitar CategoryName) |
| Modificar| `Okane.Application/InMemoryRepository.cs` (inyección ICategoryRepository, Update con CategoryId) |
| Modificar| `Okane.Application/ExpensesService.cs` (inyección ICategoryRepository, Create/Update y respuestas con categoría) |
| Modificar| `Okane.Storage.EntityFramework/OkaneDbContext.cs` (DbSet Categories + relación) |
| Modificar| `Okane.Storage.EntityFramework/ExpensesRepository.cs` (Include(Category), Update con CategoryId) |
| Modificar| `Okane.WebApi/Program.cs` (registro ICategoryRepository) |
| Modificar| `Okane.Tests/ExpensesServiceTests.cs` (constructor con InMemoryCategoryRepository) |
| Crear/editar | Migración AddCategoriesAndExpenseCategoryId (tabla Categories, columna CategoryId, opcionalmente migración de datos) |

Si en algún paso quieres el código completo de un archivo concreto, dime cuál y te lo pongo entero aquí.
