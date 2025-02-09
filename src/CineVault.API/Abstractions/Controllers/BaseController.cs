using CineVault.API.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace CineVault.API.Abstractions.Controllers;

[ApiController]
[InheritedApiVersion(1)]
[InheritedApiVersion(2)]
[Route("api/v{v:apiVersion}/[controller]")]
public abstract class BaseController : ControllerBase
{
}