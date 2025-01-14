using Expense_Tracker.Models;
using Expense_Tracker.Models.Database;
using Microsoft.AspNetCore.Identity;
using AspNetCore.Identity.Mongo;
using Microsoft.Extensions.Options;
using Expense_Tracker.Models.UserIdentity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure MongoDB settings
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDBSettings"));

// Register the MongoDB context
builder.Services.AddSingleton<MongoDbContext>();

// Configure Identity with MongoDB
builder.Services.AddIdentityMongoDbProvider<ApplicationUser, ApplicationRole>(identityOptions =>
{
    identityOptions.Password.RequireDigit = true;
    identityOptions.Password.RequireLowercase = true;
    identityOptions.Password.RequireUppercase = true;
    identityOptions.Password.RequireNonAlphanumeric = false;
    identityOptions.Password.RequiredLength = 8;
    identityOptions.Lockout.MaxFailedAccessAttempts = 5;
    identityOptions.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    identityOptions.User.RequireUniqueEmail = true;
    
}, mongoOptions =>
{
    mongoOptions.ConnectionString = builder.Configuration.GetConnectionString("MongoIdentityConnection");
});

// authentication and authorization middleware
builder.Services.AddAuthentication(
    options =>
    {
        options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
        options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    }
    ).AddCookie(IdentityConstants.ApplicationScheme);
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Add this line to enable authentication
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
