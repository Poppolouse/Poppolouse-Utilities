using Poppolouse_Utilities.fm;

namespace Poppolouse_Utilities.Tests;

public class FileManagerTests
{
    private readonly FileManager _fileManager = new();

    [Fact]
    public async Task WriteStream_WritesContentToFile()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".txt");
        try
        {
            using var stream = new MemoryStream("hello"u8.ToArray());

            var result = await _fileManager.writeStream(path, stream);

            Assert.Equal(0, result);
            Assert.Equal("hello", await File.ReadAllTextAsync(path));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task WriteStream_WithDestination_WritesContentToFile()
    {
        var directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, "output.txt");
        try
        {
            using var stream = new MemoryStream("hello"u8.ToArray());

            var result = await _fileManager.writeStream("output.txt", stream, directory);

            Assert.Equal(0, result);
            Assert.Equal("hello", await File.ReadAllTextAsync(path));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
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

            await Assert.ThrowsAsync<IOException>(() => _fileManager.writeStream(path, stream));
            Assert.Equal("existing", await File.ReadAllTextAsync(path));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task WriteStream_WithDestination_ThrowsWhenFileExistsAndPreservesContent()
    {
        var directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, "output.txt");
        await File.WriteAllTextAsync(path, "existing");
        try
        {
            using var stream = new MemoryStream("replacement"u8.ToArray());

            await Assert.ThrowsAsync<IOException>(() => _fileManager.writeStream("output.txt", stream, directory));

            Assert.Equal("existing", await File.ReadAllTextAsync(path));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Theory]
    [InlineData(null, "file")]
    [InlineData("name.txt", "stream")]
    public async Task WriteStream_RejectsNullArguments(string? file, string parameterName)
    {
        Stream? stream = parameterName == "stream" ? null : new MemoryStream();

        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _fileManager.writeStream(file!, stream!));

        Assert.Equal(parameterName, exception.ParamName);
    }

    [Theory]
    [InlineData(null, "file")]
    [InlineData("name.txt", "stream")]
    [InlineData("name.txt", "destination")]
    public async Task WriteStream_WithDestination_RejectsNullArguments(string? file, string parameterName)
    {
        Stream? stream = parameterName == "stream" ? null : new MemoryStream();
        string? destination = parameterName == "destination" ? null : Path.GetTempPath();

        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _fileManager.writeStream(file!, stream!, destination!));

        Assert.Equal(parameterName, exception.ParamName);
    }

    [Fact]
    public async Task WriteStream_EmptyStreamCreatesEmptyFile()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".txt");
        try
        {
            using var stream = new MemoryStream();

            await _fileManager.writeStream(path, stream);

            Assert.Equal(0, new FileInfo(path).Length);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task WriteStream_WithDestination_EmptyStreamCreatesEmptyFile()
    {
        var directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(directory);
        try
        {
            using var stream = new MemoryStream();

            await _fileManager.writeStream("empty.txt", stream, directory);

            Assert.Equal(0, new FileInfo(Path.Combine(directory, "empty.txt")).Length);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task WriteStream_PropagatesSourceReadFailure(bool withDestination)
    {
        var directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, "output.txt");
        try
        {
            using var stream = new ReadFailureStream();

            if (withDestination)
            {
                var expected = new IOException("source read failed");
                stream.Failure = expected;
                var exception = await Record.ExceptionAsync(() => _fileManager.writeStream("output.txt", stream, directory));
                Assert.Same(expected, exception is AggregateException aggregate ? aggregate.GetBaseException() : exception);
            }
            else
            {
                var expected = new IOException("source read failed");
                stream.Failure = expected;
                var exception = await Assert.ThrowsAsync<IOException>(() => _fileManager.writeStream(path, stream));
                Assert.Same(expected, exception);
            }
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private sealed class ReadFailureStream : MemoryStream
    {
        public IOException Failure { get; set; } = new("source read failed");

        public override int Read(byte[] buffer, int offset, int count) => throw Failure;

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) =>
            ValueTask.FromException<int>(Failure);
    }
}
