namespace Microsoft.AspNetCore.Http;

internal static class HttpContextExtensions
{
	public static string AccessToken(this HttpContext context)
	{
		if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
		{
			bool take = false;
			var accessToken = authHeader.ToString().Split(' ').SingleOrDefault(s =>
			{
				if (take) return true;
				if (s == "Bearer") take = true;
				return false;
			}) ?? throw new InvalidOperationException("The access token is not in the request headers.");
			return accessToken;
		}
		throw new InvalidOperationException("The access token is not in the request headers.");
	}
}
