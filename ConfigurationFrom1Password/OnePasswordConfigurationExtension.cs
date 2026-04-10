using System.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace ConfigurationFrom1Password;

/// <summary>
/// Extension methods for replacing 1Password secrets in configuration.
/// </summary>
public static class OnePasswordConfigurationExtension
{
    /// <summary>
    /// Replace all 1Password secrets in the configuration with the actual values.
    /// </summary>
    /// <param name="builder">The configuration builder to extend with 1Password secret replacement</param>
    /// <returns>The configuration builder with 1Password secrets replaced</returns>
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IConfigurationBuilder Replace1PasswordSecrets(this IConfigurationBuilder builder)
    {
        builder.Add(new OnePasswodConfigurationSource());
        return builder;
    }
}

public class OnePasswodConfigurationSource : IConfigurationSource
{
    /// <inheritdoc />
    public IConfigurationProvider Build(IConfigurationBuilder builder) => new OnePasswordConfigurationProvider(builder);
}

public class OnePasswordConfigurationProvider : ConfigurationProvider
{
    private readonly IConfigurationRoot _configurationRoot;

    public OnePasswordConfigurationProvider(IConfigurationBuilder builder) => _configurationRoot = builder.Build();

    /// <inheritdoc />
    public override void Load()
    {
        Func<KeyValuePair<string, string?>, bool> is1PasswordValue = c => c.Value is not null && c.Value.StartsWith("op://");
        var lineEnding = Environment.NewLine.ToCharArray();
        var secrets = _configurationRoot.AsEnumerable()
            .Where(is1PasswordValue)
            .Select(c => c.Value!)
            .Distinct()
            .ToDictionary(key => key, key => ReadSecretFrom1Password(key).TrimEnd(lineEnding));
        
        var onePasswordConfiguration = _configurationRoot.AsEnumerable()
            .Where(is1PasswordValue)
            .Select(c => (c.Key, Value: secrets[c.Value!]));
        
        foreach (var config in onePasswordConfiguration)
        {
            Data[config.Key] = config.Value;
        }
    }

    private static string ReadSecretFrom1Password(string key)
    {
        var read1PasswordKeyInfo = new ProcessStartInfo("op.exe", ["read", key])
        {
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };
        var read1PasswordKeyProcess = Process.Start(read1PasswordKeyInfo);
        
        if (read1PasswordKeyProcess is null)
            throw new InvalidOperationException("Failed to start op.exe");
        
        read1PasswordKeyProcess.WaitForExit();
        return read1PasswordKeyProcess.StandardOutput.ReadToEnd();
    }
}