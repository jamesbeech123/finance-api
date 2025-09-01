using Microsoft.EntityFrameworkCore;
using FinanceApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
builder.Services.AddOpenApi();
builder.Services.AddDbContext<WealthContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("WealthDb")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseDefaultFiles(); 
app.UseStaticFiles();  
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<WealthContext>();

    
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();

    // Seed sample data if empty
    if (!context.Clients.Any())
    {
        
        var client = new Client
        {
            FullName = "Alice UltraHighNet",
            NetWorth = 50000000,
            Portfolios = new List<Portfolio>()
        };

        
        var portfolio = new Portfolio
        {
            Name = "Global Growth Fund",
            TotalInvestment = 12000000,
            Client = client,
            Investments = new List<Investment>()
        };
        client.Portfolios.Add(portfolio);

        
        var investments = new List<Investment>
        {
            new Investment { AssetName = "Apple Stock", AssetType = "Stock", Units = 5000000, PurchasePrice = 150, CurrentPrice = 160, PurchaseDate = DateTime.Now, Portfolio = portfolio },
            new Investment { AssetName = "Government Bonds", AssetType = "Bond", Units = 2000000, PurchasePrice = 100, CurrentPrice = 102, PurchaseDate = DateTime.Now, Portfolio = portfolio },
            new Investment { AssetName = "Private Equity Fund", AssetType = "ETF", Units = 5000000, PurchasePrice = 50, CurrentPrice = 55, PurchaseDate = DateTime.Now, Portfolio = portfolio }
        };
        portfolio.Investments = investments;

        
        context.Clients.Add(client);
        context.SaveChanges();
    }

    
    var clientsWithData = context.Clients
        .Include(c => c.Portfolios)
            .ThenInclude(p => p.Investments)
        .ToList();

    
    foreach (var c in clientsWithData)
    {
        Console.WriteLine($"Client: {c.FullName}, Portfolios: {c.Portfolios.Count}");
        foreach (var p in c.Portfolios)
        {
            Console.WriteLine($"  Portfolio: {p.Name}, Investments: {p.Investments.Count}");
        }
    }
}

app.Run();
