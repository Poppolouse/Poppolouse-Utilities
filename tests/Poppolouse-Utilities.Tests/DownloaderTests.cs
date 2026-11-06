using Poppolouse_Utilities.net;
using System.Net;
using Xunit;

namespace Poppolouse_Utilities.Tests;

public class DownloaderTests
{
    [SkippableFact]
    public async Task DownloadFileAsync_SavesFileFromUrl()
    {
        Skip.IfNot(await InternetAccess.IsAvailableAsync(), "No internet access.");

        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".html");
        try
        {
            using var expectedClient = new HttpClient();
            var expectedBytes = await expectedClient.GetByteArrayAsync("https://example.com/");
            await new Downloader().DownloadFileAsync("https://example.com/", path);

            Assert.True(File.Exists(path));
            Assert.Equal(expectedBytes, await File.ReadAllBytesAsync(path));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [SkippableFact]
    public async Task DownloadFileAsync_ThrowsForMissingPage()
    {
        Skip.IfNot(await InternetAccess.IsAvailableAsync(), "No internet access.");

        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".html");
        try
        {
            var exception = await Assert.ThrowsAsync<HttpRequestException>(() =>
                new Downloader().DownloadFileAsync("https://example.com/this-page-does-not-exist", path));

            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
            Assert.False(File.Exists(path));
        }
        finally
        {
            File.Delete(path);
        }
    }
}
