SET XACT_ABORT ON;
BEGIN TRAN;

-- 3-level seed for dbo.Categories
-- Columns: Id (uniqueidentifier), ParentId (uniqueidentifier, null), Name (nvarchar), Slug (varchar), SortOrder (int), IsActive (bit), CreatedAt (datetime2)

-- Root category IDs
DECLARE @Mobile UNIQUEIDENTIFIER         = NEWID();
DECLARE @Computer UNIQUEIDENTIFIER       = NEWID();
DECLARE @HomeAppliances UNIQUEIDENTIFIER = NEWID();
DECLARE @Supermarket UNIQUEIDENTIFIER    = NEWID();
DECLARE @Fashion UNIQUEIDENTIFIER        = NEWID();
DECLARE @Beauty UNIQUEIDENTIFIER         = NEWID();
DECLARE @Books UNIQUEIDENTIFIER          = NEWID();
DECLARE @Sports UNIQUEIDENTIFIER         = NEWID();

-- Insert Roots
INSERT INTO dbo.Categories (Id, ParentId, Name, Slug, SortOrder, IsActive, CreatedAt) VALUES
(@Mobile,         NULL, N'موبایل',                 'mobile',                 0, 1, GETUTCDATE()),
(@Computer,       NULL, N'کالای دیجیتال',          'digital-goods',          1, 1, GETUTCDATE()),
(@HomeAppliances, NULL, N'خانه و آشپزخانه',        'home-kitchen',           2, 1, GETUTCDATE()),
(@Supermarket,    NULL, N'سوپرمارکت',              'supermarket',            3, 1, GETUTCDATE()),
(@Fashion,        NULL, N'مد و پوشاک',             'fashion',                4, 1, GETUTCDATE()),
(@Beauty,         NULL, N'زیبایی و سلامت',         'beauty-health',          5, 1, GETUTCDATE()),
(@Books,          NULL, N'کتاب و لوازم‌تحریر',     'books-stationery',       6, 1, GETUTCDATE()),
(@Sports,         NULL, N'ورزش و سفر',             'sports-travel',          7, 1, GETUTCDATE());

/* =======================
   Level 2 under Mobile
   ======================= */
DECLARE @Mobile_Phones UNIQUEIDENTIFIER = NEWID();
DECLARE @Mobile_Tablets UNIQUEIDENTIFIER = NEWID();
DECLARE @Mobile_Accessories UNIQUEIDENTIFIER = NEWID();
DECLARE @Mobile_Wearables UNIQUEIDENTIFIER = NEWID();

INSERT INTO dbo.Categories (Id, ParentId, Name, Slug, SortOrder, IsActive, CreatedAt) VALUES
(@Mobile_Phones,      @Mobile, N'گوشی موبایل',           'mobile-phones',        0, 1, GETUTCDATE()),
(@Mobile_Tablets,     @Mobile, N'تبلت',                  'tablets',              1, 1, GETUTCDATE()),
(@Mobile_Accessories, @Mobile, N'لوازم جانبی موبایل',    'mobile-accessories',   2, 1, GETUTCDATE()),
(@Mobile_Wearables,   @Mobile, N'ساعت و بند هوشمند',     'smart-wearables',      3, 1, GETUTCDATE());

-- Level 3 under Mobile -> Phones
INSERT INTO dbo.Categories (Id, ParentId, Name, Slug, SortOrder, IsActive, CreatedAt) VALUES
(NEWID(), @Mobile_Phones, N'گوشی هوشمند اندروید', 'android-phones',  0, 1, GETUTCDATE()),
(NEWID(), @Mobile_Phones, N'آیفون',               'apple-iphone',    1, 1, GETUTCDATE()),
(NEWID(), @Mobile_Phones, N'گوشی ساده',           'feature-phones',  2, 1, GETUTCDATE());

-- Level 3 under Mobile -> Accessories
INSERT INTO dbo.Categories (Id, ParentId, Name, Slug, SortOrder, IsActive, CreatedAt) VALUES
(NEWID(), @Mobile_Accessories, N'شارژر و کابل',       'chargers-cables',     0, 1, GETUTCDATE()),
(NEWID(), @Mobile_Accessories, N'قاب و گلس',          'cases-protectors',    1, 1, GETUTCDATE()),
(NEWID(), @Mobile_Accessories, N'پاوربانک',           'power-banks',         2, 1, GETUTCDATE());

