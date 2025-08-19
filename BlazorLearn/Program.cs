using BlazorLearn.Components;
using BlazorLearn.Services.Implementations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DbContext برای Identity
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// سرویس‌های خودت (Dapper)
builder.Services.AddScoped<FileStorageService>();
builder.Services.AddScoped<ProvinceService>();
builder.Services.AddScoped<CityService>();
builder.Services.AddScoped<PersonService>();

// Razor Components (Blazor Server)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Razor Pages برای UI پیش‌فرض Identity
builder.Services.AddRazorPages();

// ✅ هویت با UI پیش‌فرض
//builder.Services
//    .AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>(options =>
//    {
//        options.User.RequireUniqueEmail = true;
//        // سایر تنظیمات اختیاری password/lockout و ...
//    })
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddDefaultTokenProviders()
//    .AddDefaultUI(); // مهم: UI آماده‌ی Identity

// احراز هویت/مجوز
builder.Services.AddAuthentication().AddIdentityCookies();
builder.Services.AddAuthorization();

var app = builder.Build();

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

// مسیرهای Razor Pages (برای Identity UI)
app.MapRazorPages();

// Blazor Server
app.UseAntiforgery();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
