using Creative.Api.Implementations.EntityFrameworkCore;
using Creative.Api.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Scheduler.Api.Data;
using Scheduler.Api.Data.Models;

namespace Scheduler.Api.Controllers;

public class HousekeeperTasksController : Controller
{
	private ICrud<HousekeepingTask> _crud { get; init; }
	private IValidator<HousekeepingTask> _validator { get; init; }
	public HousekeeperTasksController(ScheduleDb db, IValidator<HousekeepingTask> validator)
	{
		_crud = new Crud<HousekeepingTask>(db);
		_validator = validator;
	}

}
