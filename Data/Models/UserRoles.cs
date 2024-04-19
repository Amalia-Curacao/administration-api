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

public static class UserRolesExtensions
{
	public static bool HasAuthorityOver(this UserRoles role, UserRoles other) => role switch
	{
		UserRoles.Admin => true,
		UserRoles.Housekeeper => other == UserRoles.None,
		UserRoles.Manager => other == UserRoles.Housekeeper || other == UserRoles.None,
		UserRoles.Owner => other == UserRoles.Manager || other == UserRoles.Housekeeper || other == UserRoles.None,
		_ => false,
	};
}