-- Level 3 under Mobile -> Wearables
INSERT INTO dbo.Categories (Id, ParentId, Name, Slug, SortOrder, IsActive, CreatedAt) VALUES
(NEWID(), @Mobile_Wearables, N'ساعت هوشمند',        'smart-watches',        0, 1, GETUTCDATE()),
(NEWID(), @Mobile_Wearables, N'مچ‌بند سلامت',       'fitness-bands',        1, 1, GETUTCDATE());

/* =======================
   Level 2 under Digital Goods
   ======================= */
DECLARE @Comp_Laptops UNIQUEIDENTIFIER = NEWID();
DECLARE @Comp_Desktops UNIQUEIDENTIFIER = NEWID();
DECLARE @Comp_Components UNIQUEIDENTIFIER = NEWID();
DECLARE @Comp_Accessories UNIQUEIDENTIFIER = NEWID();

INSERT INTO dbo.Categories (Id, ParentId, Name, Slug, SortOrder, IsActive, CreatedAt) VALUES
(@Comp_Laptops,    @Computer, N'لپ‌تاپ',                'laptops',               0, 1, GETUTCDATE()),
(@Comp_Desktops,   @Computer, N'رایانه رومیزی',         'desktop-pcs',           1, 1, GETUTCDATE()),
(@Comp_Components, @Computer, N'قطعات کامپیوتر',        'pc-components',         2, 1, GETUTCDATE()),
(@Comp_Accessories,@Computer, N'لوازم جانبی کامپیوتر',   'computer-accessories',  3, 1, GETUTCDATE());

-- Level 3 under Digital Goods -> Components
INSERT INTO dbo.Categories (Id, ParentId, Name, Slug, SortOrder, IsActive, CreatedAt) VALUES
(NEWID(), @Comp_Components, N'کارت گرافیک',  'graphic-cards',   0, 1, GETUTCDATE()),
(NEWID(), @Comp_Components, N'پردازنده',     'cpus',            1, 1, GETUTCDATE()),
(NEWID(), @Comp_Components, N'حافظه (RAM)',  'memory-ram',      2, 1, GETUTCDATE()),
(NEWID(), @Comp_Components, N'ذخیره‌سازی (SSD/HDD)', 'storage',  3, 1, GETUTCDATE());

-- Level 3 under Digital Goods -> Accessories
INSERT INTO dbo.Categories (Id, ParentId, Name, Slug, SortOrder, IsActive, CreatedAt) VALUES
(NEWID(), @Comp_Accessories, N'ماوس',          'mice',             0, 1, GETUTCDATE()),
(NEWID(), @Comp_Accessories, N'کیبورد',        'keyboards',        1, 1, GETUTCDATE()),
(NEWID(), @Comp_Accessories, N'هدست و اسپیکر', 'headsets-speakers',2, 1, GETUTCDATE());

/* =======================
   Level 2 under Home & Kitchen
   ======================= */
DECLARE @Home_KitchenLarge UNIQUEIDENTIFIER = NEWID();
DECLARE @Home_Cleaning UNIQUEIDENTIFIER = NEWID();
DECLARE @Home_Cookware UNIQUEIDENTIFIER = NEWID();

INSERT INTO dbo.Categories (Id, ParentId, Name, Slug, SortOrder, IsActive, CreatedAt) VALUES
(@Home_KitchenLarge, @HomeAppliances, N'لوازم خانگی بزرگ', 'large-appliances',   0, 1, GETUTCDATE()),
(@Home_Cleaning,     @HomeAppliances, N'نظافت خانه',       'home-cleaning',      1, 1, GETUTCDATE()),
(@Home_Cookware,     @HomeAppliances, N'آشپزی و سرو',       'cookware-serveware', 2, 1, GETUTCDATE());

-- Level 3 under Home & Kitchen -> Large Appliances
INSERT INTO dbo.Categories (Id, ParentId, Name, Slug, SortOrder, IsActive, CreatedAt) VALUES
(NEWID(), @Home_KitchenLarge, N'یخچال و فریزر', 'refrigerators',     0, 1, GETUTCDATE()),
(NEWID(), @Home_KitchenLarge, N'ماشین لباسشویی', 'washing-machines', 1, 1, GETUTCDATE()),
(NEWID(), @Home_KitchenLarge, N'اجاق گاز و فر',  'ovens-stoves',     2, 1, GETUTCDATE());

-- Level 3 under Home & Kitchen -> Cleaning
INSERT INTO dbo.Categories (Id, ParentId, Name, Slug, SortOrder, IsActive, CreatedAt) VALUES
(NEWID(), @Home_Cleaning, N'جاروبرقی',         'vacuum-cleaners', 0, 1, GETUTCDATE()),
(NEWID(), @Home_Cleaning, N'بخارشوی',          'steam-cleaners',  1, 1, GETUTCDATE());

