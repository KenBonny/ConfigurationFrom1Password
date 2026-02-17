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