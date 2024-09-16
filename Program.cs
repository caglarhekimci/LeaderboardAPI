using LeaderboardAPI.Data;
using LeaderboardAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Configuration;
using System.Text;

//Redis Ba�lant�s�: Redis servisi singleton olarak ba�land�.
//JWT Authentication: Kimlik do�rulama i�in JWT konfig�re edildi.
//Swagger: API dok�mantasyonu i�in Swagger kullan�ld�.

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add MySQL and Redis services
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("MySQL"),
    //MySQL veritaban�na ba�lanmak i�in kullan�l�r. MySQL ba�lant� dizisini appsettings.json dosyas�ndan al�r.
    new MySqlServerVersion(new Version(8, 0, 25))));
//MySQL s�r�m�n� belirtir. Kurulum s�ras�nda hangi s�r�m� y�klendiyse, ona g�re ayarlama yap�l�r (�rne�in 8.0.25).

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
    dbContext.Database.Migrate(); // Bu komut, veritaban�n� migrasyonlara g�re g�nceller.
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



