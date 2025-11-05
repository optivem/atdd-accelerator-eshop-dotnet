namespace Optivem.AtddAccelerator.EShop.SystemTest.Core.Clients.Api.Controllers;

public class EchoControllerClient : ApiClient
{
    public EchoControllerClient(string baseUrl) : base(baseUrl)
    {
    }

    public async Task<string> Echo()
    {
        return await GetAsync<string>("/api/echo");
    }
}
