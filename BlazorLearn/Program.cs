using BlazorLearn.Components;
using BlazorLearn.Components.Account;                // برای IdentitySeed
using BlazorLearn.Services.Implementations;
// اگر کلاس IdentityNoOpEmailSender در namespace دیگری است، همان را نگه دارید:
using Identity; // (از اسکفولد شما)
using Microsoft.AspNetCore.Components.Authorization; // برای AuthenticationStateProvider
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);



var fa = CultureInfo.GetCultureInfo("fa-IR");
CultureInfo.DefaultThreadCurrentCulture = fa;
CultureInfo.DefaultThreadCurrentUICulture = fa;


// DbContext اصلی پروژه (همان که Identity هم روی آن است)
builder.Services.AddDbContext<BlazorLearnContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// سرویس‌های خودتان (Dapper)
builder.Services.AddScoped<FileStorageService>();
builder.Services.AddScoped<ProvinceService>();
builder.Services.AddScoped<CityService>();
builder.Services.AddScoped<PersonService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<UnitService>();


builder.Services.AddScoped<BlazorLearn.Services.Implementations.ProductImageService>();
builder.Services.AddScoped<BlazorLearn.Services.Implementations.ProductService>(); // 

// File storage for images
builder.Services.AddScoped<BlazorLearn.Services.Infra.IFileStorage, BlazorLearn.Services.Infra.FileStorage>();



// ثبت کنترلرها
builder.Services.AddControllers();

builder.Services.AddServerSideBlazor().AddCircuitOptions(o => o.DetailedErrors = true);


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
        options.SignIn.RequireConfirmedAccount = true;
        options.User.RequireUniqueEmail = true;
        // در صورت نیاز تنظیمات Password/Lockout و ... را اینجا اضافه کن
    })
    .AddRoles<IdentityRole>()                   // ← مهم: نقش‌ها
    .AddEntityFrameworkStores<BlazorLearnContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// ایمیل‌سندر اسکفولد (No-Op)
builder.Services.AddSingleton<IEmailSender<IdentityUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

// --- Seeding نقش‌ها و (اختیاری) افزودن کاربر به نقش ---
// اگر متدهای Seed شما خودش CreateScope می‌کند، همین کافی است
await IdentitySeed.EnsureRolesAsync(app.Services);
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

// Razor Pages برای UI اسکفولد Identity
app.MapRazorPages();

// مپ کردن کنترلرها
app.MapControllers();


// Blazor Server
app.UseAntiforgery();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Endpoints اضافی‌ای که اسکفولد ساخته (Minimal APIهای Identity)
app.MapAdditionalIdentityEndpoints();

app.Run();
