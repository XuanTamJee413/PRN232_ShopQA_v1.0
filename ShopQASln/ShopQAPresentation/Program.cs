<<<<<<< HEAD
﻿using Business.Iservices;
using Business.Service;
using DataAccess.Context;
using DataAccess.IRepositories;
using DataAccess;
=======
﻿using DataAccess;
using DataAccess.Context;
using DataAccess.IRepositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
>>>>>>> 4012be3b525ad99380ec232ed5043aabc20d32f5
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("https://localhost:7035") // Cho phép yêu cầu từ domain này
               .AllowAnyMethod() // Cho phép bất kỳ phương thức HTTP nào
               .AllowAnyHeader(); // Cho phép bất kỳ header nào
    });
});

// Thêm DbContext và các dịch vụ khác
builder.Services.AddDbContext<ShopQADbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.MigrationsAssembly("DataAccess")
    ));

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();


builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();

<<<<<<< HEAD
=======
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "fallback_secret_key"))
        };
    });
>>>>>>> 4012be3b525ad99380ec232ed5043aabc20d32f5
var app = builder.Build();

// Sử dụng CORS
app.UseCors("AllowSpecificOrigin"); // Áp dụng chính sách CORS

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
<<<<<<< HEAD
=======
app.UseAuthentication();

>>>>>>> 4012be3b525ad99380ec232ed5043aabc20d32f5
app.UseAuthorization();
app.MapControllers();
app.Run();
