using CargoTrack.Business.Abstract;
using CargoTrack.Business.Concrete;
using CargoTrack.Business.Mappings;
using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.Abstract;
using CargoTrack.DataAccess.Repositories.Concrete;
using CargoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using CargoTrack.Business.Validators;
using CargoTrack.DTO.CargoDtos;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("CargoTrackConnection");

builder.Services.AddDbContext<CargoTrackDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

builder.Services.AddScoped<ICargoService, CargoService>();
builder.Services.AddScoped<ICargoRepository, CargoRepository>();
builder.Services.AddScoped<ICargoPricingService, CargoPricingService>();
builder.Services.AddScoped<IValidator<CreateCargoDto>, CreateCargoValidator>();
builder.Services.AddScoped<IValidator<UpdateCargoDto>, UpdateCargoValidator>();
builder.Services.AddScoped<ICargoPriceService, CargoPriceService>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

builder.Services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<CargoTrackDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();
CargoMappingConfig.Register();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
