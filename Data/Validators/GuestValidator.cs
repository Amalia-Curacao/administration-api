using FluentValidation;
using Scheduler.Api.Data.Models;

namespace Scheduler.Api.Data.Validators;

public class GuestValidator : AbstractValidator<Guest>
{
	public GuestValidator()
	{
		RuleFor(p => p.LastName)
			.NotEmpty().WithMessage($"Guest's last name is required.")
			.MaximumLength(50).WithMessage($"Guest's last name can not be longer than 50 characters.");
		/* RuleFor(p => p.Age)
			.NotEmpty().WithMessage($"Guest's age is required.")
			.InclusiveBetween(0, 150).WithMessage($"Guest's age needs to be between 0 and 150 years.");
		RuleFor(p => p.FirstName)
			.NotEmpty().WithMessage($"Guest's first name is required.")
			.MaximumLength(50).WithMessage($"Guest's first name can not be longer than 50 characters.");
		 RuleFor(p => p.Prefix)
			.IsInEnum().WithMessage($"Guest's prefix needs to match with one of the following \"Mr.\", \"Ms\", \"Mrs.\" or \"Other\"."); */
		RuleFor(p => p.ReservationId)
			.NotEmpty().WithMessage($"Guest's reservation id is required.")
			.NotNull().WithMessage($"Guest's reservation id is required.");
	}
}
