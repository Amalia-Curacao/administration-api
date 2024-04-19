namespace Scheduler.Api.Security.Authorization;

public interface IAuthorizeRequirement<T>
{
	public string ErrorMessage { get; }
	public Task<bool> Authorize(T? parameter);
}
