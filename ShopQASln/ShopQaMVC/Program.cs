var builder = WebApplication.CreateBuilder(args);

// Thêm các dịch vụ cần thiết
builder.Services.AddControllersWithViews();  // Cho MVC
builder.Services.AddRazorPages();  // Kích hoạt Razor Pages

var app = builder.Build();

// Cấu hình pipeline HTTP
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
