using Microsoft.EntityFrameworkCore;
using RunningApplicationNew.DataLayer;
using RunningApplicationNew.IRepository;
using RunningApplicationNew.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Swagger ve OpenAPI ayarlarý
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext'i ekle
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repository katmanlarýný DI'ye ekle
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();

// JwtHelper sýnýfýný DI'ye ekle
builder.Services.AddScoped<IJwtHelper, JwtHelper>();

var app = builder.Build();

// Swagger yapýlandýrmasý
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
