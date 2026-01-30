using Microsoft.EntityFrameworkCore;
using Okane.Application;
using Okane.Storage.EntityFramework;
using Okane.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOpenApi()
    .AddTransient<ExpensesService>()
    .AddTransient<IExpenseRepository, ExpensesRepository>()
    .AddTransient<ICategoryRepository, CategoriesRepository>()
    .AddDbContext<OkaneDbContext>(options => 
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("Default"),
            npgsql => npgsql.MigrationsAssembly(typeof(OkaneDbContext).Assembly.FullName)));

var app = builder.Build();
    
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/expenses", 
    (ExpensesService service, CreateExpenseRequest request) => 
        service.Create(request).ToHttpResult());

app.MapGet("/expenses/{id}", 
    (ExpensesService service, int id) => service.Retrieve(id).ToHttpResult());

app.MapGet("/expenses", (ExpensesService service) => service.All().ToHttpResult());

app.MapPut("/expenses/{id}", 
    (ExpensesService service, int id, UpdateExpenseRequest request) => service.Update(id, request).ToHttpResult());

app.MapDelete("/expenses/{id}", 
    (ExpensesService service, int id) => service.Delete(id).ToHttpResult());

app.Run();
