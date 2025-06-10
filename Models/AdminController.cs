using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Wheels_in_Csharp.Models;

[Route("api/admin")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet("promote-to-admin")]
    public async Task<ActionResult<string>> PromoteToAdmin([FromQuery] string userEmail)
    {
        if (string.IsNullOrEmpty(userEmail))
        {
            return BadRequest("O e-mail do usuário é obrigatório.");
        }

        if (!await _roleManager.RoleExistsAsync("Admin"))
        {
            await _roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        var user = await _userManager.FindByEmailAsync(userEmail);

        if (user == null)
        {
            return NotFound($"Usuário com e-mail {userEmail} não encontrado.");
        }

        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            return Ok($"O usuário {userEmail} já é um administrador.");
        }

        await _userManager.AddToRoleAsync(user, "Admin");

        return Ok($"✅ Usuário {userEmail} promovido a ADMIN com sucesso!");
    }
}
