
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddHttpClient("IgnoreSSL")
    .ConfigurePrimaryHttpMessageHandler(() =>
        new HttpClientHandler
        {
            // Bỏ qua kiểm tra chứng chỉ SSL (chỉ dùng khi dev)
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        });



// Thêm các dịch vụ cần thiết
builder.Services.AddControllersWithViews();  // Cho MVC
builder.Services.AddRazorPages();  // Kích hoạt Razor Pages

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle("Google", options =>
{
    options.ClientId = "GOOGLE_CLIENT_ID";
    options.ClientSecret = "GOOGLE_CLIENT_SECRET";
    options.CallbackPath = "/signin-google";
});
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.AddHttpClient("IgnoreSSL")
    .ConfigurePrimaryHttpMessageHandler(() =>
        new HttpClientHandler
        {
            // Bỏ qua kiểm tra chứng chỉ SSL (chỉ dùng khi dev)
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        });



// Thêm các dịch vụ cần thiết
builder.Services.AddControllersWithViews();  // Cho MVC
builder.Services.AddRazorPages();  // Kích hoạt Razor Pages

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
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Áp dụng Razor Pages và MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();  // Đảm bảo rằng Razor Pages được cấu hình để xử lý các yêu cầu

app.Run();
