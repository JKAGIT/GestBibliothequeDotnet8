using GestBibliothequeDotnet8.Donnee;
using GestBibliothequeDotnet8.Repositories;
using GestBibliothequeDotnet8.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Serilog;
using Serilog.Extensions.Hosting;
using Serilog.Settings.Configuration; // Add this namespace for 'ReadFrom.Configuration'

var builder = WebApplication.CreateBuilder(args);

//// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) // Ensure the correct namespace is included
    .CreateLogger();
builder.Host.UseSerilog(Log.Logger);

builder.Services.AddDbContext<GestBibliothequeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("GestBibliothequeDbConnect")));


builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IEntityValidationService<>), typeof(EntityValidationService<>));
builder.Services.AddScoped(typeof(IRecherche<>), typeof(Recherche<>));
builder.Services.AddScoped<ICategories, CategoriesService>();
builder.Services.AddScoped<IEmprunts, EmpruntsService>();
builder.Services.AddScoped<ILivres, LivresService>();
builder.Services.AddScoped<IReservations, ReservationsService>();
builder.Services.AddScoped<IUsagers, UsagersService>();
builder.Services.AddScoped<IUtilisateurs, UtilisateursService>();
builder.Services.AddScoped<IRetours, RetoursService>();


builder.Services.AddScoped<GenerateurMatriculeUnique>();

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));


// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

//app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
