using FamilyTree.API;

var builder = WebApplication.CreateBuilder(args);

builder.AddStartServices();

var app = builder.Build();

app.Run();