-- Level 3 under Home & Kitchen -> Cookware
INSERT INTO dbo.Categories (Id, ParentId, Name, Slug, SortOrder, IsActive, CreatedAt) VALUES
(NEWID(), @Home_Cookware, N'قابلمه و تابه',    'pots-pans',       0, 1, GETUTCDATE()),
(NEWID(), @Home_Cookware, N'ظروف سرو',         'serveware',       1, 1, GETUTCDATE());

/* =======================
   Level 2 under Supermarket
   ======================= */
DECLARE @Market_Beverages UNIQUEIDENTIFIER = NEWID();
DECLARE @Market_Snacks UNIQUEIDENTIFIER = NEWID();
DECLARE @Market_Staples UNIQUEIDENTIFIER = NEWID();

INSERT INTO dbo.Categories (Id, ParentId, Name, Slug, SortOrder, IsActive, CreatedAt) VALUES
(@Market_Beverages, @Supermarket, N'نوشیدنی',   'beverages', 0, 1, GETUTCDATE()),
(@Market_Snacks,    @Supermarket, N'تنقلات',    'snacks',    1, 1, GETUTCDATE()),
(@Market_Staples,   @Supermarket, N'کالاهای اساسی', 'staples',2, 1, GETUTCDATE());

-- Level 3 under Supermarket
INSERT INTO dbo.Categories (Id, ParentId, Name, Slug, SortOrder, IsActive, CreatedAt) VALUES
(NEWID(), @Market_Beverages, N'نوشابه و آبمیوه', 'softdrinks-juices', 0, 1, GETUTCDATE()),
(NEWID(), @Market_Beverages, N'چای و قهوه',      'tea-coffee',        1, 1, GETUTCDATE()),
(NEWID(), @Market_Snacks,    N'چیپس و پفک',      'chips',             0, 1, GETUTCDATE()),
(NEWID(), @Market_Snacks,    N'شکلات و بیسکویت', 'chocolate-biscuit', 1, 1, GETUTCDATE()),
(NEWID(), @Market_Staples,   N'برنج و حبوبات',   'rice-legumes',      0, 1, GETUTCDATE()),
(NEWID(), @Market_Staples,   N'روغن و کنسرو',    'oil-canned',        1, 1, GETUTCDATE());

/* =======================
   Level 2 under Fashion
   ======================= */
DECLARE @Fashion_Women UNIQUEIDENTIFIER = NEWID();
DECLARE @Fashion_Men UNIQUEIDENTIFIER = NEWID();
DECLARE @Fashion_Kids UNIQUEIDENTIFIER = NEWID();

INSERT INTO dbo.Categories (Id, ParentId, Name, Slug, SortOrder, IsActive, CreatedAt) VALUES
(@Fashion_Women, @Fashion, N'زنانه',  'women-fashion', 0, 1, GETUTCDATE()),
(@Fashion_Men,   @Fashion, N'مردانه', 'men-fashion',   1, 1, GETUTCDATE()),
(@Fashion_Kids,  @Fashion, N'بچگانه', 'kids-fashion',  2, 1, GETUTCDATE());

-- Level 3 under Fashion
INSERT INTO dbo.Categories (Id, ParentId, Name, Slug, SortOrder, IsActive, CreatedAt) VALUES
(NEWID(), @Fashion_Women, N'پوشاک',      'women-clothing', 0, 1, GETUTCDATE()),
(NEWID(), @Fashion_Women, N'کیف و کفش',  'women-bags-shoes', 1, 1, GETUTCDATE()),
(NEWID(), @Fashion_Men,   N'پوشاک',      'men-clothing',   0, 1, GETUTCDATE()),
(NEWID(), @Fashion_Men,   N'کفش مردانه', 'men-shoes',      1, 1, GETUTCDATE()),
(NEWID(), @Fashion_Kids,  N'لباس کودک',  'kids-clothing',  0, 1, GETUTCDATE());

/* =======================
   Level 2 under Beauty & Health
   ======================= */
DECLARE @Beauty_Makeup UNIQUEIDENTIFIER = NEWID();
DECLARE @Beauty_Skincare UNIQUEIDENTIFIER = NEWID();
DECLARE @Beauty_Haircare UNIQUEIDENTIFIER = NEWID();

