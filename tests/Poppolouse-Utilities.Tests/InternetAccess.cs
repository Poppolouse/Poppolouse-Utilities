namespace Poppolouse_Utilities.Tests;

public static class InternetAccess
{
    private static readonly Lazy<Task<bool>> Available = new(CheckAsync);

    public static Task<bool> IsAvailableAsync() => Available.Value;

    private static async Task<bool> CheckAsync()
    {
        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
            using var response = await client.GetAsync("https://example.com/");
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            return false;
        }
        catch (TaskCanceledException)
        {
            return false;
        }
    }
}
