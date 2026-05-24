using Microsoft.EntityFrameworkCore;
using task8.Data;
using task8.Services;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<UniversityTasksDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ISubmissionService, SubmissionService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();