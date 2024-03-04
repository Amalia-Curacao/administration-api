using FluentValidation;
using Scheduler.Api.Data.Models;

namespace Scheduler.Api.Data.Validators;

public class ScheduleInviteLinkValidator : AbstractValidator<ScheduleInviteLink>
{
	public ScheduleInviteLinkValidator()
	{
		RuleFor(l => l.ScheduleId)
			.NotNull().WithMessage("Schedule ID is required.");
		RuleFor(l => l.Role)
			.NotNull().WithMessage("Role is required.");
		RuleFor(l => l.ExpiresAt)
			.NotNull().WithMessage("Expiration is required.");
		RuleFor(l => l.Code)
			.NotNull().WithMessage("Code is required.")
			.NotEmpty().WithMessage("Code is required.");
		RuleFor(l => l.CreatedAt)
			.NotNull().WithMessage("Created at is required.");
	}
}
