using Scheduler.Api.Data.Models;

namespace Scheduler.Api.Authorization.Auth0;

public class HasRoleRequirement : IAuthorizationRequirementWithParameter
{
    public UserRoles Role { get; }
	public object? Parameter { get; }

	public HasRoleRequirement(UserRoles role, object? parameter = null)
    {
        Role = role;
        Parameter = parameter;
    }

	public IAuthorizationRequirementWithParameter WithParameter(object? parameter)
	{
		return new HasRoleRequirement(Role, parameter);
	}
}
