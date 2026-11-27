using CargoTrack.Business.Abstract;
using CargoTrack.Business.Concrete;
using CargoTrack.Business.Mappings;
using CargoTrack.Business.Validators;
using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.Abstract;
using CargoTrack.DataAccess.Repositories.Concrete;
using CargoTrack.DTO.CargoDtos;
using CargoTrack.Entity.Entities;
using CargoTrack.WebUI.Seed;
using CargoTrack.WebUI.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// SQL Server
var connectionString = builder.Configuration
    .GetConnectionString("CargoTrackConnection");

builder.Services.AddDbContext<CargoTrackDbContext>(
    options => options.UseSqlServer(connectionString));

// Generic Repository
builder.Services.AddScoped(
    typeof(IRepository<>),
    typeof(GenericRepository<>));

// Cargo
builder.Services.AddScoped<ICargoService, CargoService>();
builder.Services.AddScoped<ICargoRepository, CargoRepository>();

// Pricing
builder.Services.AddScoped<ICargoPricingService, CargoPricingService>();
builder.Services.AddScoped<ICargoPriceService, CargoPriceService>();

// Validation
builder.Services.AddScoped<
    IValidator<CreateCargoDto>,
    CreateCargoValidator>();

builder.Services.AddScoped<
    IValidator<UpdateCargoDto>,
    UpdateCargoValidator>();

builder.Services.AddScoped<
    IValidator<ChangeCargoStatusDto>,
    ChangeCargoStatusDtoValidator>();

builder.Services.AddScoped<
    IManagerDashboardRepository,
    ManagerDashboardRepository>();

builder.Services.AddScoped<
    IManagerDashboardService,
    ManagerDashboardService>();

builder.Services.AddScoped<
    IUserDashboardRepository,
    UserDashboardRepository>();

builder.Services.AddScoped<
    IUserDashboardService,
    UserDashboardService>();


// Domain Services
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ITransferCenterService, TransferCenterService>();
builder.Services.AddScoped<IDeliveryService, DeliveryService>();

// Cargo Search
builder.Services.AddScoped<ICargoSearchRepository,CargoSearchRepository>();

// AuditLog
builder.Services.AddScoped<IAuditLogReadRepository,AuditLogReadRepository>();

builder.Services.AddScoped<IAuditLogReadService,AuditLogReadService>();

// Reporting
builder.Services.AddScoped<IAdminReportRepository,AdminReportRepository>();

builder.Services.AddScoped<IAdminReportService,AdminReportService>();

// Current User
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICurrentUserContext,HttpCurrentUserContext>();

// Identity
builder.Services.AddIdentity<AppUser, AppRole>()
.AddEntityFrameworkStores<CargoTrackDbContext>();

// MVC
builder.Services.AddControllersWithViews();

// Mapster
CargoMappingConfig.Register();

var app = builder.Build();

// Roles
await RoleSeeder.SeedAsync(app.Services);

// Exception Handling
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error/Index");
    app.UseHsts();
}

// Middleware
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Root: Login
app.MapControllerRoute(
    name: "root",
    pattern: "",
    defaults: new
    {
        controller = "Account",
        action = "Login"
    });

// Default MVC Route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
