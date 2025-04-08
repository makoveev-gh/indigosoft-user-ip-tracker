using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Indigosoft.User.Ip.Tracker.Infrastructure.Dto;

[DataContract]
public class IPConnectionRequest
{
    [DataMember]
    [JsonPropertyName("user_id")]
    public long UserId { get; set; }

    [DataMember]
    [JsonPropertyName("ip_address")]
    public string IpAddress { get; set; }

    [JsonIgnore]
    public DateTime Updated { get; set; } = DateTime.UtcNow;
}
