using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Wheels_in_Csharp.Models;

[Route("api/admin")]  // Rota base para APIs
[ApiController]       // Indica que é um controller de API
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

    [HttpGet("promote-to-admin")]  // Rota: GET /api/admin/promote-to-admin?userEmail=xxx
    public async Task<ActionResult<string>> PromoteToAdmin([FromQuery] string userEmail)
    {
        if (string.IsNullOrEmpty(userEmail))
        {
            return BadRequest("O e-mail do usuário é obrigatório.");
        }

        // 1. Verifica se a role "Admin" existe
        if (!await _roleManager.RoleExistsAsync("Admin"))
        {
            await _roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        // 2. Busca o usuário pelo e-mail
        var user = await _userManager.FindByEmailAsync(userEmail);

        if (user == null)
        {
            return NotFound($"Usuário com e-mail {userEmail} não encontrado.");
        }

        // 3. Verifica se o usuário já é admin
        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            return Ok($"O usuário {userEmail} já é um administrador.");
        }

        // 4. Adiciona a role "Admin" ao usuário
        await _userManager.AddToRoleAsync(user, "Admin");

        return Ok($"✅ Usuário {userEmail} promovido a ADMIN com sucesso!");
    }
}