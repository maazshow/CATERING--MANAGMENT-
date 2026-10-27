using CateringMvc.Data;
using CateringMvc.Models;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Database aur tables pehli dafa khud ban jate hain, aur default prices daal di jati hain.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    // Purani database (pehle wale version ki) mein naye tables/columns khud add ho jate hain.
    // Purana data (orders aur prices) bilkul safe rehta hai.
    string[] upgrades =
    {
        "IF COL_LENGTH('PriceSettings','ZardaRicePerKg') IS NULL " +
        "ALTER TABLE PriceSettings ADD ZardaRicePerKg decimal(18,2) NOT NULL CONSTRAINT DF_PS_ZardaRice DEFAULT 400",

        "IF COL_LENGTH('PriceSettings','ZardaSweetsPerKg') IS NULL " +
        "ALTER TABLE PriceSettings ADD ZardaSweetsPerKg decimal(18,2) NOT NULL CONSTRAINT DF_PS_ZardaSweets DEFAULT 150",

        "IF COL_LENGTH('Orders','DeliveryDate') IS NULL " +
        "ALTER TABLE Orders ADD DeliveryDate datetime2 NULL",

        "UPDATE Orders SET DeliveryDate = CreatedAt WHERE DeliveryDate IS NULL",

        "IF COL_LENGTH('Orders','Status') IS NULL " +
        "ALTER TABLE Orders ADD Status nvarchar(20) NOT NULL CONSTRAINT DF_O_Status DEFAULT 'Completed'",

        // Order ke items (ek order mein kai items).
        "IF OBJECT_ID(N'dbo.OrderLines', N'U') IS NULL " +
        "CREATE TABLE dbo.OrderLines (" +
        "Id int IDENTITY(1,1) NOT NULL PRIMARY KEY, " +
        "OrderId int NOT NULL, " +
        "Description nvarchar(100) NOT NULL DEFAULT '', " +
        "Detail nvarchar(200) NOT NULL DEFAULT '', " +
        "Amount decimal(18,2) NOT NULL DEFAULT 0, " +
        "CONSTRAINT FK_OrderLines_Orders_OrderId FOREIGN KEY (OrderId) REFERENCES dbo.Orders(Id) ON DELETE CASCADE)",

        "IF COL_LENGTH('OrderLines','ItemType') IS NULL " +
        "ALTER TABLE OrderLines ADD ItemType nvarchar(20) NOT NULL CONSTRAINT DF_OL_ItemType DEFAULT 'Biryani'",

        "IF COL_LENGTH('OrderLines','RiceKg') IS NULL " +
        "ALTER TABLE OrderLines ADD RiceKg decimal(18,2) NOT NULL CONSTRAINT DF_OL_RiceKg DEFAULT 0",

        "IF COL_LENGTH('OrderLines','MeatKg') IS NULL " +
        "ALTER TABLE OrderLines ADD MeatKg decimal(18,2) NOT NULL CONSTRAINT DF_OL_MeatKg DEFAULT 0",

        "IF COL_LENGTH('OrderLines','CookingCharge') IS NULL " +
        "ALTER TABLE OrderLines ADD CookingCharge decimal(18,2) NOT NULL CONSTRAINT DF_OL_Cooking DEFAULT 0",

        "IF COL_LENGTH('PriceSettings','BiryaniCookingCharge') IS NULL " +
        "ALTER TABLE PriceSettings ADD BiryaniCookingCharge decimal(18,2) NOT NULL CONSTRAINT DF_PS_BiryaniCookCharge DEFAULT 2000",

        "IF COL_LENGTH('PriceSettings','ZardaCookingCharge') IS NULL " +
        "ALTER TABLE PriceSettings ADD ZardaCookingCharge decimal(18,2) NOT NULL CONSTRAINT DF_PS_ZardaCookCharge DEFAULT 1000",

        "IF COL_LENGTH('PriceSettings','KormaCookingCharge') IS NULL " +
        "ALTER TABLE PriceSettings ADD KormaCookingCharge decimal(18,2) NOT NULL CONSTRAINT DF_PS_KormaCookCharge DEFAULT 1000",

        // Haleem aur Chana Daig ke rates.
        "IF COL_LENGTH('PriceSettings','HaleemPerKg') IS NULL " +
        "ALTER TABLE PriceSettings ADD HaleemPerKg decimal(18,2) NOT NULL CONSTRAINT DF_PS_HaleemKg DEFAULT 1200",

        "IF COL_LENGTH('PriceSettings','HaleemCookingCharge') IS NULL " +
        "ALTER TABLE PriceSettings ADD HaleemCookingCharge decimal(18,2) NOT NULL CONSTRAINT DF_PS_HaleemCook DEFAULT 1000",

        "IF COL_LENGTH('PriceSettings','ChanaDaigPerKg') IS NULL " +
        "ALTER TABLE PriceSettings ADD ChanaDaigPerKg decimal(18,2) NOT NULL CONSTRAINT DF_PS_ChanaKg DEFAULT 600",

        "IF COL_LENGTH('PriceSettings','ChanaDaigCookingCharge') IS NULL " +
        "ALTER TABLE PriceSettings ADD ChanaDaigCookingCharge decimal(18,2) NOT NULL CONSTRAINT DF_PS_ChanaCook DEFAULT 1000",

        // Haleem / Chana ki quantity (kg) har item par.
        "IF COL_LENGTH('OrderLines','QuantityKg') IS NULL " +
        "ALTER TABLE OrderLines ADD QuantityKg decimal(18,2) NOT NULL CONSTRAINT DF_OL_QtyKg DEFAULT 0",

        // Advance / payment. Purane orders ko poori payment wala maan lete hain (taake sab red na ho jayen).
        "IF COL_LENGTH('Orders','AdvancePaid') IS NULL " +
        "ALTER TABLE Orders ADD AdvancePaid decimal(18,2) NULL",

        "UPDATE Orders SET AdvancePaid = TotalPrice WHERE AdvancePaid IS NULL",

        // Chana Daig ke masalay aur Beef Korma.
        "IF COL_LENGTH('PriceSettings','ChanaDaigSabzMasalaPerKg') IS NULL " +
        "ALTER TABLE PriceSettings ADD ChanaDaigSabzMasalaPerKg decimal(18,2) NOT NULL CONSTRAINT DF_PS_ChanaSabz DEFAULT 100",

        "IF COL_LENGTH('PriceSettings','ChanaDaigGaramMasalaPerKg') IS NULL " +
        "ALTER TABLE PriceSettings ADD ChanaDaigGaramMasalaPerKg decimal(18,2) NOT NULL CONSTRAINT DF_PS_ChanaGaram DEFAULT 80",

        "IF COL_LENGTH('PriceSettings','BeefKormaMeatPerKg') IS NULL " +
        "ALTER TABLE PriceSettings ADD BeefKormaMeatPerKg decimal(18,2) NOT NULL CONSTRAINT DF_PS_BeefMeat DEFAULT 2200",

        "IF COL_LENGTH('PriceSettings','BeefKormaCookingPerKg') IS NULL " +
        "ALTER TABLE PriceSettings ADD BeefKormaCookingPerKg decimal(18,2) NOT NULL CONSTRAINT DF_PS_BeefCook DEFAULT 250",

        // White Rice ke rates.
        "IF COL_LENGTH('PriceSettings','WhiteRicePerKg') IS NULL " +
        "ALTER TABLE PriceSettings ADD WhiteRicePerKg decimal(18,2) NOT NULL CONSTRAINT DF_PS_WhiteRice DEFAULT 350",

        "IF COL_LENGTH('PriceSettings','WhiteRiceCookingCharge') IS NULL " +
        "ALTER TABLE PriceSettings ADD WhiteRiceCookingCharge decimal(18,2) NOT NULL CONSTRAINT DF_PS_WhiteRiceCook DEFAULT 500",

        // Simple Rice aur Gor Wala Chawal ke rates.
        "IF COL_LENGTH('PriceSettings','SimpleRicePerKg') IS NULL " +
        "ALTER TABLE PriceSettings ADD SimpleRicePerKg decimal(18,2) NOT NULL CONSTRAINT DF_PS_SimpleRice DEFAULT 300",

        "IF COL_LENGTH('PriceSettings','SimpleRiceCookingCharge') IS NULL " +
        "ALTER TABLE PriceSettings ADD SimpleRiceCookingCharge decimal(18,2) NOT NULL CONSTRAINT DF_PS_SimpleRiceCook DEFAULT 500",

        "IF COL_LENGTH('PriceSettings','GurChawalRicePerKg') IS NULL " +
        "ALTER TABLE PriceSettings ADD GurChawalRicePerKg decimal(18,2) NOT NULL CONSTRAINT DF_PS_GurRice DEFAULT 400",

        "IF COL_LENGTH('PriceSettings','GurChawalGurPerKg') IS NULL " +
        "ALTER TABLE PriceSettings ADD GurChawalGurPerKg decimal(18,2) NOT NULL CONSTRAINT DF_PS_GurGur DEFAULT 150",

        "IF COL_LENGTH('PriceSettings','GurChawalCookingCharge') IS NULL " +
        "ALTER TABLE PriceSettings ADD GurChawalCookingCharge decimal(18,2) NOT NULL CONSTRAINT DF_PS_GurCook DEFAULT 1000",

        // Chana Daig ka naam ab Chana Wala Chawal.
        "UPDATE OrderLines SET Description = 'Chana Wala Chawal' WHERE Description = 'Chana Daig'",


        // Purane (ab istemal na hone wale) columns ko default de dein.
        "IF COL_LENGTH('PriceSettings','KormaCookingPerKg') IS NOT NULL " +
        "AND NOT EXISTS (SELECT 1 FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID('PriceSettings') " +
        "AND parent_column_id = COLUMNPROPERTY(OBJECT_ID('PriceSettings'),'KormaCookingPerKg','ColumnId')) " +
        "ALTER TABLE PriceSettings ADD CONSTRAINT DF_PS_KormaCookingPerKg DEFAULT 0 FOR KormaCookingPerKg",

        // Purane PriceSettings mein biryani cooking ka column ho to default de dein (ab istemal nahi hota).
        "IF COL_LENGTH('PriceSettings','BiryaniCookingPerKg') IS NOT NULL " +
        "AND NOT EXISTS (SELECT 1 FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID('PriceSettings') " +
        "AND parent_column_id = COLUMNPROPERTY(OBJECT_ID('PriceSettings'),'BiryaniCookingPerKg','ColumnId')) " +
        "ALTER TABLE PriceSettings ADD CONSTRAINT DF_PS_BiryaniCooking DEFAULT 0 FOR BiryaniCookingPerKg"
    };
    foreach (var sql in upgrades)
        db.Database.ExecuteSqlRaw(sql);

    if (!db.PriceSettings.Any())
    {
        db.PriceSettings.Add(new PriceSetting());
        db.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
