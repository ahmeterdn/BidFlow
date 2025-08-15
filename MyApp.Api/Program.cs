using Microsoft.EntityFrameworkCore;
using BidFlow.Data;
using BidFlow.Api.Mappings;
using BidFlow.Repositories;
using BidFlow.Services;
using FluentValidation;
using BidFlow.Api.Data;
using AutoMapper;
using BidFlow.Common;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequirePermission:Users.Read", policy =>
        policy.Requirements.Add(new BidFlow.Authorization.PermissionRequirement("Users.Read")));

    options.AddPolicy("RequirePermission:Users.Create", policy =>
        policy.Requirements.Add(new BidFlow.Authorization.PermissionRequirement("Users.Create")));

    options.AddPolicy("RequirePermission:Users.Update", policy =>
        policy.Requirements.Add(new BidFlow.Authorization.PermissionRequirement("Users.Update")));

    options.AddPolicy("RequirePermission:Users.Delete", policy =>
        policy.Requirements.Add(new BidFlow.Authorization.PermissionRequirement("Users.Delete")));

    options.AddPolicy("RequirePermission:Users.Activate", policy =>
        policy.Requirements.Add(new BidFlow.Authorization.PermissionRequirement("Users.Activate")));

    options.AddPolicy("RequireAdmin", policy =>
        policy.RequireRole("Admin", "SuperAdmin"));

    options.AddPolicy("RequireSuperAdmin", policy =>
        policy.RequireRole("SuperAdmin"));

    options.AddPolicy("RequirePermission:Auth.Profile.Read", policy =>
        policy.Requirements.Add(new BidFlow.Authorization.PermissionRequirement("Auth.Profile.Read")));

    options.AddPolicy("RequirePermission:Auth.Password.Change", policy =>
        policy.Requirements.Add(new BidFlow.Authorization.PermissionRequirement("Auth.Password.Change")));
});

builder.Services.AddScoped<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, BidFlow.Authorization.PermissionAuthorizationHandler>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuditInterceptor>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<MappingProfile>();
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    await context.Database.EnsureCreatedAsync();

    await BidFlow.Data.SeedData.RBACSeeder.SeedAsync(context);
}

app.Run();