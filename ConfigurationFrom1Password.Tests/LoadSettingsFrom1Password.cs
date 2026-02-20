using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ConfigurationFrom1Password.Tests;

record ComplexObject(string Name, int Value, Uri Url, string[] Tags);

[Explicit]
public class LoadSettingsFrom1Password
{
    private static HostApplicationBuilder _builder;

    [Before(Class)]
    public static async Task ClassSetup(ClassHookContext context, CancellationToken cancellationToken)
    {
        _builder = Host.CreateApplicationBuilder();
        _builder.Configuration.AddJsonFile("LoadSettingsFrom1Password.json").Replace1PasswordSecrets();
    }
    
    [Test]
    [Arguments("NonSensitiveData", "non-sensitive-value")]
    [Arguments("SensitiveData", TestData.Password)]
    [Arguments("ExternalService", TestData.Url)]
    public Task CheckPasswordIsLoaded(string key, string expectedValue) => _builder.Configuration.AreEqual(key, expectedValue);

    [Test]
    public async Task CheckConnectionStringIsLoaded() =>
        await Assert.That(_builder.Configuration.GetConnectionString("DefaultConnection"))
            .IsEqualTo(TestData.ConnectionString);
    
    [Test]
    public async Task CheckComplexObjectIsLoaded()
    {
        var expected = new ComplexObject(
            "ComplexObject",
            1,
            new Uri(TestData.Url),
            [TestData.Url, "Item 2"]);
        
        var fromConfig = _builder.Configuration.GetSection("ComplexObject").Get<ComplexObject>();
        await Assert.That(fromConfig).IsEqualTo(expected).IgnoringType<string[]>();
        await Assert.That(fromConfig.Tags).IsEquivalentTo(expected.Tags);
        await Assert.That(fromConfig).IsEquivalentTo(expected);
    }

    [Test]
    public async Task CheckOtpIsLoaded() =>
        await Assert.That(_builder.Configuration.GetValue<int>("OneTimePassword")).IsBetween(0, 999999);
}