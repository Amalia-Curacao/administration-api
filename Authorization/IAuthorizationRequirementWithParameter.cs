using Microsoft.AspNetCore.Authorization;

namespace Scheduler.Api.Authorization;

public interface IAuthorizationRequirementWithParameter : IAuthorizationRequirement
{
	public object? Parameter { get; }
	public IAuthorizationRequirementWithParameter WithParameter(object? parameter);
}
