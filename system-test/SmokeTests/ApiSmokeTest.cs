using System.Net;

namespace Optivem.AtddAccelerator.EShop.SystemTest.SmokeTests;

public class ApiSmokeTest
{
    [Fact]
    public async Task Echo_ShouldReturn200OK()
    {
        using var client = new HttpClient();
        var response = await client.GetAsync($"{TestConfiguration.BaseUrl}/api/echo");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}