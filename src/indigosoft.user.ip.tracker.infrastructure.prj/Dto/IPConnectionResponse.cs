using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Indigosoft.User.Ip.Tracker.Infrastructure.Dto;

[DataContract]
public class IPConnectionResponse
{
    [DataMember]
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [DataMember]
    [JsonPropertyName("user_id")]
    public long UserId { get; set; }

    [DataMember]
    [JsonPropertyName("ip_address")]
    public string IpAddress { get; set; }

    [DataMember]
    [JsonPropertyName("updated")]
    public DateTime Updated { get; set; }
}
