using ComputerRepairSystem.domain.data;
using ComputerRepairSystem.domain.entities;
using ComputerRepairSystem.company.Data;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MasterErpDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("MasterErp")));
builder.Services.AddDbContext<TenantDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("TenantErp")));
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapPost("/companies", async (

  Company company,

  MasterErpDbContext db) =>

{

    db.Companies.Add(company);

    await db.SaveChangesAsync();



    return Results.Created($"/companies/{company.CompanyId}", company);

});

app.MapControllers();

app.Run();
