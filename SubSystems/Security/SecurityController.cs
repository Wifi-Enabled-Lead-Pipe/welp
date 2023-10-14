using Microsoft.AspNetCore.Mvc;

namespace Welp.Security;

[Route("security")]
public class SecurityController : Controller, ISecurityService
{
    private readonly ISecurityService securityService;

    public SecurityController(ISecurityService securityService)
    {
        this.securityService = securityService;
    }
}
