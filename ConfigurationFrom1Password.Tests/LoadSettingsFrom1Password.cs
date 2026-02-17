using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ConfigurationFrom1Password.Tests;

public class LoadSettingsFrom1Password
{
    [Test]
    public async Task ReplaceSingleSetting()
    {
        var dict = new Dictionary<string, string>
        {
            ["op://Private/1Password Config Extension Test/password"] = "Test Password w17h #\n",
            ["op://Private/1Password Config Extension Test/url"] = "https://www.google.com\n",
            ["op://Private/1Password Config Extension Test/connectionstring"] = "connection string value to access a database\n"
        };
        
        var builder = Host.CreateApplicationBuilder();

        builder.Configuration
            .AddInMemoryCollection([new ("SensitiveData", "op://Private/1Password Config Extension Test/password")])
            .Replace1PasswordSecretsForTesting(key => dict[key]);
        var secret = builder.Configuration["SensitiveData"];
        await Assert.That(secret).IsNotNull().And.IsEqualTo("Test Password w17h #");
    }
    
    [Test]
    public async Task Replace1PasswordSecretsButNotOthers()
    {
        var dict = new Dictionary<string, string>
        {
            ["op://Private/1Password Config Extension Test/password"] = "Test Password w17h #\n",
            ["op://Private/1Password Config Extension Test/url"] = "https://www.google.com\n",
            ["op://Private/1Password Config Extension Test/connectionstring"] = "connection string value to access a database\n"
        };

        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.AddInMemoryCollection([new ("SensitiveData", "op://Private/1Password Config Extension Test/password"),
            new ("NonSensitiveData", "non-sensitive value")])
            .Replace1PasswordSecretsForTesting(key => dict[key]);
        
        var secret = builder.Configuration["SensitiveData"];
        await Assert.That(secret).IsNotNull().And.IsEqualTo("Test Password w17h #");
        var nonSensitive = builder.Configuration["NonSensitiveData"];
        await Assert.That(nonSensitive).IsNotNull().And.IsEqualTo("non-sensitive value");
    }
}