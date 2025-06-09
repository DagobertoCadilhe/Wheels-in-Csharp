using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Wheels_in_Csharp.Models;
using Wheels_in_Csharp.Services.Interfaces;

namespace Wheels_in_Csharp.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class UsersModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersModel> _logger;

        public UsersModel(IUserService userService, ILogger<UsersModel> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // Filtros
        [BindProperty(SupportsGet = true)]
        public string NameFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string EmailFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string RoleFilter { get; set; }

        // Paginação
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public const int PageSize = 10;

        // Lista de usuários para exibição
        public List<UserViewModel> Users { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                var usersQuery = _userService.GetAllUsersQueryable();

                // Aplicar filtros
                if (!string.IsNullOrEmpty(NameFilter))
                {
                    usersQuery = usersQuery.Where(u => u.FullName.Contains(NameFilter));
                }

                if (!string.IsNullOrEmpty(EmailFilter))
                {
                    usersQuery = usersQuery.Where(u => u.Email.Contains(EmailFilter));
                }

                if (!string.IsNullOrEmpty(RoleFilter))
                {
                    usersQuery = usersQuery.Where(u => u.Roles.Any(r => r == RoleFilter));
                }

                // Paginação
                var totalItems = await usersQuery.CountAsync();
                TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

                var users = await usersQuery
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToListAsync();

                // Mapear para ViewModel
                Users = users.Select(u => new UserViewModel
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    CPF = u.CPF,
                    RegistrationDate = u.RegistrationDate,
                    EmailConfirmed = u.EmailConfirmed,
                    IsAdmin = u.Roles.Contains("Admin")
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar usuários");
                TempData["ErrorMessage"] = "Erro ao carregar usuários. Por favor, tente novamente.";
            }
        }

        public async Task<IActionResult> OnPostToggleStatusAsync(string userId)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Usuário não encontrado.";
                    return RedirectToPage();
                }

                user.EmailConfirmed = !user.EmailConfirmed;
                var result = await _userService.UpdateUserAsync(user);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = $"Status do usuário {user.Email} atualizado com sucesso!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Erro ao atualizar status do usuário.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar status do usuário");
                TempData["ErrorMessage"] = "Erro ao alterar status do usuário. Por favor, tente novamente.";
            }

            return RedirectToPage(new { 
                currentPage = CurrentPage,
                nameFilter = NameFilter,
                emailFilter = EmailFilter,
                roleFilter = RoleFilter
            });
        }

        public async Task<IActionResult> OnPostDeleteAsync(string userId)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(userId);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Usuário excluído com sucesso!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Erro ao excluir usuário: " + string.Join(", ", result.Errors.Select(e => e.Description));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir usuário");
                TempData["ErrorMessage"] = "Erro ao excluir usuário. Por favor, tente novamente.";
            }

            return RedirectToPage(new { 
                currentPage = CurrentPage,
                nameFilter = NameFilter,
                emailFilter = EmailFilter,
                roleFilter = RoleFilter
            });
        }
    }

    public class UserViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string CPF { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool IsAdmin { get; set; }
    }
}