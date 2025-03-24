using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Indigosoft.User.Ip.Tracker.Infrastructure.Dto;

[DataContract]
public class UserData
{
    [DataMember]
    [JsonPropertyName("id")]
    public required long Id { get; set; }

    [DataMember]
    [JsonPropertyName("name")]
    public required string Name { get; set; }
}
