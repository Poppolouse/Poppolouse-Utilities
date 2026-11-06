using System.Net.Http;
using Poppolouse_Utilities.fm;

namespace Poppolouse_Utilities.net;

public class Downloader
{
    private readonly HttpClient _httpClient = new();

    /// <summary>
    /// Downloads a file from the specified URL and save it to a stream.
    /// This stream can then be used to write the file to disk or process it in memory.
    /// </summary>
    /// <param name="url">the URL of the file to download</param>
    /// <returns> The stream containing the downloaded file. </returns>
    /// <example>
    /// <code>
    ///     var downloader = new Downloader();
    ///     var stream = await downloader.GetStreamAsync("https://example.com/file.txt");
    ///     // Do something with the downloaded stream, e.g., write it to a file or process it in memory.
    /// </code>
    /// </example>
    private async Task<Stream> GetStreamAsync(string url)
    {
        try
        {
            Stream stream = await _httpClient.GetStreamAsync(url);
            return stream;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task DownloadFileAsync(string url, string filePath)
    {
        try
        {
            using Stream stream = await GetStreamAsync(url);
            var fileManager = new FileManager();
            await fileManager.writeStream(filePath, stream);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    
}