INSERT INTO dbo.Categories (Id, ParentId, Name, Slug, SortOrder, IsActive, CreatedAt) VALUES
(@Beauty_Makeup,   @Beauty, N'آرایشی',       'makeup',      0, 1, GETUTCDATE()),
(@Beauty_Skincare, @Beauty, N'مراقبت پوست',  'skin-care',   1, 1, GETUTCDATE()),
(@Beauty_Haircare, @Beauty, N'مراقبت مو',    'hair-care',   2, 1, GETUTCDATE());

-- Level 3 under Beauty
INSERT INTO dbo.Categories (Id, ParentId, Name, Slug, SortOrder, IsActive, CreatedAt) VALUES
(NEWID(), @Beauty_Makeup,   N'رژلب و لاک',     'lip-nail',      0, 1, GETUTCDATE()),
(NEWID(), @Beauty_Makeup,   N'کرم پودر و پنکک', 'face-makeup',   1, 1, GETUTCDATE()),
(NEWID(), @Beauty_Skincare, N'کرم و مرطوب‌کننده', 'creams',     0, 1, GETUTCDATE()),
(NEWID(), @Beauty_Skincare, N'ضدآفتاب',        'sunscreens',    1, 1, GETUTCDATE()),
(NEWID(), @Beauty_Haircare, N'شامپو و ماسک مو', 'shampoo-mask', 0, 1, GETUTCDATE());

/* =======================
   Level 2 under Books & Stationery
   ======================= */
DECLARE @Books_Printed UNIQUEIDENTIFIER = NEWID();
DECLARE @Books_Stationery UNIQUEIDENTIFIER = NEWID();

INSERT INTO dbo.Categories (Id, ParentId, Name, Slug, SortOrder, IsActive, CreatedAt) VALUES
(@Books_Printed,   @Books, N'کتاب چاپی',     'printed-books', 0, 1, GETUTCDATE()),
(@Books_Stationery,@Books, N'لوازم‌تحریر',   'stationery',    1, 1, GETUTCDATE());

-- Level 3 under Books
INSERT INTO dbo.Categories (Id, ParentId, Name, Slug, SortOrder, IsActive, CreatedAt) VALUES
(NEWID(), @Books_Printed,    N'رمان و ادبیات',     'novels-literature', 0, 1, GETUTCDATE()),
(NEWID(), @Books_Printed,    N'آموزشی و دانشگاهی', 'education-academic',1, 1, GETUTCDATE()),
(NEWID(), @Books_Stationery, N'دفتر و کاغذ',       'notebooks-paper',   0, 1, GETUTCDATE()),
(NEWID(), @Books_Stationery, N'خودکار و لوازم نوشتن', 'pens-writing',  1, 1, GETUTCDATE());

/* =======================
   Level 2 under Sports & Travel
   ======================= */
DECLARE @Sports_Wear UNIQUEIDENTIFIER = NEWID();
DECLARE @Sports_Gear UNIQUEIDENTIFIER = NEWID();
DECLARE @Sports_Travel UNIQUEIDENTIFIER = NEWID();

INSERT INTO dbo.Categories (Id, ParentId, Name, Slug, SortOrder, IsActive, CreatedAt) VALUES
(@Sports_Wear,  @Sports, N'پوشاک ورزشی',  'sports-wear',   0, 1, GETUTCDATE()),
(@Sports_Gear,  @Sports, N'تجهیزات ورزشی', 'sports-gear',   1, 1, GETUTCDATE()),
(@Sports_Travel,@Sports, N'کمپینگ و سفر',  'camping-travel',2, 1, GETUTCDATE());

-- Level 3 under Sports
INSERT INTO dbo.Categories (Id, ParentId, Name, Slug, SortOrder, IsActive, CreatedAt) VALUES
(NEWID(), @Sports_Wear,   N'کفش ورزشی',       'sport-shoes',  0, 1, GETUTCDATE()),
(NEWID(), @Sports_Wear,   N'لباس ورزشی',      'sport-clothes',1, 1, GETUTCDATE()),
(NEWID(), @Sports_Gear,   N'دمبل و وزنه',     'dumbbells',    0, 1, GETUTCDATE()),
(NEWID(), @Sports_Gear,   N'توپ و راکت',      'balls-rackets',1, 1, GETUTCDATE()),
(NEWID(), @Sports_Travel, N'چادر و کیسه خواب','tents-sleeping',0, 1, GETUTCDATE()),
(NEWID(), @Sports_Travel, N'کوله‌پشتی سفر',   'travel-backpacks',1,1, GETUTCDATE());

COMMIT TRAN;
