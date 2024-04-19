namespace Microsoft.AspNetCore.Http;

internal static class HttpContextExtensions
{
	public const string MissingAccessTokenException = $"Access token could not be found in the request headers; Add it in the 'Authorization' header under 'Bearer' for Auth0 or 'Token' for the Creative.Security.";

	public static string? Auth0AccessToken(this HttpContext context)
	{
		if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
		{
			bool take = false;
			return authHeader.ToString().Split(' ').SingleOrDefault(s =>
			{
				if (take) return true;
				if (s == "Bearer") take = true;
				return false;
			});
		}
		return null;
	}

	public static string? AccessToken(this HttpContext context)
	{
		if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
		{
			bool take = false;
			return authHeader.ToString().Split(' ').SingleOrDefault(s =>
			{
				if (take) return true;
				if (s == "Token") take = true;
				return false;
			});
		}
		return null;
	}
}
