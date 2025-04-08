using Indigosoft.User.Ip.Tracker.Database;

namespace Indigosoft.User.Ip.Tracker.Service;

/// <summary>Фоновый сервис инициализации.</summary>
/// <remarks>Создаёт экземпляр класса <see cref="InitializationService"/>.</remarks>
public class InitializationService(IEnumerable<IInitializable> services) : BackgroundService
{

	#region Data

	private readonly IReadOnlyList<IInitializable> _services = services.ToList();

	private static readonly Serilog.ILogger Log = Serilog.Log.ForContext<InitializationService>();

	#endregion

	#region Overrides

	/// <inheritdoc/>
	protected override async Task ExecuteAsync(
		CancellationToken cancellationToken)
	{
		foreach(var service in _services)
		{
			try
			{
				await service.InitializeAsync(cancellationToken);
				Log.Information($"Service {service.GetType().Name:serviceName} initialized");
			}
			catch(Exception ex)
			{
				Log.Error(
					ex,
					$"Service {service.GetType().Name:serviceName} initialize failed.");
			}
		}
	}

	#endregion
}
