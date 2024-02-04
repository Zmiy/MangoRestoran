using Mango.Services.Identity;
using Mango.Services.Identity.DbContext;
using Mango.Services.Identity.Initializer;
using Mango.Services.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    );
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

var bld = builder.Services.AddIdentityServer(options =>
{
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseSuccessEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.EmitStaticAudienceClaim = true;
}).AddInMemoryIdentityResources(SD.IdentityResources)
.AddInMemoryApiScopes(SD.ApiScopes)
.AddInMemoryClients(SD.Clients)
.AddAspNetIdentity<ApplicationUser>();

bld.AddDeveloperSigningCredential(); 

builder.Services.AddScoped<IDbInitializer, DbInitializer>();


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
using (var scope = app.Services.CreateScope())
{
    var dbinit = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    dbinit.Initialize();
    // use context
}
//var dbinit = app.Services.GetService<IDbInitializer>();

app.UseRouting();

app.UseIdentityServer();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
