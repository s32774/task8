using Microsoft.EntityFrameworkCore;
using task8.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UniversityTasksDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();