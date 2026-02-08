using FamilyTree.API;
using FamilyTree.API.Endpoints;
using FamilyTree.API.Middleware;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.AddStartServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ExceptionMiddleware();

app.MapPersonEndpoints();

app.MapFamilyTreeEndponts();

app.Run();

