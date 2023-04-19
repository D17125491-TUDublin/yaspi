using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using yaspi.mvc.Code;
using yaspi.mvc.Data;
using yaspi.common;
using yaspi.mvc.BackgroundServices;
using yaspi.mvc;
using yaspi.integration.facebook;
using yaspi.integration.twitter;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
    
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

// ***************************************************************
// Dependency injection - registering services used by controllers
builder.Services.AddSingleton<IEventBusSubscriber,EventBusSubscriber>();
builder.Services.AddSingleton<IConnectionManager,LocalServicesConnectionManager>();
builder.Services.AddSingleton<IEventBus,EventBus>();
builder.Services.AddSingleton<TwitterBackgroundService>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<TwitterBackgroundService>());
builder.Services.AddSingleton<FacebookBackgroundService>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<FacebookBackgroundService>());

bool USE_TEST_API_SERVICES = true;

if (USE_TEST_API_SERVICES)
{
    builder.Services.AddSingleton<IFacebookApiService,TestFacebookApiService>();
    builder.Services.AddSingleton<ITwitterApiService,TestTwitterApiService>();
}
else
{
    builder.Services.AddSingleton<IFacebookApiService,FacebookApiService>();
    builder.Services.AddSingleton<ITwitterApiService,TwitterApiService>();
}
// ****************************************************************



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
