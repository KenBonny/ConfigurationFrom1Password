using System.Collections.Frozen;
using Microsoft.Extensions.Configuration;

namespace ConfigurationFrom1Password.Tests;

public static class OnePasswordTestConfigurationExtension
{
    public static IConfigurationBuilder Replace1PasswordSecretsForTesting(this IConfigurationBuilder builder, Func<string, string> getSecretsForTesting)
    {
        builder.Add(new OnePasswodTestConfigurationSource(getSecretsForTesting));
        return builder;
    }
}

public class OnePasswodTestConfigurationSource(Func<string, string> getSecretsForTesting) : IConfigurationSource
{
    /// <inheritdoc />
    public IConfigurationProvider Build(IConfigurationBuilder builder) =>
        new OnePasswordConfigurationProvider(builder.Build(), getSecretsForTesting);
}

public static class TestData
{
    public const string Password = "Test Password w17h #";
    public const string Url = "https://www.google.com";
    public const string ConnectionString = "connection string value to access a database";

    private static readonly FrozenDictionary<string, string> Secrets = new Dictionary<string, string>()
    {
        ["op://Private/1Password Config Extension Test/password"] = Password + Environment.NewLine,
        ["op://Private/1Password Config Extension Test/url"] = Url + Environment.NewLine,
        ["op://Private/1Password Config Extension Test/connectionstring"] =
            ConnectionString + Environment.NewLine
    }.ToFrozenDictionary();
    
    public static Func<string, string> GetSecretsForTesting = key => Secrets[key];

    extension(IConfiguration config)
    {
        public async Task AreEqual(string key, string value) => await Assert.That(config[key]).IsEqualTo(value); 
    }
}