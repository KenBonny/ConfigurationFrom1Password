using ConfigurationFrom1Password;

var builder = WebApplication.CreateBuilder(args);

// Load configuration and replace sensitive values from 1Password.
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json")
    .AddInMemoryCollection([ new ("SensitiveData", "op://Private/1Password Config Extension Test/password")]);
    
if (builder.Environment.IsDevelopment())
    builder.Configuration.Replace1PasswordSecrets();

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

app.MapControllers();

app.Run();