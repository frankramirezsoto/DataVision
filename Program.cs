using DataVision.Data;
using DataVision.Filters;
using DataVision.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<IRecopeService, RecopeService>();

// CORS
var MyCors = "_mycors";
builder.Services.AddCors(o =>
{
    o.AddPolicy(MyCors, p => p
        .WithOrigins("http://localhost:8080") // FRONT
        .AllowAnyHeader()
        .AllowAnyMethod()
    );
});

// JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});
builder.Services.AddAuthorization();

builder.Services.AddOpenApi();

// DB
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<LogUserActionFilter>();
builder.Services.AddScoped<LogService>();
builder.Services.AddScoped<IRecopeService, RecopeService>();

builder.Services.AddControllers(options =>
{
    options.Filters.AddService<LogUserActionFilter>();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// aplica CORS antes de auth/routers
app.UseCors(MyCors);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
