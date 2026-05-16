using InterportCargo.Web.Data;
using InterportCargo.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Razor Pages
builder.Services.AddRazorPages();

// EF Core with SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=interportcargo.db"));

// Cookie authentication — two roles: Customer, QuotationOfficer
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/CustomerLogin";
        options.AccessDeniedPath = "/Index";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddAuthorization();

// HR API integration — base URL configured in appsettings.json
builder.Services.AddHttpClient<IHrApiService, HrApiService>(client =>
{
    var baseUrl = builder.Configuration["HrApi:BaseUrl"] ?? "http://localhost:5100/";
    client.BaseAddress = new Uri(baseUrl);
});

var app = builder.Build();

// Apply migrations and create DB on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();

// Expose Program for integration testing
public partial class Program { }
