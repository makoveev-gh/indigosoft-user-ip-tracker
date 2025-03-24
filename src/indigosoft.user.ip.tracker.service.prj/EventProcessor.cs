using Indigosoft.User.Ip.Tracker.Database;
using Indigosoft.User.Ip.Tracker.Database.Dal;
using Indigosoft.User.Ip.Tracker.Infrastructure.Dto;
using Microsoft.EntityFrameworkCore;

namespace Indigosoft.User.Ip.Tracker.Service;
public class EventProcessor(IApplicationDbProvider dbProvider, EventQueue<IPConnectionRequest> eventQueue, EventProcessorSettings settings) : BackgroundService
{
    private readonly IApplicationDbProvider _applicationDbProvider = dbProvider;
    private static readonly Serilog.ILogger Log = Serilog.Log.ForContext<EventProcessor>();
    private readonly EventQueue<IPConnectionRequest> _eventQueue = eventQueue;
    private readonly EventProcessorSettings _settings = settings;


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Log.Information("EventProcessor started");

        if(_applicationDbProvider is null) throw new ArgumentNullException(nameof(_applicationDbProvider));

        int iterationCount = 0;

        while(!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if(_eventQueue.IsEmpty)
                {
                    await Task.Delay(_settings.EmptyQueueWaitTime, stoppingToken);
                    continue;
                }

                if(_eventQueue.TryDequeue(out var connect))
                {
                    using var context = await _applicationDbProvider.GetAsync(stoppingToken);

                    var entity = await context.UserIpConnections
                        .FirstOrDefaultAsync(x => x.UserId == connect.UserId && x.IpAddress == connect.IpAddress, stoppingToken);

                    if(entity == null)
                    {
                        await context.UserIpConnections.AddAsync(new UserIpConnection
                        {
                            Id = Guid.NewGuid(),
                            UserId = connect.UserId,
                            IpAddress = connect.IpAddress,
                            Created = connect.Updated,
                            Updated = connect.Updated
                        });
                    }
                    else
                    {
                        entity.Updated = connect.Updated;
                        context.UserIpConnections.Update(entity);
                    }

                    await context.SaveChangesAsync(stoppingToken);
                }

                iterationCount++;

                if(iterationCount >= _settings.IterationCountBeforeWait)
                {
                    await Task.Delay(_settings.IterationDelay, stoppingToken);
                    iterationCount = 0;
                }
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Error processing raw user IP events");
            }
        }

        Log.Information("EventProcessor stopped");
    }
} 