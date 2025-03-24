using System.Threading;
using System.Threading.Tasks;

namespace Indigosoft.User.Ip.Tracker.Database;

public interface IInitializable
{
	bool IsInitialized { get; }

	Task InitializeAsync(CancellationToken cancellationToken);
}
