using System.Runtime.Serialization;

namespace Indigosoft.User.Ip.Tracker.Service;
 public class EventProcessorSettings
{
    /// <summary>
    /// Gets or sets the time (in milliseconds) to wait when the event queue is empty.
    /// This parameter controls how long the processor waits before checking the queue again.
    /// </summary>
    [DataMember]
    public int EmptyQueueWaitTime { get; init; } = 1000; 

    /// <summary>
    /// Gets or sets the time (in milliseconds) to wait between each iteration of the processing loop.
    /// This parameter is used to add a delay after processing each event before starting the next iteration.
    /// </summary>
    [DataMember]
    public int IterationDelay { get; init; } = 500; 

    /// <summary>
    /// Gets or sets the number of iterations to perform before introducing a wait time.
    /// This parameter controls how many events will be processed before applying the delay defined by IterationDelay.
    /// </summary>
    [DataMember]
    public int IterationCountBeforeWait { get; init; } = 1000;

    /// <summary>
    /// Gets or sets the maximum number of items that the handler takes out of the queue in one iteration.
    /// </summary>
    [DataMember]
    public int BatchSize { get; init; } = 10;
}
