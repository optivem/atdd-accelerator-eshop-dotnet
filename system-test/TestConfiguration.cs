namespace Optivem.AtddAccelerator.EShop.SystemTest;

public static class TestConfiguration
{
    public static string BaseUrl => Environment.GetEnvironmentVariable("TEST_BASE_URL") ?? "http://localhost:8080";
    
    public static int WaitSeconds => int.TryParse(Environment.GetEnvironmentVariable("TEST_WAIT_SECONDS"), out var seconds) 
        ? seconds 
        : 10;
}
