using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VNBrokers.Brokers.Dnse;
using VNBrokers.Transport;

namespace VNBrokers.Tests.Brokers.Dnse;

[TestClass]
public sealed class DnseSigningTests
{
    [TestMethod]
    public void DnseSigner_adds_hmac_signature_headers_matching_python_sdk()
    {
        var signer = new DnseSigner(
            apiKey: "key",
            apiSecret: "secret",
            now: () => new DateTimeOffset(2024, 10, 2, 7, 44, 2, TimeSpan.Zero),
            nonce: () => "abc123");
        var request = new HttpTransportRequest(
            HttpMethod.Post,
            new Uri("https://api.dnse.example/accounts/orders?marketType=DERIVATIVE"));

        var signed = signer.Sign(request);

        signed.Headers.Should().NotBeNull();
        signed.Headers!["X-API-Key"].Should().Be("key");
        signed.Headers["X-Aux-Date"].Should().Be("Wed, 02 Oct 2024 07:44:02 +0000");
        signed.Headers["X-Signature"].Should().Be(
            "Signature keyId=\"key\",algorithm=\"hmac-sha256\"," +
            "headers=\"(request-target) x-aux-date\"," +
            "signature=\"TGoPDEvWPw8PKV8Ev8hTQcrCls%2FFZ7eWdx5uwc0oIMg%3D\"," +
            "nonce=\"abc123\"");
    }

    [TestMethod]
    public void DnseAuth_returns_no_signer_when_key_or_secret_is_missing()
    {
        new DnseAuth(new DnseConfig { ApiKey = "key" }).CreateSigner().Should().BeNull();
        new DnseAuth(new DnseConfig { ApiSecret = "secret" }).CreateSigner().Should().BeNull();
    }
}
