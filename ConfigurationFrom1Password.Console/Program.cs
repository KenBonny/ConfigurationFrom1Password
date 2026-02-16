using ConfigurationFrom1Password;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .AddInMemoryCollection([new("InMemorySensitiveData", "op://Private/1Password Config Extension Test/password")])
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Replace1PasswordSecrets();

Console.WriteLine(builder.Configuration["InMemorySensitiveData"]);
Console.WriteLine(builder.Configuration["SensitiveData"]);
Console.WriteLine(builder.Configuration["ExternalService"]);
Console.WriteLine(builder.Configuration.GetConnectionString("DefaultConnection"));