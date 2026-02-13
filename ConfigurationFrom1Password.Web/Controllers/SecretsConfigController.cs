using Microsoft.AspNetCore.Mvc;

namespace ConfigurationFrom1Password.Web.Controllers;

public record Config(string Password, string ConnectionString, string Url);

[ApiController]
[Route("[controller]")]
public class SecretsConfigController(IConfiguration config) : ControllerBase
{
    [HttpGet]
    public Config Get() =>
        new(config["SensitiveData"]!, config.GetConnectionString("DefaultConnection")!, config["ExternalService"]!);
}