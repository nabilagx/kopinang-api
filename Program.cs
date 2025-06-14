using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using kopinang_api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Inisialisasi Firebase
var base64 = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIAL_B64");
if (string.IsNullOrEmpty(base64))
{
    throw new Exception("FIREBASE_CREDENTIAL_B64 is not set in environment variables.");
}

FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromJson(
        Encoding.UTF8.GetString(Convert.FromBase64String(base64))
    )
});

// JWT Auth
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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Controller + JSON
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });

// PostgreSQL
//builder.Services.AddDbContext<kopinang_api.Data.DBContext>(options =>
//     options.UseNpgsql(builder.Configuration.GetConnectionString("DATABASE_URL")));

// Parsing DATABASE_URL
// Parsing DATABASE_URL
var dbUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (string.IsNullOrEmpty(dbUrl))
    throw new Exception("DATABASE_URL environment variable is not set!");

var uri = new Uri(dbUrl);
var userInfo = uri.UserInfo.Split(':');
var connStr = new Npgsql.NpgsqlConnectionStringBuilder
{
    Host = uri.Host,
    Port = uri.Port,
    Username = userInfo[0],
    Password = userInfo[1],
    Database = uri.AbsolutePath.TrimStart('/'),
    SslMode = Npgsql.SslMode.Require,
    TrustServerCertificate = true
}.ToString();

Console.WriteLine("🟢 PostgreSQL connection string:");
Console.WriteLine(connStr);

// Gunakan koneksi ke PostgreSQL dari DATABASE_URL
builder.Services.AddDbContext<kopinang_api.Data.DBContext>(options =>
    options.UseNpgsql(connStr));


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Firestore
builder.Services.AddSingleton<FirestoreService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

app.UseCors("AllowAll");

// Aktifkan Swagger di semua environment (Dev & Production)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "kopinang-api v1");
    c.RoutePrefix = "swagger";
});

// Redirect root ke Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

// Middleware standar
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
