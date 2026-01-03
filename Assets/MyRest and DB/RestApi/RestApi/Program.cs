using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using RestApi.Data;
using System.Data.SqlTypes;

var builder = WebApplication.CreateBuilder(args);

//SQLite
builder.Services.AddDbContext<API_Context>(opt =>
    opt.UseSqlite("Data Source=myapp.db"));

builder.Services.AddControllers();

//CORS Unity
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowUnity",
        policy => policy.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//Create database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<API_Context>();
    context.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowUnity"); //Enable CORS
app.UseAuthorization();
app.MapControllers();

app.Run();