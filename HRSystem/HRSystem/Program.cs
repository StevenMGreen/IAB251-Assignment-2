using Microsoft.EntityFrameworkCore;
using HRSystem.Data;

var builder = WebApplication.CreateBuilder(args);

// ── Services ───────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title       = "HR System API",
        Version     = "v1",
        Description = "Enterprise HR System – Employee and Department data. " +
                      "This API is provided as a read-only service for other " +
                      "enterprise systems (e.g. Quotation Module) to consume."
    });
});

// ── Database ───────────────────────────────────────────────────────
// Uses SQLite. The database file is created automatically in the project folder.
builder.Services.AddDbContext<HRContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("HRDatabase")));

// ── CORS ───────────────────────────────────────────────────────────
// Allow the Quotation system (running on a different port) to call this API.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowQuotationSystem", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ── Middleware Pipeline ────────────────────────────────────────────
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowQuotationSystem");
app.MapControllers();

// ── Ensure DB is created and seeded on startup ─────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HRContext>();
    db.Database.EnsureCreated();
}

app.Run();
