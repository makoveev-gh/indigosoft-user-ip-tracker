﻿using Xunit;
using Indigosoft.User.Ip.Tracker.Database.Helpers;

namespace Indigosoft.User.Ip.Tracker.Tests.Helpers;

public class RegexHelperTests
{
    [Theory]
    [InlineData("192.168.0.1")]
    [InlineData("8.8.8.8")]
    [InlineData("0.0.0.0")]
    [InlineData("255.255.255.255")]
    public void IsIPv4_ValidIPv4_ReturnsTrue(string ip)
    {
        Assert.True(RegexHelper.IsIPv4(ip));
    }

    [Theory]
    [InlineData("256.256.256.256")]
    [InlineData("192.168.0")]
    [InlineData("192.168.0.256")]
    [InlineData("abc.def.ghi.jkl")]
    [InlineData(" 192.168.0.1 ")]
    public void IsIPv4_InvalidIPv4_ReturnsFalse(string ip)
    {
        Assert.False(RegexHelper.IsIPv4(ip));
    }

    [Theory]
    [InlineData("2001:0db8:85a3:0000:0000:8a2e:0370:7334")]
    [InlineData("::1")]
    [InlineData("fe80::1ff:fe23:4567:890a")]
    [InlineData("2001:db8::8a2e:370:7334")]
    public void IsIPv6_ValidIPv6_ReturnsTrue(string ip)
    {
        Assert.True(RegexHelper.IsIPv6(ip));
    }

    [Theory]
    [InlineData("2001:0db8:85a3::8a2e::7334")]
    [InlineData("::gggg")]
    [InlineData("12345::abcd")]
    [InlineData(":::")]
    [InlineData(" 2001:db8::1 ")]
    public void IsIPv6_InvalidIPv6_ReturnsFalse(string ip)
    {
        Assert.False(RegexHelper.IsIPv6(ip));
    }
}
