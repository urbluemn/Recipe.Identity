var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentityServer()
    .AddInMemoryApiResources()
    .AddInMemoryIdentityResources()
    .AddInMemoryApiScopes()
    .AddInMemoryClients()
    .AddDeveloperSigningCredential();

var app = builder.Build();

app.UseRouting();
app.UseIdentityServer();

app.Run();
