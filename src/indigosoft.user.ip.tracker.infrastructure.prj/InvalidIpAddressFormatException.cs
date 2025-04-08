namespace Indigosoft.User.Ip.Tracker.Infrastructure;
public class InvalidIpAddressFormatException : Exception
{
    public InvalidIpAddressFormatException(string ip)
        : base($"IP address '{ip}' does not match expected IPv4 or IPv6 format.") { }
}
