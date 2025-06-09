using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Wheels_in_Csharp.Data;
using Wheels_in_Csharp.Models;
using Wheels_in_Csharp.Services;
using Wheels_in_Csharp.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Configuração da conexão
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Configuração do DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString,
    sqlOptions => sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

// Configuração do Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configuração de cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.LogoutPath = "/Account/Logout";
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
});

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

// Registro do EmailSender
builder.Services.AddTransient<IEmailSender, EmailSender>();

// Registro dos serviços
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IRentalService, RentalService>();
builder.Services.AddScoped<IUserService, UserService>();

// Configuração do MVC e API Controllers
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Admin");
    options.Conventions.AllowAnonymousToPage("/Account/Login");
    options.Conventions.AllowAnonymousToPage("/Account/Register");
});

// Configuração do CORS (se necessário para API)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Pipeline de requisições
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

// Configuração do banco de dados e roles
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Aplicar migrações
        await context.Database.MigrateAsync();

        // Criar roles padrão se não existirem
        string[] roles = { "Admin", "Customer" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Criar admin padrão se não existir
        var adminEmail = "admin@wheels.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Administrador",
                CPF = "12345678901",
                EmailConfirmed = true
            };

            string adminPassword = "Admin@123";
            await userManager.CreateAsync(adminUser, adminPassword);
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        // Promover usuário específico a admin (opcional)
        var specificUserEmail = "reluxaccs@gmail.com";
        var specificUser = await userManager.FindByEmailAsync(specificUserEmail);

        if (specificUser != null && !await userManager.IsInRoleAsync(specificUser, "Admin"))
        {
            await userManager.AddToRoleAsync(specificUser, "Admin");
            Console.WriteLine($"✅ Usuário {specificUserEmail} promovido a ADMINISTRADOR!");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erro na inicialização do banco de dados");
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();

// Habilitar CORS (antes do UseRouting)
app.UseCors("AllowAll");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Mapeamento dos endpoints
app.MapControllers();  // IMPORTANTE: Para habilitar os API Controllers
app.MapRazorPages();

// Endpoint para debug de rotas (opcional)
app.MapGet("/debug/routes", (IEnumerable<EndpointDataSource> endpointSources) =>
{
    var sb = new StringBuilder();
    foreach (var endpointSource in endpointSources)
    {
        foreach (var endpoint in endpointSource.Endpoints)
        {
            if (endpoint is RouteEndpoint routeEndpoint)
            {
                sb.AppendLine($"{routeEndpoint.DisplayName}");
                sb.AppendLine($"  Route: {routeEndpoint.RoutePattern.RawText}");
                sb.AppendLine($"  HTTP Methods: {string.Join(", ", routeEndpoint.Metadata.GetOrderedMetadata<IHttpMethodMetadata>().SelectMany(m => m.HttpMethods))}");
                sb.AppendLine();
            }
        }
    }
    return sb.ToString();
});

app.Run();