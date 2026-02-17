using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ConfigurationFrom1Password.Tests;

public class LoadSettings
{
    [Test]
    public async Task ReplaceSingleSetting()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration
            .AddInMemoryCollection([new ("SensitiveData", "op://Private/1Password Config Extension Test/password")])
            .Replace1PasswordSecretsForTesting(TestData.GetSecretsForTesting);

        await builder.Configuration.AreEqual("SensitiveData", TestData.Password);
    }
    
    [Test]
    public async Task Replace1PasswordSecretsButNotOthers()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration
            .AddInMemoryCollection(
            [
                new("SensitiveData", "op://Private/1Password Config Extension Test/password"),
                new("NonSensitiveData", "non-sensitive value")
            ])
            .Replace1PasswordSecretsForTesting(TestData.GetSecretsForTesting);
        
        await builder.Configuration.AreEqual("SensitiveData", TestData.Password);
        await builder.Configuration.AreEqual("NonSensitiveData", "non-sensitive value");
    }
}