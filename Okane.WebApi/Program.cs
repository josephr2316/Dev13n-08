using Okane.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOpenApi()
    .AddTransient<ExpensesService>()
    .AddSingleton<List<Expense>>();

var app = builder.Build();
    
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/expenses", 
    (ExpensesService service, CreateExpenseRequest request) => 
        Results.Created("/expenses", service.Create(request)));

app.MapGet("/expenses/{id}", 
    (ExpensesService service, int id) =>
    {
        var response = service.Retrieve(id);
        
        // TODO: Remove this logic
        if  (response == null)
            return Results.NotFound();
        
        return Results.Ok(response);
    });

app.MapGet("/expenses", (ExpensesService service) => Results.Ok(service.All()));

app.MapPut("/expenses/{id}", 
    (ExpensesService service, int id, UpdateExpenseRequest request) =>
    {
        var response = service.Update(id, request);
        
        if (response == null)
            return Results.NotFound();
        
        return Results.Ok(response);
    });

app.MapDelete("/expenses/{id}", 
    (ExpensesService service, int id) =>
    {
        var deleted = service.Delete(id);
        
        if (!deleted)
            return Results.NotFound();
        
        return Results.NoContent();
    });

app.Run();