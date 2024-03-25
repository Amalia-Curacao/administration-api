using Creative.Database.Data;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scheduler.Api.Controllers.Internal.Auth0;
using Scheduler.Api.Data;
using Scheduler.Api.Data.Models;
using Scheduler.Api.Data.Validators;
using Scheduler.Api.Security.Authenticator;
using Scheduler.Api.Security.Authenticator.Auth0;
using Scheduler.Api.Security.Authorization;
using Scheduler.Api.Security.Registration;
using Scheduler.Api.UserProcess;
using Scheduler.Api.UserProcess.Authorization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Register database context
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var connectionString = builder.Configuration.GetConnectionString("POSTGRESQL_CONNECTIONSTRING")
	?? throw new NullReferenceException($"The database connection string cannot be null.");
var options = new PostgreSqlOptions() { ConnectionString = connectionString };
var createDb = () => ScheduleDb.Create(options);
builder.Services.AddDbContext<ScheduleDb>(options => createDb());


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

// Register authenticator
builder.Services.AddScoped<Auth0Api>(_ => new(new Uri(domain)));
builder.Services.AddScoped<IAuthorizer, Authorizer>();
builder.Services.AddScoped<IRegisterUserService, RegisterUser>(services => new RegisterUser(createDb(), services.GetService<IValidator<User>>()!));
builder.Services.AddScoped<IAuthenticator, Auth0Authenticator>(services => new Auth0Authenticator(createDb(), services.GetService<Auth0Api>()!, services.GetService<IRegisterUserService>()!));
builder.Services.AddScoped<UserProcessor>();

// Register validators
builder.Services.AddScoped<IValidator<Guest>, GuestValidator>();
builder.Services.AddScoped<IValidator<Reservation>, ReservationValidator>();
builder.Services.AddScoped<IValidator<Room>, RoomValidator>();
builder.Services.AddScoped<IValidator<User>, UserValidator>();
builder.Services.AddScoped<IValidator<ScheduleInviteLink>, ScheduleInviteLinkValidator>();
builder.Services.AddScoped<IValidator<HousekeepingTask>, HousekeepingTaskValidator>();
builder.Services.AddScoped<IValidator<ScheduleInviteLink>, ScheduleInviteLinkValidator>();

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
else
{
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