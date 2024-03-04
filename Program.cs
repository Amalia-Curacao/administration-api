using Creative.Database.Data;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Scheduler.Api.Authorization;
using Scheduler.Api.Authorization.Auth0;
using Scheduler.Api.Controllers;
using Scheduler.Api.Data;
using Scheduler.Api.Data.Models;
using Scheduler.Api.Data.Validators;
using Scheduler.Api.Data.Validators.Abstract;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Register database context
var connectionString = builder.Configuration.GetConnectionString("POSTGRESQL_CONNECTIONSTRING")
    ?? throw new NullReferenceException($"The database connection string cannot be null.");
var options = new PostgreSqlOptions() { ConnectionString = connectionString };
var createDb= () => ScheduleDb.Create(options);
builder.Services.AddDbContext<ScheduleDb>(_ => createDb());

// Register validators
builder.Services.AddScoped<IValidator<Guest>, GuestValidator>();
builder.Services.AddScoped<IValidator<Reservation>, ReservationValidator>();
builder.Services.AddScoped<IValidator<Room>, RoomValidator>();  
builder.Services.AddScoped<IValidator<User>, UserValidator>();
builder.Services.AddScoped<IValidator<ScheduleInviteLink>, ScheduleInviteLinkValidator>();
builder.Services.AddScoped<IValidator<HousekeepingTask>, HousekeepingTaskValidator>();
builder.Services.AddScoped<RelationshipValidator<Reservation>, ReservationRelationshipValidator>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Auth0
var domain = $"https://{builder.Configuration["Auth0:Domain"]}/";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
	    options.Authority = domain;
	    options.Audience = builder.Configuration["Auth0:Audience"];
	    options.TokenValidationParameters = new TokenValidationParameters
	    {
		    NameClaimType = ClaimTypes.NameIdentifier
	    };
    });

builder.Services.AddAuthorization(options =>
{
options.AddPolicies("role",[
        ("none", new HasRoleRequirement(UserRoles.None)),
	    ("admin", new HasRoleRequirement(UserRoles.Admin)),
	    ("manager", new HasRoleRequirement(UserRoles.Manager)),
	    ("housekeeper", new HasRoleRequirement(UserRoles.Housekeeper)),
	    ("owner", new HasRoleRequirement(UserRoles.Owner))],
        ("param=scheduleId", "ScheduleId"), ("param=id", "Id"), ("body=scheduleId", "Body"));
});

builder.Services.AddSingleton<IAuthorizationHandler, HasRoleHandler>(_ => new HasRoleHandler(createDb(), new Uri(domain), nameof(SchedulesController)));

builder.Services.AddControllers();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapControllerRoute(
        name: "default",
        pattern: "swagger"
        );
}
else{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.MapControllers();
}

// TODO think about CORS
app.UseCors(builder => builder.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Auth0
app.UseAuthentication();
app.UseAuthorization();

app.Run();