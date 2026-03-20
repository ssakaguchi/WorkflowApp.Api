using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WorkflowApp.Api.Infrastructure.Data;
using WorkflowApp.Api.Infrastructure.Security;
using WorkflowApp.Api.Services;
using WorkflowApp.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

// データベースコンテキストの登録
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();


// JWTの発行者、対象、シークレットキー、有効期限を設定
var issuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("Jwt:Issuerが設定されていません");

var audience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("Jwt:Audienceが設定されていません");

var secretKey = builder.Configuration["Jwt:SecretKey"]
    ?? throw new InvalidOperationException("Jwt:SecretKeyが設定されていません");

// JWT認証の設定
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // トークンの検証パラメータを設定
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,              // 発行者の検証を有効にする
            ValidIssuer = issuer,               // 有効な発行者を設定
            ValidateAudience = true,            // 対象の検証を有効にする
            ValidAudience = audience,           // 有効な対象を設定
            ValidateIssuerSigningKey = true,    // 署名キーの検証を有効にする
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),    // 署名キーを設定
            ValidateLifetime = true,            // トークンの有効期限の検証を有効にする
        };
    });

builder.Services.AddAuthentication();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
