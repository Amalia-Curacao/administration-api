using System.Net.Http.Headers;

namespace Scheduler.Api.Controllers.Internal.Auth0;

internal sealed class Auth0Api
{
	private HttpClient HttpClient { get; }
	public Auth0Api(Uri domain)
	{
		HttpClient = new HttpClient
		{
			BaseAddress = domain ?? throw new ArgumentNullException(nameof(domain)),
			DefaultRequestHeaders =
			{
				Accept = { new MediaTypeWithQualityHeaderValue("application/json") }
			}
		};
	}


	internal async Task<Auth0UserInfo> UserInfo(string accessToken)
	{
		HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
		var response = await HttpClient.GetAsync("/userinfo");
		if (response.IsSuccessStatusCode)
		{
			return await response.Content.ReadFromJsonAsync<Auth0UserInfo>()
				?? throw new ArgumentNullException("The response content from auth0 is null.");
		}
		else
		{
			throw new InvalidOperationException("The access token is not valid.");
		}
	}


	internal sealed class Auth0UserInfo
	{
		/// <summary> The auth0 user id </summary> 
		public required string Sub { get; init; }
		public required string Nickname { get; init; }
		public required string Name { get; init; }
		public required string Picture { get; init; }
		public DateTime? UpdatedAt { get; init; }
		public required string Email { get; init; }
		public bool? EmailVerified { get; init; }
	}
}
