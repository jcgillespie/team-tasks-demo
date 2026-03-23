using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using TeamTasks.Api.Data;
using TeamTasks.Api.Services;

var builder = WebApplication.CreateBuilder(args);

var keyVaultUri = builder.Configuration["KeyVault:VaultUri"];
if (!string.IsNullOrWhiteSpace(keyVaultUri))
{
    var secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());
    builder.Configuration.AddAzureKeyVault(secretClient, new Azure.Extensions.AspNetCore.Configuration.Secrets.AzureKeyVaultConfigurationOptions());
}

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var providerMode = HostedConfiguration.ResolveProviderMode(builder.Configuration, builder.Environment.EnvironmentName);
var connectionString = HostedConfiguration.ResolveConnectionString(builder.Configuration, providerMode);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    AppDbContext.ConfigureProvider(options, connectionString, providerMode);
});

builder.Services.AddScoped<ITaskService, TaskService>();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:5173"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientCors", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbInitializer.SeedAsync(dbContext);
}

app.UseHttpsRedirection();
app.UseCors("ClientCors");
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program;
