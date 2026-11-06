using Poppolouse_Utilities.fm;

namespace Poppolouse_Utilities.Tests;

public class FileManagerTests
{
    [Fact]
    public async Task WriteStream_WritesContentToFile()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".txt");
        try
        {
            using var stream = new MemoryStream("hello"u8.ToArray());

            var result = await new FileManager().writeStream(path, stream);

            Assert.Equal(0, result);
            Assert.Equal("hello", await File.ReadAllTextAsync(path));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task WriteStream_ThrowsWhenFileExists()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".txt");
        File.WriteAllText(path, "existing");
        try
        {
            using var stream = new MemoryStream("hello"u8.ToArray());

            await Assert.ThrowsAsync<IOException>(() => new FileManager().writeStream(path, stream));
        }
        finally
        {
            File.Delete(path);
        }
    }
}
