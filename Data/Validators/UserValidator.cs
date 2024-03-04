using FluentValidation;
using Scheduler.Api.Data.Models;

namespace Scheduler.Api.Data.Validators;

public class UserValidator : AbstractValidator<User>
{
	public UserValidator()
	{
		RuleFor(h => h.FirstName)
			.NotEmpty().WithMessage($"User's first name is required.");
	}
}
