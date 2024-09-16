using LeaderboardAPI.Data;
using LeaderboardAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Configuration;
using System.Text;

//Redis Baðlantýsý: Redis servisi singleton olarak baðlandý.
//JWT Authentication: Kimlik doðrulama için JWT konfigüre edildi.
//Swagger: API dokümantasyonu için Swagger kullanýldý.

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add MySQL and Redis services
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("MySQL"),
    //MySQL veritabanýna baðlanmak için kullanýlýr. MySQL baðlantý dizisini appsettings.json dosyasýndan alýr.
    new MySqlServerVersion(new Version(8, 0, 25))));
//MySQL sürümünü belirtir. Kurulum sýrasýnda hangi sürümü yüklendiyse, ona göre ayarlama yapýlýr (örneðin 8.0.25).

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse($"{builder.Configuration["Redis:Host"]}:{builder.Configuration["Redis:Port"]}", true);
    return ConnectionMultiplexer.Connect(configuration);
});

// Add JWT authentication
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Add other services (e.g., LeaderboardService, AuthService)
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<LeaderboardService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate(); // Bu komut, veritabanýný migrasyonlara göre günceller.
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();



