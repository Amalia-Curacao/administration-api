using FluentValidation;
using Scheduler.Api.Data.Models;

namespace Scheduler.Api.Data.Validators;

public class HousekeepingTaskValidator : AbstractValidator<HousekeepingTask>
{
	public HousekeepingTaskValidator()
	{
		RuleFor(task => task.Date)
			.NotNull().WithMessage("Date is required.");
		RuleFor(task => task.RoomNumber)
			.NotNull().WithMessage("Housekeeping task must have a valid room number.");
		RuleFor(task => task.RoomScheduleId)
			.NotNull().WithMessage("Housekeeping task must have a valid room schedule id.");
		RuleFor(task => task.ScheduleId)
			.NotNull().WithMessage("Housekeeping task need to belong to a schedule.");
		RuleFor(task => task.Type)
			.NotNull().WithMessage("Housekeeping task is required to have a type.")
			.NotEqual(HousekeepingTaskType.None).WithMessage("Housekeeping task can not have type none.");
	}
}
