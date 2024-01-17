using FluentValidation;
using Scheduler.Api.Data.Models;

namespace Scheduler.Api.Data.Validators;

public class HousekeeperValidator : AbstractValidator<Housekeeper>
{
	public HousekeeperValidator()
	{
		RuleFor(h => h.FirstName)
			.NotEmpty().WithMessage($"Housekeeper's first name is required.");
		RuleFor(h => h.ScheduleId)
			.NotEmpty().WithMessage($"Housekeeper's schedule id is required.")
			.NotNull().WithMessage($"Housekeeper's schedule id is required.");
	}
}
