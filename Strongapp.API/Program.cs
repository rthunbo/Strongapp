using Strongapp.API.Database;
using Strongapp.API.Repositories;
using Strongapp.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<StrongDatabaseSettings>(
    builder.Configuration.GetSection("StrongDatabase"));

builder.Services.AddSingleton<ExerciseRepository>();
builder.Services.AddSingleton<WorkoutRepository>();
builder.Services.AddSingleton<TemplateRepository>();
builder.Services.AddSingleton<FolderRepository>();
builder.Services.AddSingleton<MeasurementRepository>();

builder.Services.AddSingleton<WorkoutService>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

await DbInitializer.Seed(app);

app.Run();
