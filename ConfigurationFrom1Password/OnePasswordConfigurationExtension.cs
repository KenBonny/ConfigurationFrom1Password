using System.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace ConfigurationFrom1Password;

public static class OnePasswordConfigurationExtension
{
    public static IConfigurationBuilder Replace1PasswordSecrets(this IConfigurationBuilder builder)
    {
        builder.Add(new OnePasswodConfigurationSource());
        return builder;
    }
}

public class OnePasswodConfigurationSource() : IConfigurationSource
{
    /// <inheritdoc />
    public IConfigurationProvider Build(IConfigurationBuilder builder) => new OnePasswordConfigurationProvider(builder.Build());
}

public class OnePasswordConfigurationProvider : ConfigurationProvider
{
    private readonly IConfigurationRoot _configurationRoot;
    private readonly Func<string, string> _getSecret;

    public OnePasswordConfigurationProvider(IConfigurationRoot configurationRoot) : this(configurationRoot, ReadSecretFrom1Password) { }

    /// <summary>
    /// Use this constructor if you want to mock the secret retrieval.
    /// This is mainly used in testing scenarios.
    /// The constructor with only IConfigurationBuilder is used in production scenarios
    /// and automatically gets the secret from 1Password.
    /// </summary>
    /// <param name="configurationRoot"></param>
    /// <param name="getSecret"></param>
    public OnePasswordConfigurationProvider(IConfigurationRoot configurationRoot, Func<string, string> getSecret)
    {
        _getSecret = getSecret;
        _configurationRoot = configurationRoot;
    }

    /// <inheritdoc />
    public override void Load()
    {
        var onePasswordConfiguration = _configurationRoot.AsEnumerable()
            .Where(c => c.Value is not null && c.Value.StartsWith("op://"))
            .Select(c => (c.Key, Value: _getSecret(c.Value!).TrimEnd(Environment.NewLine.ToCharArray())))
            .ToArray();
        // Task.WaitAll(onePasswordConfiguration);

        foreach (var task in onePasswordConfiguration)
        {
            Data[task.Key] = task.Value;
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