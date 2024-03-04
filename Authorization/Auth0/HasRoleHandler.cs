using Creative.Api.Implementations.EntityFrameworkCore;
using Creative.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Scheduler.Api.Data;
using Scheduler.Api.Data.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Scheduler.Api.Authorization.Auth0;

public class HasRoleHandler : AuthorizationHandler<HasRoleRequirement>
{
    private HttpClient HttpClient { get; }
    private IRead<ScheduleInviteLink> ScheduleInviteLinks { get; }
    private IEnumerable<string> ControllerNames { get; }

    /// <param name="db"> Database with the User with the roles that they have on their respected schedules. </param>
    /// <param name="auth0Domain"> The auth0 domain url </param>
    /// <param name="controllerNames"></param>
    public HasRoleHandler(ScheduleDb db, Uri auth0Domain, params string[] controllerNames)
    {
        // setup http client
        HttpClient = new HttpClient();
        HttpClient.BaseAddress = auth0Domain ?? throw new ArgumentNullException(nameof(auth0Domain));
        HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        ScheduleInviteLinks = new Crud<ScheduleInviteLink>(db ?? throw new ArgumentNullException(nameof(db)), db.ScheduleInviteLinks.Include(l => l.User));

        ControllerNames = controllerNames.Select(c => c.Replace("Controller", ""));
    }

    protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasRoleRequirement requirement)
    {
        if (requirement.Role == UserRoles.None) context.Succeed(requirement);
        var auth0Id = await VerifyAccessToken(GetAccessToken(context));
        var userInviteLinks = await ScheduleInviteLinks.Get(l => l.User is not null && l.User.Auth0Id == auth0Id);

        if (userInviteLinks.Any(l => l.Role! == UserRoles.Admin)) context.Succeed(requirement);
        // Get requested users invite links
        var scheduleId = await GetScheduleId(context, requirement);
        var scheduleLinks = userInviteLinks.Where(l => l.ScheduleId is null || l.ScheduleId == scheduleId);
        // Check if the user has the required role and the invite link is not disabled
        if (scheduleLinks.Any(l => !l.Disabled && l.Role == requirement.Role)) context.Succeed(requirement);

        else context.Fail();
    }

    private async Task<int> GetScheduleId(AuthorizationHandlerContext context, HasRoleRequirement requirement)
    {
        if (context.Resource is DefaultHttpContext resource)
        {
            int? scheduleId = (requirement.Parameter as string) switch
			    {
				    "ScheduleId" => TryScheduleIdRouteValue(resource.Request.RouteValues),
				    "Id" => TryIdRouteValueControllers(resource.Request.RouteValues),
				    "Body" => await TryScheduleIdRequestBody(resource.Request.Body),
				    _ => TryScheduleIdRouteValue(resource.Request.RouteValues)
				        ?? await TryScheduleIdRequestBody(resource.Request.Body)
				        ?? TryIdRouteValueControllers(resource.Request.RouteValues),
			    };

			return scheduleId ?? throw new ArgumentNullException("No schedule id could be found.");
        }
        else
        {
            throw new InvalidOperationException("The context resource is not a DefaultHttpContext.");
        }

        // tries to get the schedule id from the route values as 'ScheduleId'
        int? TryScheduleIdRouteValue(RouteValueDictionary routeValues)
        {
            if (routeValues.TryGetValue("ScheduleId", out object? scheduleId))
                if (scheduleId is not null) return int.Parse((string)scheduleId!);

            return null;
        }

        // tries to get the schedule id from the request body as 'ScheduleId'
        async Task<int?> TryScheduleIdRequestBody(Stream requestBody)
        {
            using var streamReader = new StreamReader(requestBody);
            var stream = await streamReader.ReadToEndAsync();
            if (string.IsNullOrEmpty(stream)) return null;

            var json = JsonSerializer.Deserialize<JsonObject>(stream);
            if (json is not null)
            {
                json.TryGetPropertyValue("ScheduleId", out var node);
                if (node is not null) return int.Parse(node.AsValue().ToString());
            }
            return null;
        }

        // tries to get the schedule id from the request path as 'Id'
        int? TryIdRouteValueControllers(RouteValueDictionary routeValues)
        {
            if (ControllerNames.Any(c => resource.Request.Path.ToString().Contains(c)))
            {
                if (routeValues.TryGetValue("Id", out object? id))
                    if (id is not null) return int.Parse((string)id!);
            }
            return null;
        }
    }

    /// <summary> Verifies the auth0 access token. </summary>
    /// <returns> Auth0 user id. </returns>
    /// <exception cref="ArgumentNullException">The response content from auth0 is null.</exception>
    /// <exception cref="InvalidOperationException"> The access token is not valid. </exception>
    private async Task<string> VerifyAccessToken(string accessToken)
    {
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await HttpClient.GetAsync("/userinfo");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadFromJsonAsync<SampleResponse>();
            return content is null ? throw new ArgumentNullException("The response content is null.") : content.Sub;
        }
        else
        {
            throw new InvalidOperationException("The access token is not valid.");
        }
    }
    private static string GetAccessToken(AuthorizationHandlerContext context)
    {
        if (context.Resource is DefaultHttpContext resource)
        {
            bool take = false;
            return resource.Request.Headers["Authorization"].ToString().Split(" ").SingleOrDefault(s =>
            {
                if (take) return true;
                if (s == "Bearer") take = true;
                return false;
            }) ?? throw new ArgumentNullException("Authorization header does not contain a access token.");
        }
        else
        {
            throw new InvalidOperationException("The context resource is not a DefaultHttpContext.");
        }
    }

    private sealed class SampleResponse
    {
        public required string Sub { get; init; }
    }
}
