using Indigosoft.User.Ip.Tracker.Database.Dal;
using Indigosoft.User.Ip.Tracker.Database.Services;
using Indigosoft.User.Ip.Tracker.Infrastructure;
using Indigosoft.User.Ip.Tracker.Infrastructure.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace Indigosoft.User.Ip.Tracker.Service.v1;

public static class IPConnectionController
{
	/// <summary>Url.</summary>
	private static readonly string Url = "api/v1/connections";

	public static WebApplication MapIPConnectionEndpoints(this WebApplication app)
	{
		app.MapPostAddOrUpdateConnection()
			.MapGetUserIps()
            .MapGetLastConnectionAsync()
            .MapGetUsersByIpFragmentAsync();
		return app;
	}

    #region POST AddOrUpdateConnection

    /// <summary>
    /// Maps the endpoint for adding or updating a user's IP connection.
    /// </summary>
    /// <param name="app">The web application instance.</param>
    /// <returns>The web application with the route configured.</returns>
    private static WebApplication MapPostAddOrUpdateConnection(this WebApplication app)
    {
        app.MapPost($"{Url}/add", AddOrUpdateConnectionAsync).WithOpenApi(operation =>
        {
            operation.Responses.Add("500", new OpenApiResponse() { Description = "Failed to process the connection." });
            operation.Responses["200"].Description = "Connection successfully saved.";
            operation.Summary = "Add or update a user IP connection.";
            operation.Tags = [new OpenApiTag() { Name = "IP Connections" }];
            return operation;
        });

        return app;
    }

    /// <summary>
    /// Adds a new user IP connection or updates it if it already exists.
    /// </summary>
    private static async Task<IResult> AddOrUpdateConnectionAsync(
        [FromServices] IIPConnectionDatabaseService ipConnectionManager,
        [FromBody] IPConnectionRequest data)
    {
        try
        {
            await ipConnectionManager.AddConnectionInQueue(data);
            return TypedResults.Ok();
        }
        catch(InvalidIpAddressFormatException ex)
        {
            return TypedResults.Problem(new ProblemDetails
            {
                Status = 400,
                Detail = ex.Message
            });
        }
        catch(Exception ex)
        {
            return TypedResults.Problem(ex.Message);
        }
    }

    #endregion

    #region GET GetUserIps

    /// <summary>
    /// Maps the endpoint for retrieving all IP addresses used by a specific user.
    /// </summary>
    private static WebApplication MapGetUserIps(this WebApplication app)
    {
        app.MapGet($"{Url}/ips", GetUserIpsAsync).WithOpenApi(operation =>
        {
            operation.Responses.Add("500", new OpenApiResponse() { Description = "Failed to retrieve user IPs." });
            operation.Responses["200"].Description = "User IPs retrieved successfully.";
            operation.Summary = "Get all IP addresses used by a specific user.";
            operation.Tags = [new OpenApiTag() { Name = "IP Connections" }];
            return operation;
        });

        return app;
    }

    /// <summary>
    /// Returns a list of IP addresses used by the specified user.
    /// </summary>
    private static async Task<IResult> GetUserIpsAsync(
        [FromServices] IIPConnectionDatabaseService ipConnectionManager,
        [FromQuery] long userId)
    {
        try
        {
            var result = await ipConnectionManager.GetUserIpsAsync(userId);
            return TypedResults.Json(result);
        }
        catch(Exception ex)
        {
            return TypedResults.Problem(ex.Message);
        }
    }

    #endregion

    #region GET GetLastConnection

    /// <summary>
    /// Maps the endpoint for retrieving the last IP connection of a user.
    /// </summary>
    private static WebApplication MapGetLastConnectionAsync(this WebApplication app)
    {
        app.MapGet($"{Url}/lastConnection", GetLastConnectionAsync).WithOpenApi(operation =>
        {
            operation.Responses.Add("500", new OpenApiResponse() { Description = "Failed to retrieve the last connection." });
            operation.Responses["200"].Description = "Last connection retrieved successfully.";
            operation.Summary = "Get the most recent IP connection of a user.";
            operation.Tags = [new OpenApiTag() { Name = "IP Connections" }];
            return operation;
        });

        return app;
    }

    /// <summary>
    /// Returns the most recent IP connection of a specified user.
    /// </summary>
    private static async Task<IResult> GetLastConnectionAsync(
        [FromServices] IIPConnectionDatabaseService ipConnectionManager,
        [FromQuery] long userId)
    {
        try
        {
            var result = await ipConnectionManager.GetLastConnectionAsync(userId);
            return TypedResults.Json(result);
        }
        catch(Exception ex)
        {
            return TypedResults.Problem(ex.Message);
        }
    }
    #endregion

    #region GET GetUsersByIpFragmentAsync
    /// <summary>
    /// Maps the endpoint for retrieving user IDs by partial IP address match.
    /// </summary>
    private static WebApplication MapGetUsersByIpFragmentAsync(this WebApplication app)
    {
        app.MapGet($"{Url}/usersByIp", GetUsersByIpFragmentAsync).WithOpenApi(operation =>
        {
            operation.Responses.Add("500", new OpenApiResponse() { Description = "Failed to search users by IP fragment." });
            operation.Responses["200"].Description = "Users found successfully.";
            operation.Summary = "Find users by partial IP address match.";
            operation.Tags = [new OpenApiTag() { Name = "IP Connections" }];
            return operation;
        });

        return app;
    }

    /// <summary>
    /// Returns a list of user IDs that have used IP addresses containing the specified fragment.
    /// </summary>
    private static async Task<IResult> GetUsersByIpFragmentAsync(
        [FromServices] IIPConnectionDatabaseService ipConnectionManager,
        [FromQuery] string ipFragment)
    {
        try
        {
            var result = await ipConnectionManager.GetUsersByIpFragmentAsync(ipFragment);
            return TypedResults.Json(result);
        }
        catch(Exception ex)
        {
            return TypedResults.Problem(ex.Message);
        }
    }

    #endregion
}
