using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Wheels_in_Csharp.Models;

namespace Wheels_in_Csharp.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "O email é obrigatório")]
            [EmailAddress(ErrorMessage = "Email inválido")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "O nome completo é obrigatório")]
            [Display(Name = "Nome Completo")]
            [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
            public string FullName { get; set; }

            [Required(ErrorMessage = "O CPF é obrigatório")]
            [StringLength(11, MinimumLength = 11, ErrorMessage = "CPF deve ter exatamente 11 dígitos")]
            [Display(Name = "CPF")]
            [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve conter apenas números (11 dígitos)")]
            public string CPF { get; set; }

            [Required(ErrorMessage = "O endereço é obrigatório")]
            [Display(Name = "Endereço Completo")]
            [StringLength(200, ErrorMessage = "O endereço deve ter no máximo 200 caracteres")]
            public string Address { get; set; }

            [Required(ErrorMessage = "A senha é obrigatória")]
            [StringLength(100, ErrorMessage = "A senha deve ter pelo menos {2} e no máximo {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Senha")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar Senha")]
            [Compare("Password", ErrorMessage = "A senha e a confirmação não coincidem.")]
            public string ConfirmPassword { get; set; }

            [Display(Name = "Registrar como Administrador")]
            public bool IsAdmin { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var existingUserByCpf = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.CPF == Input.CPF);
                if (existingUserByCpf != null)
                {
                    ModelState.AddModelError(string.Empty, "Este CPF já está cadastrado.");
                    return Page();
                }

                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                user.FullName = Input.FullName;
                user.CPF = Input.CPF;
                user.Address = Input.Address;
                user.RegistrationDate = DateTime.UtcNow;

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Usuário criou uma nova conta com senha.");

                    // Verificar e criar roles se não existirem
                    if (!await _roleManager.RoleExistsAsync("Admin"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    }
                    if (!await _roleManager.RoleExistsAsync("Customer"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Customer"));
                    }

                    // Atribuir role baseado na seleção
                    if (Input.IsAdmin)
                    {
                        await _userManager.AddToRoleAsync(user, "Admin");
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, "Customer");
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}