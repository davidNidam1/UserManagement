using UserManagement.Services; 
using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models; // For swagger
using System.Text; // For Encoding the token

//Create the server
var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // Allow frontend requests
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});


// Hardcoded ConnectionString for MongoDB
var connectionString = "mongodb://localhost:27017";
Console.WriteLine($" Using hardcoded MongoDB ConnectionString: {connectionString}");

// Register MongoDB Client as a Singleton Service
// Singleton since we need constant connection to the db
builder.Services.AddSingleton<IMongoClient>(s => new MongoClient(connectionString));

// Register Application Services only once
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<AuthService>(); 

// Configure JWT Authentication (Development Example)
var key = Encoding.UTF8.GetBytes("A1b2C3d4E5f6G7h8I9J0kLmNOpQrStUvWxYz123456");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false, // Not verifying the token issuer in this setup
            ValidateAudience = false, // Not verifying audience
            ValidateLifetime = true, // Ensure token is still valid
            ValidateIssuerSigningKey = true, // Check the validity of the signing key
            IssuerSigningKey = new SymmetricSecurityKey(key) // Secret key for signing
        };
    });

builder.Services.AddAuthorization(); // Enable authorization policies

// Add JWT Authentication Support to Swagger UI
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Register API Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Enable the server listening to requests
var app = builder.Build();

// Enable Swagger UI in Development Mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS
app.UseCors(MyAllowSpecificOrigins); // Apply CORS policy
app.UseAuthentication(); // Enable JWT authentication
app.UseAuthorization(); // Apply authorization policies
app.MapControllers(); // Activate all API controllers
app.Run(); // Start the web application

public partial class Program { }
