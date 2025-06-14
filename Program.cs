using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using kopinang_api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Inisialisasi Firebase (ambil path dari appsettings.json)
var base64 = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIAL_B64");
if (string.IsNullOrEmpty(base64))
{
    throw new Exception("FIREBASE_CREDENTIAL_B64 is not set in environment variables.");
}

FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromJson(
        Encoding.UTF8.GetString(Convert.FromBase64String(
            Environment.GetEnvironmentVariable("FIREBASE_CREDENTIAL_B64")
        ))
    )
});



// Tambahkan JWT Authentication
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

// Controller + JSON settings
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });

// Database (PostgreSQL)
builder.Services.AddDbContext<kopinang_api.Data.DBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Swagger (API Docs)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Firestore Service (khusus promo)
builder.Services.AddSingleton<FirestoreService>();

// ORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

// Middleware
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "kopinang-api v1");
        c.RoutePrefix = "swagger";
    });
}


app.UseHttpsRedirection();


app.UseAuthentication();  //  Diperlukan agar [Authorize] berfungsi
app.UseAuthorization();

app.MapControllers();
app.Run();
