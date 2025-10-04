using BlazorLearn.Components;
using BlazorLearn.Components.Account;                // برای IdentitySeed
using BlazorLearn.Endpoints.Auth;
using BlazorLearn.Infrastructure;
using BlazorLearn.Services.Abstractions;
using BlazorLearn.Services.Implementations;
// اگر کلاس IdentityNoOpEmailSender در namespace دیگری است، همان را نگه دارید:
using Identity; // (از اسکفولد شما)
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization; // برای AuthenticationStateProvider
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

var fa = CultureInfo.GetCultureInfo("fa-IR");
CultureInfo.DefaultThreadCurrentCulture = fa;
CultureInfo.DefaultThreadCurrentUICulture = fa;


// DbContext اصلی پروژه (همان که Identity هم روی آن است)
builder.Services.AddDbContext<BlazorLearnContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// سرویس‌های خودتان (Dapper)
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ISessionIdProvider, SessionIdProvider>();
builder.Services.AddScoped<ICartService, CartService>();

builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddSingleton<FileStorageService>();
builder.Services.AddScoped<ProvinceService>();
builder.Services.AddScoped<CityService>();
builder.Services.AddScoped<PersonService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<UnitService>();
builder.Services.AddScoped<ICatalogReadService, CatalogReadService>();
builder.Services.AddScoped<ISlideReadService, SlideService>();
builder.Services.AddScoped<ISlideWriteService, SlideService>();
//builder.Services.AddScoped<ProductSeeder>();

builder.Services.AddScoped<IDbConnection>(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var cs = cfg.GetConnectionString("DefaultConnection");
    return new SqlConnection(cs);
});

builder.Services.AddHttpClient();
builder.Services.AddScoped<CategorySeeder>();

// برای کشِ In-Memory
builder.Services.AddMemoryCache();

// سرویس OTP (فعلاً In-Memory)
builder.Services.AddSingleton<IOtpServiceSmall, InMemoryOtpService>();


builder.Services.AddScoped<BlazorLearn.Services.Implementations.ProductImageService>();
builder.Services.AddScoped<BlazorLearn.Services.Implementations.ProductService>(); // 

// File storage for images
builder.Services.AddScoped<BlazorLearn.Services.Infra.IFileStorage, BlazorLearn.Services.Infra.FileStorage>();

// ثبت کنترلرها
builder.Services.AddControllers();
builder.Services.AddServerSideBlazor().AddCircuitOptions(o => o.DetailedErrors = true);

builder.Services.AddScoped(sp =>
{
    var nav = sp.GetRequiredService<NavigationManager>();
    return new HttpClient
    {
        BaseAddress = new Uri(nav.BaseUri) // مثلا https://localhost:7241/
    };
});


// Blazor Server
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Razor Pages برای UI پیش‌فرض Identity (اسکفولد شده)
builder.Services.AddRazorPages();

// State احراز هویت در درخت کامپوننت‌ها
builder.Services.AddCascadingAuthenticationState();

// سرویس‌های اسکفولد (Accessors/Redirect/Validation)
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

// احراز هویت/مجوز
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddIdentityCookies();

builder.Services.AddAuthorization();

// ✅ Identity به‌همراه Roles (string-based)
builder.Services
    .AddIdentityCore<IdentityUser>(options =>
    {
        //ورود با کمک پیامک خواهد بود
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = false;
        // در صورت نیاز تنظیمات Password/Lockout و ... را اینجا اضافه کن
    })
    .AddRoles<IdentityRole>()                   // ← مهم: نقش‌ها
    .AddEntityFrameworkStores<BlazorLearnContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// ایمیل‌سندر اسکفولد (No-Op)
builder.Services.AddSingleton<IEmailSender<IdentityUser>, IdentityNoOpEmailSender>();

var app = builder.Build();



// --- Seed Roles & Default Admin ---
/*
using (var scope = app.Services.CreateScope())
{
    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    string[] roles = { "Admin", "Seller" };
    foreach (var r in roles)
        if (!await roleMgr.RoleExistsAsync(r))
            await roleMgr.CreateAsync(new IdentityRole(r));

    // ادمین پیش‌فرض (اختیاری)
    var adminEmail = "admin@site.local";
    var admin = await userMgr.FindByEmailAsync(adminEmail);
    if (admin is null)
    {
        admin = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
        await userMgr.CreateAsync(admin, "Admin#12345"); // پسورد را بعداً تغییر بده
        await userMgr.AddToRoleAsync(admin, "Admin");
    }
}
*/


// مپ اندپوینت‌ها
app.MapAuthOtpEndpoints(); 

// --- Seeding نقش‌ها و (اختیاری) افزودن کاربر به نقش ---
// اگر متدهای Seed شما خودش CreateScope می‌کند، همین کافی است

//await IdentitySeed.EnsureRolesAsync(app.Services);
await IdentitySeed.EnsureUserInRoleAsync(app.Services, "aliarjmandi@yahoo.com", "Admin");

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<BlazorLearn.Infrastructure.EnsureSessionIdMiddleware>();

// Razor Pages برای UI اسکفولد Identity
app.MapRazorPages();

// مپ کردن کنترلرها
app.MapControllers();


// Blazor Server
app.UseAntiforgery();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

/*

app.MapPost("/Seeder/Basalam/seed-categories", async (CategorySeeder seeder, IWebHostEnvironment env) =>
{
    if (!env.IsDevelopment()) return Results.Forbid();
    try
    {
        await seeder.SeedFromHtmlAsync("wwwroot/menu_final.html", true);
        return Results.Ok(new { ok = true });
    }
    catch (FileNotFoundException ex)
    {
        return Results.Problem(
            title: "menu_final.html not found",
            detail: ex.FileName,
            statusCode: StatusCodes.Status404NotFound);
    }
    catch (Exception ex)
    {
        return Results.Problem(
            title: "Seeder failed",
            detail: ex.ToString(),
            statusCode: StatusCodes.Status500InternalServerError);
    }
})
.DisableAntiforgery();
*/


// Program.cs — Endpoint
/*
app.MapPost("/Seeder/Products/seed", async (ProductSeeder seeder, int perLeaf = 12, bool drop = true) =>
{
    await seeder.SeedAsync(perLeaf, drop);
    return Results.Ok(new { ok = true, perLeaf, dropped = drop });
})
// .RequireAuthorization("Admin")     // اگر خواستی محدودش کن
.WithName("SeedProducts");
*/

// Endpoints اضافی‌ای که اسکفولد ساخته (Minimal APIهای Identity)

app.MapAdditionalIdentityEndpoints();
app.Run();
