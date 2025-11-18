using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Data;
using ProyectoFinal.Security;
using ProyectoFinal.Services;

var builder = WebApplication.CreateBuilder(args);

// Cadena de conexión
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                      ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity con roles
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<IActivityLogService, ActivityLogService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Ejecutar el seeding de roles/usuario admin ANTES de configurar el pipeline final
await DbInitializer.InitializeAsync(app.Services);

// Pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
