using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using kopinang_api.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });

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

builder.Services.AddDbContext<kopinang_api.Data.DBContext>(options =>
    options.UseNpgsql(connStr));

builder.Services.AddSingleton<FirestoreService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

app.UseCors("AllowAll");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "kopinang-api v1");
    c.RoutePrefix = "swagger";
});

// Redirect ke Swagger di root
app.MapGet("/", () => Results.Redirect("/swagger"));


app.UseMiddleware<kopinang_api.Middleware.FirebaseAuthMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
