using Scheduler.Api.Security.Authorization;

namespace Scheduler.Api.UserProcess.Authorization;

public interface IAuthorizer
{
	public string[] Errors { get; }
	public void TryAuthorize<T>(T parameter, params IAuthorizeRequirement<T>[] requirements);
}
