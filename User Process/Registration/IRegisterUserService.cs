using Scheduler.Api.Data.Models;

namespace Scheduler.Api.Security.Registration;

public interface IRegisterUserService
{
	public Task<User> Register(User user);
}
