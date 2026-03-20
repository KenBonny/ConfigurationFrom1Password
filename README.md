# ConfigurationFrom1Password

A .NET configuration extension that securely loads secrets from 1Password into your application configuration without hardcoding credentials in your code. This is intended for local development and testing scenarios, not production use. Please use a secure secrets management solution for production applications.

## Motivation

I didn't like putting account credentials, passwords, and secret keys directly into code or configuration files. This implementation uses 1Password's CLI to read secrets during application startup, keeping your credentials safe and secure.

Instead of hardcoding sensitive values or storing them in plain text configuration files, this library allows you to reference 1Password secret references (e.g., `op://Private/MyVault/password`) in your configuration, and they are automatically resolved at runtime.

## Features

- **Secure credential management**: No credentials stored in code or configuration files
- **1Password integration**: Leverages the 1Password CLI (`op.exe`) to fetch secrets
- **Seamless configuration replacement**: Works with existing .NET configuration system
- **Multiple configuration sources**: Supports JSON files, in-memory collections, and more
- **Complex object support**: Can resolve secrets in nested configuration objects
- **Connection strings**: Works with connection strings and any configuration value

## Prerequisites

- Active 1Password account with stored secrets
- [1Password CLI](https://developer.1password.com/docs/cli) installed and configured
- .NET 8.0 or later

## Installation

Add the `ConfigurationFrom1Password` project to your solution and reference it in your application.

## Usage

### Basic Setup

Add the `.Replace1PasswordSecrets()` extension method to your configuration builder **after** adding your configuration sources. This will scan for any 1Password secret references and replace them with the actual secret values at runtime.
Replace sensitive configuration values with 1Password secret references in your `appsettings.json`:

```json
{
  "NonSensitiveData": "regular-value",
  "SensitiveData": "op://Private/1Password Config Extension Test/password",
  "ConnectionStrings": {
    "DefaultConnection": "op://Private/1Password Config Extension Test/connectionstring"
  }
}
```

> **Be Aware**: I have not gotten it to work with a `ServiceCollection` configuration provider, you must use a generic `Host` or `WebApplication` builder to use the extension method. If you have a solution for this, please submit a PR.

### Console Application

```csharp
using ConfigurationFrom1Password;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

if (builder.Environment.IsDevelopment())
    builder.Configuration.Replace1PasswordSecrets();

// Access your secrets securely
var sensitiveData = builder.Configuration["SensitiveData"];
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
```

### ASP.NET Core Web Application

```csharp
using ConfigurationFrom1Password;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json");

if (builder.Environment.IsDevelopment())
    builder.Configuration.Replace1PasswordSecrets();

var app = builder.Build();
app.Run();
```

### In-Memory Configuration with Secrets

```csharp
builder.Configuration
    .AddInMemoryCollection([
        new("SensitiveData", "op://Private/MyVault/password")
    ])
    .Replace1PasswordSecrets();
```

## How It Works

1. The extension scans all configuration values for 1Password secret references (values starting with `op://`)
2. For each unique secret reference, it executes the 1Password CLI to retrieve the actual value
3. The configuration values are replaced with the resolved secrets
4. Your application accesses the configuration normally, with all secrets securely loaded

The `OnePasswordConfigurationProvider` uses the `op.exe read` command to fetch secrets, ensuring that credentials are only retrieved at runtime and never stored in your codebase.

## 1Password Secret Reference Format

1Password secret references follow this format:

```
op://<vault>/<item>/<field>
```

Examples:
- `op://Private/DatabaseCredentials/password`
- `op://Private/APIKeys/token`
- `op://Private/MyApp/connectionstring`

## Security Benefits

- **No hardcoded secrets**: Credentials never appear in source code
- **Version control safe**: Configuration files can be committed without exposing secrets
- **Centralized secret management**: All secrets managed through 1Password
- **Audit trail**: 1Password provides access logs for secret retrieval
- **Team collaboration**: Share secrets securely through 1Password vaults

## Testing

The project includes comprehensive tests demonstrating various usage scenarios. Note that tests requiring 1Password access are marked as `[Explicit]` to prevent accidental execution.
