using Scheduler.Api.UserProcess.Authorization;

namespace Scheduler.Api.Security.Authorization;

public class Authorizer : IAuthorizer
{
	public string[] Errors { get => _errors.ToArray(); }
	private IEnumerable<string> _errors = [];
	public async void TryAuthorize<T>(T parameter, params IAuthorizeRequirement<T>[] requirements)
	{
		foreach (var req in requirements)
		{
			if (!await req.Authorize(parameter))
			{
				_errors = _errors.Append(req.ErrorMessage);
			}
		}
	}
}
