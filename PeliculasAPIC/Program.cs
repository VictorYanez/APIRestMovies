using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;

using PeliculasAPI;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
var startup = new Startup(builder.Configuration);

startup.ConfigureServices(builder.Services);

var app = builder.Build();

// Generate a password hash for the password
// Metodo Temporal para generar el hash de una contraseña
//var hasher = new PasswordHasher<IdentityUser>();
//var hash = hasher.HashPassword(null, "VJda1122$");
//  valor generado es AQAAAAIAAYagAAAAEBoWC44Q2r5031a5LXYaoYNidHgLORBnGJhgwfkbR3KQ1B4puRmQ8YFfQiWcs5uHrw==
//Console.WriteLine($"Hash generado: {hash}");

startup.Configure(app, app.Environment);

app.Run();
