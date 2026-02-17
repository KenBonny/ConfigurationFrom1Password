using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ConfigurationFrom1Password.Tests;

public class RoughSpeedCheckTest
{
    [Test]
    public Task LoadFrom1Password()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.AddJsonFile("LoadSettingsFrom1Password.json").Replace1PasswordSecrets();
        return Task.CompletedTask;
    }
}