using LabTrack.Data;
using LabTrack.Models;
using LabTrack.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
var builder=WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(o=>o.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IPasswordHasher<Usuario>,PasswordHasher<Usuario>>();
builder.Services.AddScoped<ReservaService>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(o=>{
 o.LoginPath="/Cuenta/Login";o.AccessDeniedPath="/Cuenta/Denegado";o.Cookie.HttpOnly=true;o.Cookie.SecurePolicy=CookieSecurePolicy.SameAsRequest;o.Cookie.SameSite=SameSiteMode.Lax;o.ExpireTimeSpan=TimeSpan.FromHours(2);
});
builder.Services.AddAuthorization();
var app=builder.Build();
if(!app.Environment.IsDevelopment()){app.UseExceptionHandler("/Home/Error");app.UseHsts();}
app.UseHttpsRedirection();app.UseStaticFiles();app.UseRouting();app.UseAuthentication();app.UseAuthorization();
app.MapControllerRoute(name:"default",pattern:"{controller=Home}/{action=Index}/{id?}");
using(var scope=app.Services.CreateScope()){
 var db=scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
 await db.Database.EnsureCreatedAsync();
 await DemoSeed.SeedAsync(db,scope.ServiceProvider.GetRequiredService<IPasswordHasher<Usuario>>());
}
app.Run();
public partial class Program {}
