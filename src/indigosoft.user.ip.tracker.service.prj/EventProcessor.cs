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

                var batch = new List<IPConnectionRequest>();

                while(batch.Count < _settings.BatchSize && _eventQueue.TryDequeue(out var item))
                {
                    batch.Add(item);
                }

                if(batch.Count > 0)
                {
                    using var context = await _applicationDbProvider.GetAsync(stoppingToken);

                    var userIds = batch.Select(x => x.UserId).Distinct().ToList();
                    var ipAddresses = batch.Select(x => x.IpAddress).Distinct().ToList();

                    var existing = await context.UserIpConnections
                        .Where(x => userIds.Contains(x.UserId) && ipAddresses.Contains(x.IpAddress))
                        .ToListAsync(stoppingToken);

                    var toAdd = new List<UserIpConnection>();

                    foreach(var connect in batch)
                    {
                        var entity = existing.FirstOrDefault(x => x.UserId == connect.UserId && x.IpAddress == connect.IpAddress);
                        if(entity == null)
                        {
                            toAdd.Add(new UserIpConnection
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
                        }
                    }

                    if(toAdd.Count > 0)
                        await context.UserIpConnections.AddRangeAsync(toAdd, stoppingToken);

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