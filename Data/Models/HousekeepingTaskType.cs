using System.Text.Json.Serialization;

namespace Scheduler.Api.Data.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum HousekeepingTaskType
{
	None = 0,
	Towels = 1,
	Bedsheets = 2,
	All = 3,
}
