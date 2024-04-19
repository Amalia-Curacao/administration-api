using Creative.Api.Implementations.EntityFrameworkCore;
using Creative.Api.Interfaces;
using FluentValidation;
using Scheduler.Api.Data;
using Scheduler.Api.Data.Models;

namespace Scheduler.Api.Security.Registration;

public class RegisterUser : IRegisterUserService
{
	public ICreate<User> CreateUser { get; }
	public IValidator<User> Validator { get; }
	public RegisterUser(ScheduleDb db, IValidator<User> validator)
	{
		CreateUser = new Crud<User>(db);
		Validator = validator;
	}

	public async Task<User> Register(User user)
	{
		await Validator.ValidateAndThrowAsync(user);
		return (await CreateUser.Add(true, user)).Single();
	}
}
