using System.Diagnostics;

namespace ConfigurationFrom1Password.Tests;

public class BeforeAllTests
{
    [Before(Assembly)]
    public static void LogInTo1PasswordSoAllTestsCanAccessSecrets()
    {
        var logIn1PasswordInfo = new ProcessStartInfo("op.exe", ["signin"])
        {
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };
        var logIn1PasswordProcess = Process.Start(logIn1PasswordInfo);
        
        if (logIn1PasswordProcess is null)
            throw new InvalidOperationException("Failed to start op.exe");
        
        logIn1PasswordProcess.WaitForExit();
    }
}