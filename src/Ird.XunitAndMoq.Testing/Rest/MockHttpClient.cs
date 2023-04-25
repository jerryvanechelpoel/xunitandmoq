using Moq;
using Moq.Language;
using Moq.Protected;
using System.Reflection;
using System.Text;

namespace Ird.XunitAndMoq.Testing.Rest;

public static class MockHttpClient
{
    public static HttpClient CreateMockHttpClient(params Task<HttpResponseMessage>[] responses)
    {
        Mock<HttpMessageHandler> httpMessageHandlerMock = new();
        ISetupSequentialResult<Task<HttpResponseMessage>> sequenceSetup =
            httpMessageHandlerMock.Protected()
                                  .SetupSequence<Task<HttpResponseMessage>>(nameof(HttpClient.SendAsync),
                                                                            ItExpr.IsAny<HttpRequestMessage>(),
                                                                            ItExpr.IsAny<CancellationToken>());

        foreach (var response in responses)
        {
            sequenceSetup.Returns(response);
        }

        return new HttpClient(httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };
    }

    public static Task<HttpResponseMessage> GetContentFromEmbeddedResource(Assembly resourceAssembly, string resourceName)
        => GetResponse(resourceAssembly.GetManifestResourceStream(resourceName));

    public static Task<HttpResponseMessage> GetContentFromString(string content)
        => GetResponse(new MemoryStream(Encoding.Default.GetBytes(content)));

    private static Task<HttpResponseMessage> GetResponse(Stream content)
    {
        HttpResponseMessage response = new()
        {
            Content = new StreamContent(content)
        };

        response.Content.Headers.Add("Content-Type", "application/json");

        return Task.FromResult(response);
    }
}