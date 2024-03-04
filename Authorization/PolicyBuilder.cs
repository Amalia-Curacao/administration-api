using Microsoft.AspNetCore.Authorization;

namespace Scheduler.Api.Authorization;

// Builds policies with [Policy.keyword]:[Policy.Types.Name]/[Policy.parameters]
// [Policy.keyword]*[Policy.Types.Name]*[Policy.parameters]
public static class PolicyBuilder
{
	public static void AddPolicies(this AuthorizationOptions options, string keyword, 
		(string, IAuthorizationRequirementWithParameter)[] policies, params (string, object?)[] parameters)
	{
		foreach (var (name, requirement) in policies)
		{
			options.AddPolicy($"{keyword}:{name}", policy => policy.Requirements.Add(requirement));

			if (parameters.Length > 0) foreach (var (parameterName, parameterValue) in parameters)
				options.AddPolicy($"{keyword}:{name}/{parameterName}", policy => policy.Requirements.Add(requirement.WithParameter(parameterValue)));
		}		
	}
}