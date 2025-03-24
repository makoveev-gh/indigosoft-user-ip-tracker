using Indigosoft.User.Ip.Tracker.Database.Services;
using Indigosoft.User.Ip.Tracker.Infrastructure.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace Indigosoft.User.Ip.Tracker.Service.v1;

public static class UsersController
{
	/// <summary>Url.</summary>
	private static readonly string Url = "api/v1/users";

	public static WebApplication MapUsersEndpoints(this WebApplication app)
	{
		app.MapInsertUserAsync()
			.MapGetAllUsersAsync();
		return app;
	}

    #region GET GetAllUsersAsync
    /// <summary>
    /// Maps the endpoint for retrieving all users.
    /// </summary>
    /// <param name="app">The web application instance.</param>
    /// <returns>The configured web application.</returns>
    private static WebApplication MapGetAllUsersAsync(this WebApplication app)
    {
        app.MapGet($"{Url}/all", GetAllUsersAsync).WithOpenApi(operation =>
        {
            operation.Responses.Add("500", new OpenApiResponse() { Description = "Failed to retrieve users." });
            operation.Responses["200"].Description = "Users retrieved successfully.";
            operation.Summary = "Returns all users.";
            operation.Tags = [new OpenApiTag() { Name = "Users" }];
            return operation;
        });

        return app;
    }

    /// <summary>
    /// Retrieves all users from the database.
    /// </summary>
    /// <param name="usersManager">Service for accessing user data.</param>
    /// <returns>JSON result with the list of users.</returns>
    private static async Task<IResult> GetAllUsersAsync(
        [FromServices] IUsersDatabaseService usersManager)
    {
        try
        {
            var result = await usersManager.GetAllUsersAsync();
            return TypedResults.Json(result);
        }
        catch(Exception ex)
        {
            return TypedResults.Problem(ex.Message);
        }
    }
    #endregion

    #region POST InsertUserAsync
    /// <summary>
    /// Maps the endpoint for inserting a new user.
    /// </summary>
    /// <param name="app">The web application instance.</param>
    /// <returns>The configured web application.</returns>
    private static WebApplication MapInsertUserAsync(this WebApplication app)
    {
        app.MapPost($"{Url}/add", InsertUserAsync).WithOpenApi(operation =>
        {
            operation.Responses.Add("500", new OpenApiResponse() { Description = "Failed to insert user." });
            operation.Responses["200"].Description = "User inserted successfully.";
            operation.Summary = "Inserts a new user.";
            operation.Tags = [new OpenApiTag() { Name = "Users" }];
            return operation;
        });

        return app;
    }

    /// <summary>
    /// Inserts a new user into the database.
    /// </summary>
    /// <param name="usersManager">Service for managing user data.</param>
    /// <param name="data">The user to insert.</param>
    /// <returns>Result of the insert operation.</returns>
    private static async Task<IResult> InsertUserAsync(
        [FromServices] IUsersDatabaseService usersManager,
        [FromBody] UserData data)
    {
        try
        {
            await usersManager.InsertUserAsync(data);
            return TypedResults.Ok();
        }
        catch(Exception ex)
        {
            return TypedResults.Problem(ex.Message);
        }
    }

    #endregion
}

