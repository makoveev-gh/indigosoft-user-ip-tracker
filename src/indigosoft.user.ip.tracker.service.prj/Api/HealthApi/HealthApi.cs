namespace Indigosoft.User.Ip.Tracker.Service.HealthApi;

/// <summary>Класс для проверки текущего состояние сервиса.</summary>
public static class HealthApi
{
	/// <summary>Подключение к методам Health.</summary>
	/// <param name="app">Веб приложение.</param>
	/// <returns>Веб приложение.</returns>
	public static WebApplication MapHealthEndpoints(this WebApplication app)
	{
		app.MapGetHealth()
			.MapHeadHealth();
		return app;
	}

	/// <summary>Получить текущее состояние сервиса через GET запрос.</summary>
	/// <param name="app">Веб приложение.</param>
	/// <returns>Веб приложение.</returns>
	private static WebApplication MapGetHealth(this WebApplication app)
	{
		app.MapGet("/health", GetHealth).WithOpenApi(operation => new(operation)
		{
			Tags = [new() { Name = "Health" }]
		});
		return app;
	}

	/// <summary>Получить текущее состояние сервиса через HEAD запрос.</summary>
	/// <param name="app">Веб приложение.</param>
	/// <returns>Веб приложение.</returns>
	private static WebApplication MapHeadHealth(this WebApplication app)
	{
		app.MapMethods("/health", ["HEAD"], GetHealth).WithOpenApi(operation => new(operation)
		{
			Tags = [new() { Name = "Health" }]
		});
		return app;
	}

	/// <summary>Проверяет доступность сервиса.</summary>
	/// <returns>Результат доступности сервиса.</returns>
	/// <response code="200">Сервис доступен.</response>
	/// <response code="404">Сервис недоступен.</response>
	private static IResult GetHealth() => Results.Ok();
}

