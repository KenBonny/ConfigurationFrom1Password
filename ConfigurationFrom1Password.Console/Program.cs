using ConfigurationFrom1Password;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .AddInMemoryCollection([new("InMemorySensitiveData", "op://Private/1Password Config Extension Test/password")])
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Replace1PasswordSecrets();

var complexObject = builder.Configuration.GetSection("ComplexObject").Get<ComplexObject>();
Console.WriteLine("InMemory Sensitive Data: " + builder.Configuration["InMemorySensitiveData"]);
Console.WriteLine("Sensitive Data: " + builder.Configuration["SensitiveData"]);
Console.WriteLine("External Service: " + builder.Configuration["ExternalService"]);
Console.WriteLine("Connection String: " + builder.Configuration.GetConnectionString("DefaultConnection"));
Console.WriteLine("Non-sensitive Data: " + builder.Configuration["NonSensitiveData"]);
Console.WriteLine("Complex Object Name: " + complexObject.Name);
Console.WriteLine("Complex Object Value: " + complexObject.Value);
Console.WriteLine("Complex Object URL: " + complexObject.Url);
Console.WriteLine("Complex Object Tags: " + string.Join("", complexObject.Tags.Select(tag => $"{Environment.NewLine} - {tag}")));

record ComplexObject(string Name, int Value, Uri Url, List<string> Tags);
