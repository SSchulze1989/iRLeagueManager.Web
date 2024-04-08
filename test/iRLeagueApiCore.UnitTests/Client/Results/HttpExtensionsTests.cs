using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.Results;
using iRLeagueApiCore.Common.Responses;
using Microsoft.Extensions.Logging;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;

namespace iRLeagueApiCore.UnitTests.Client.Results;

public sealed class HttpExtensionsTests
{
    private const string testString = "TestMessage";
    private const long testValue = 42;
    private const string testToken = "asgöahsgpasiakojsatlwetaet";

    private HttpClientWrapperFactory ClientWrapperFactory { get; init; }

    public HttpExtensionsTests()
    {
        var mockLoggerFactory = new Mock<ILoggerFactory>();
        mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        ClientWrapperFactory = new(mockLoggerFactory.Object, null);
    }

    [Fact]
    public async Task ShouldReturnActionResult()
    {
        var content = new StringContent(TestContent.AsJson());
        var testMessage = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = content
        };

        var result = await testMessage.ToClientActionResultAsync<TestContent>(default);
        Assert.True(result.Success);
        Assert.Equal("Success", result.Status);
        Assert.Equal(testString, result.Content?.String);
        Assert.Equal(testValue, result.Content?.Value);
    }

    [Fact]
    public async Task ShouldReturnNoContentActionResult()
    {
        var testMessge = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.NoContent,
            Content = null
        };

        var result = await testMessge.ToClientActionResultAsync<TestContent>(default);
        Assert.True(result.Success);
        Assert.Equal("Success", result.Status);
        Assert.Equal(HttpStatusCode.NoContent, result.HttpStatusCode);
        Assert.Null(result.Content);
    }

    [Fact]
    public async Task ShouldReturnBadRequestActionResult()
    {
        var response = new BadRequestResponse()
        {
            Status = "Bad Request",
            Errors = new ValidationError[] { new ValidationError() { Error = "TestError", Property = "String", Value = 42 } },
        };
        var content = new StringContent(JsonConvert.SerializeObject(response));
        var testMessage = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.BadRequest,
            Content = content
        };

        var result = await testMessage.ToClientActionResultAsync<TestContent>(default);
        Assert.False(result.Success);
        Assert.Equal(HttpStatusCode.BadRequest, result.HttpStatusCode);
        Assert.Equal("Bad Request", result.Status);
        Assert.Null(result.Content);
        Assert.Single(result.Errors);
        var error = Assert.IsType<ValidationError>(result.Errors.First());
        Assert.Equal("TestError", error.Error);
        Assert.Equal("String", error.Property);
    }

    [Fact]
    public async Task ShouldReturnForbiddenActionResult()
    {
        var testMessage = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.Forbidden,
            Content = default
        };

        var result = await testMessage.ToClientActionResultAsync<TestContent>(default);
        Assert.False(result.Success);
        Assert.Equal(HttpStatusCode.Forbidden, result.HttpStatusCode);
        Assert.Equal("Forbidden", result.Status);
        Assert.Null(result.Content);
    }

    [Fact]
    public async Task ShouldReturnInternalServerErrorResult()
    {
        var testMessage = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Content = default,
        };

        var result = await testMessage.ToClientActionResultAsync<TestContent>(default);
        Assert.False(result.Success);
        Assert.Equal(HttpStatusCode.InternalServerError, result.HttpStatusCode);
        Assert.Equal("Internal server Error", result.Status);
        Assert.Null(result.Content);
    }

    [Fact]
    public async Task ShouldNotThrowOnWrongContentType()
    {
        var content = new StringContent(TestContent.AsJson());
        var testMessage = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = content,
        };

        var result = await testMessage.ToClientActionResultAsync<WrongContent>(default);
    }

    [Fact]
    public async Task ShouldUnwrapHttpRequest()
    {
        var content = new StringContent(TestContent.AsJson());
        var request = Task.FromResult(new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = content,
        });

        var result = await request.AsClientActionResultAsync<TestContent>(default);
        Assert.True(result.Success);
        Assert.Equal("Success", result.Status);
        Assert.Equal(testString, result.Content?.String);
        Assert.Equal(testValue, result.Content?.Value);
    }

    [Fact]
    public async Task ShouldAddJWTHeader()
    {
        var mockMessageHandler = new Mock<HttpMessageHandler>();
        var content = new StringContent(TestContent.AsJson());
        mockMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Content = null
            });
        mockMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                x.Headers.Authorization != null && x.Headers.Authorization.Parameter == testToken), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = content
            });
        var mockTokenProvider = new Mock<IAsyncTokenProvider>();
        mockTokenProvider.Setup(x => x.GetAccessTokenAsync())
            .ReturnsAsync(testToken);
        var client = new HttpClient(mockMessageHandler.Object);
        var wrapper = ClientWrapperFactory.Create(client, mockTokenProvider.Object);

        var result = await wrapper.SendRequest(new HttpRequestMessage(HttpMethod.Get, "https://example.com"), default);
        Assert.True(result.IsSuccessStatusCode);
    }

    [Fact]
    public async Task ShouldWrapHttpGetRequest()
    {
        var mockMessageHandler = new Mock<HttpMessageHandler>();
        var content = new StringContent(TestContent.AsJson());
        mockMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = content
            });
        var mockTokenProvider = new Mock<IAsyncTokenProvider>();
        var client = new HttpClient(mockMessageHandler.Object);
        var wrapper = ClientWrapperFactory.Create(client, mockTokenProvider.Object);

        var result = await wrapper.GetAsClientActionResult<TestContent>("https://example.com");
        Assert.True(result.Success);
    }

    [Fact]
    public async Task ShouldWrapHttpPostRequest()
    {
        var mockMessageHandler = new Mock<HttpMessageHandler>();
        var content = new StringContent(TestContent.AsJson());
        mockMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = content
            });
        var mockTokenProvider = new Mock<IAsyncTokenProvider>();
        var client = new HttpClient(mockMessageHandler.Object);
        var wrapper = ClientWrapperFactory.Create(client, mockTokenProvider.Object);
        var model = new TestContent();

        var result = await wrapper.PostAsClientActionResult<TestContent>("https://example.com", model);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task ShouldWrapHttpPutRequest()
    {
        var mockMessageHandler = new Mock<HttpMessageHandler>();
        var content = new StringContent(TestContent.AsJson());
        mockMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = content
            });
        var mockTokenProvider = new Mock<IAsyncTokenProvider>();
        var client = new HttpClient(mockMessageHandler.Object);
        var wrapper = ClientWrapperFactory.Create(client, mockTokenProvider.Object);
        var model = new TestContent();

        var result = await wrapper.PutAsClientActionResult<TestContent>("https://example.com", model);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task ShouldWrapHttpDeleteRequest()
    {
        var mockMessageHandler = new Mock<HttpMessageHandler>();
        var content = new StringContent(TestContent.AsJson());
        mockMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = content
            });
        var mockTokenProvider = new Mock<IAsyncTokenProvider>();
        var client = new HttpClient(mockMessageHandler.Object);
        var wrapper = ClientWrapperFactory.Create(client, mockTokenProvider.Object);

        var result = await wrapper.DeleteAsClientActionResult("https://example.com");
        Assert.True(result.Success);
    }

    private class TestContent
    {
        public string String { get; set; } = testString;
        public long Value { get; set; } = testValue;

        public static string AsJson()
        {
            return JsonConvert.SerializeObject(new TestContent());
        }
    }

    private class WrongContent
    {
        public string WrongString { get; set; } = string.Empty;
    }
}
