using System.Text.Json.Serialization;

namespace Scheduler.Api.Data.Models;


[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserRoles
{
	None = 0,
	Admin = 1,
	Housekeeper = 2,
	Manager = 3,
	Owner = 4,
}
