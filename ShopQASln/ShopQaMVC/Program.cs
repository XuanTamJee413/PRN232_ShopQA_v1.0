using ShopQaMVC.Services;

var builder = WebApplication.CreateBuilder(args);

// Thêm các dịch vụ cần thiết
builder.Services.AddControllersWithViews();  // Cho MVC
builder.Services.AddRazorPages();  // Kích hoạt Razor Pages

builder.Services.AddHttpClient("ShopApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7101/"); // đúng cổng Web API
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    return new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
    };
});


builder.Services.AddScoped<CategoryApiService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Áp dụng Razor Pages và MVC
app.MapControllerRoute(
    name: "default",

    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();  // Đảm bảo rằng Razor Pages được cấu hình để xử lý các yêu cầu


app.Run();
