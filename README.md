
---

# BlazorLearn

یک نمونه فروشگاه آنلاین با **Blazor Server + ASP.NET Core Identity** که شامل سه محیط اصلی است:

* **Admin Panel** (مدیریت دسته‌ها، واحدها، محصولات و…)
* **Storefront (ویترین فروشگاه)** با اسلایدر، نمایش دسته‌ها و محصولات پیشنهادی
* **Customer (اکانت کاربر)** با ورود به صورت **OTP (شماره موبایل)**

> این پروژه برای شروع سریع یک فروشگاه ماژولار طراحی شده و قابل توسعه به سرویس‌های واقعی (پرداخت، ارسال پیامک، جست‌وجوی پیشرفته، سبد خرید و…) است.

---

## فهرست

* [پیش‌نیازها](#پیشنیازها)
* [ساختار پوشه‌ها](#ساختار-پوشهها)
* [راه‌اندازی سریع](#راهاندازی-سریع)
* [پایگاه‌داده و Identity](#پایگاهداده-و-identity)
* [ورود با موبایل (OTP)](#ورود-با-موبایل-otp)
* [Storefront (ویترین)](#storefront-ویترین)
* [پنل مدیریت](#پنل-مدیریت)
* [استایل‌ها و RTL](#استایلها-و-rtl)
* [Run و Publish](#run-و-publish)
* [نقشهٔ راه](#نقشهٔ-راه)
* [عیب‌یابی رایج](#عیبیابی-رایج)
* [لایسنس](#لایسنس)

---

## پیش‌نیازها

* **.NET 8 SDK**
* **SQL Server** لوکال (یا هر Db دیگر که EF Core پشتیبانی کند)
* **Node/npm** (اختیاری؛ اگر نیاز به ابزارهای Front دارید)
* Visual Studio 2022 یا VS Code

---

## ساختار پوشه‌ها

```
BlazorLearn/
├─ Components/
│  ├─ Layout/
│  │  ├─ AdminLayout.razor
│  │  ├─ AdminNav.razor
│  │  ├─ MainLayout.razor
│  │  ├─ NavMenu.razor
│  │  └─ StorefrontLayout.razor (+ StorefrontLayout.razor.css)
│  ├─ Storefront/
│  │  ├─ Models/
│  │  │  ├─ SlideItem.cs
│  │  │  ├─ CategoryItem.cs
│  │  │  └─ ProductItem.cs
│  │  ├─ HeroSlider.razor
│  │  ├─ CategoryStrip.razor
│  │  ├─ ProductsGrid.razor
│  │  ├─ ProductCard.razor
│  │  └─ Index.razor            (Route: /store)
│  └─ Account/
│     └─ Pages/
│        ├─ LoginWithPhone.razor (Route: /phonelogin)
│        └─ Register.razor (بهبود UI)
├─ Controllers/ (در صورت نیاز)
├─ Endpoints/
│  └─ Auth/
│     └─ AuthOtpEndpoints.cs     (MapGroup: /api/auth/otp)
├─ Data/
│  ├─ BlazorLearnContext.cs
│  └─ (Migrations…)
├─ wwwroot/
│  ├─ css/
│  │  └─ storefront.css
│  ├─ uploads/
│  │  ├─ banners/hero1.jpg, hero2.jpg
│  │  ├─ cats/digital.png, fashion.png, home.png
│  │  └─ products/p1.jpg, p2.jpg, p3.jpg
│  └─ bootstrap, js, favicon.png …
├─ App.razor
├─ Program.cs
├─ appsettings.json
└─ README.md
```

---

## راه‌اندازی سریع

1. **کلون و ری‌استور**

```bash
git clone https://github.com/aliarjmandi/BlazorLearn.git
cd BlazorLearn
dotnet restore
```

2. **تنظیم اتصال دیتابیس** در `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=BlazorLearn;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "Logging": { "LogLevel": { "Default": "Information" } }
}
```

3. **مهاجرت و ساخت DB**
   اگر مایگریشن داری:

```bash
dotnet ef database update
```

اگر نداری:

```bash
dotnet ef migrations add Initial
dotnet ef database update
```

4. **اجرا**

```bash
dotnet watch
```

یا از داخل Visual Studio.

---

## پایگاه‌داده و Identity

در `Program.cs`:

```csharp
builder.Services.AddDbContext<BlazorLearnContext>(...);

builder.Services
    .AddIdentityCore<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<BlazorLearnContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();
```

> **نکته:** برای ساخت کاربر/نقش اولیه (Admin/Seller/Customer)، یک Seeder ساده بنویسید و در ابتدای برنامه یک‌بار اجرا کنید.

---

## ورود با موبایل (OTP)

اندپوینت‌ها در `AuthOtpEndpoints.cs` با مسیر پایه: `/api/auth/otp`:

* `POST /api/auth/otp/request`
  درخواست ارسال کد
* `POST /api/auth/otp/resend`
  ارسال مجدد کد
* `POST /api/auth/otp/verify`
  تایید کد → ایجاد/ورود کاربر

سرویس OTP فعلاً **In-Memory** است (برای تست). کد پیامک در **لاگ** چاپ می‌شود.
صفحه‌ی UI ورود: `/phonelogin`

> بعداً می‌تونید `IOtpService` را به سرویس واقعی SMS (Kavenegar, Ghasedak, …) وصل کنید.

### نمونه درخواست‌ها (curl)

```bash
curl -X POST http://localhost:7241/api/auth/otp/request -H "Content-Type: application/json" -d "{\"phone\":\"09120000000\"}"
curl -X POST http://localhost:7241/api/auth/otp/verify  -H "Content-Type: application/json" -d "{\"phone\":\"09120000000\",\"code\":\"12345\",\"rememberMe\":true}"
```

---

## Storefront (ویترین)

* Route اصلی: **`/store`**
* Layout: `Components/Layout/StorefrontLayout.razor`
* استایل اختصاصی: `wwwroot/css/storefront.css` (در `App.razor` لینک شده)

کامپوننت‌های اصلی:

* `HeroSlider.razor` – اسلایدر ساده
* `CategoryStrip.razor` – نمایش دسته‌ها
* `ProductsGrid.razor` + `ProductCard.razor` – نمایش محصولات

> فعلاً داده‌ها در `Components/Storefront/Index.razor` **Mock** هستند. در گام بعد سرویس و API واقعی به‌جای Mockها قرار می‌گیرد:
>
> * `GET /api/storefront/hero`
> * `GET /api/storefront/categories`
> * `GET /api/storefront/products/featured?take=12`

---

## پنل مدیریت

* Layout: `AdminLayout.razor` + `AdminNav.razor`
* صفحات نمونه: مدیریت **واحد کالا**، **دسته‌بندی‌ها**، **محصولات**
* ظاهر فرم‌ها با کارت‌های Bootstrap به‌روزرسانی شده است (گرید جمع‌وجور، فاصله‌ها و…).

---

## استایل‌ها و RTL

* فایل‌های عمومی: `wwwroot/app.css`
* استایل ویترین: `wwwroot/css/storefront.css`
* برای RTL از کلاس ساده‌ی `.rtl { direction: rtl; }` استفاده شده؛ می‌توانید روی `<body>` یا ریشه‌ی layout اعمال کنید.

> اگر از **Scoped CSS** در Razor Components استفاده می‌کنید، **هر کامپوننت فقط یک** فایل `.razor.css` داشته باشد (خطای `BLAZOR101` در صورت تکرار).

---

## Run و Publish

* **Run dev**

```bash
dotnet watch
```

* **Publish**

```bash
dotnet publish -c Release -o out
```

* اجرای سایت منتشرشده (IIS/Kestrel/Containers) مطابق محیط شما.

---

## نقشهٔ راه

* [x] ویترین ساده با اسلایدر/دسته/محصولات Mock
* [x] ورود OTP
* [ ] API واقعی ویترین (Hero/Categories/Featured Products)
* [ ] صفحه‌ی لیست دسته: `/category/{slug}`
* [ ] صفحه‌ی محصول: `/product/{slug}`
* [ ] سبد خرید (Draft) + MiniCart
* [ ] جست‌وجو و مرتب‌سازی
* [ ] اتصال OTP به سرویس SMS واقعی
* [ ] تست‌های یکپارچه و واحد

---

## عیب‌یابی رایج

**1) خطای `HttpClient` در صفحات Blazor**
ثبت `HttpClient` را فراموش نکنید:

```csharp
builder.Services.AddHttpClient();
```

**2) Scoped CSS خطای `BLAZOR101`**
برای هر `.razor` فقط **یک** `.razor.css` داشته باشید. فایل‌های تکراری را حذف/تغییرنام دهید.

**3) تصاویر نمایش داده نمی‌شوند**
مطمئن شوید مسیرها تحت `wwwroot/uploads/...` موجودند و لینک‌ها درست نوشته شده‌اند.

**4) راست‌به‌چپ اعمال نشده**
کلاس `.rtl` را روی بدنه یا layout بگذارید یا یک RTL کامل اضافه کنید.

**5) پس از Verify OTP لاگین انجام نمی‌شود**
لاگ را ببینید؛ اگر کاربر ساخته نمی‌شود، تنظیمات Identity/DbContext/مهاجرت‌ها را بررسی کنید.

---

## لایسنس

MIT (یا هر لایسنس دلخواه شما).
فایل `LICENSE` را در ریشه‌ی پروژه اضافه کنید.

---

### یادداشت پایانی

اگر این README را گذاشتی روی ریپو، کافی است در Pull Requestهای بعدی فقط بخش‌های «نقشهٔ راه» و «APIهای جدید» را آپدیت کنیم تا همکارها دقیق بدانند چه چیزی کجاست و چطور اجرا می‌شود.
