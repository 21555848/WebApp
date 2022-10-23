using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Areas.Identity.Data;
using WebApp.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("WebAppContextConnection") ?? throw new InvalidOperationException("Connection string 'WebAppContextConnection' not found.");

builder.Services.AddDbContext<WebAppContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<WebAppUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<WebAppContext>();

builder.Services.AddWebOptimizer();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

//Seed deafult roles and users
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    try
    {
        var context = services.GetRequiredService<WebAppContext>();
        var userManager = services.GetRequiredService<UserManager<WebAppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await AppDbInitializer.SeedRolesAsync(userManager, roleManager);
        await AppDbInitializer.SeedDefaultUser(userManager, roleManager);
    }

    catch (Exception e)
    {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(e, "An error occurred while seeding the DB.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseWebOptimizer();

app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
